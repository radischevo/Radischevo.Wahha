using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	public static class MappedDbSerializerBuilder
	{
		#region Nested types
		private sealed class DelegatedDbSerializer<TEntity> : DbSerializer<TEntity>
		{
			#region Instance Fields
			private MetaType _metaType;
			private Func<MetaType, TEntity, DbQueryStatement, IValueSet> _serializer;
			private Func<MetaType, TEntity, IDbValueSet, TEntity> _deserializer;
			#endregion
			
			#region Constructors
			public DelegatedDbSerializer(MetaType metaType,
				Func<MetaType, TEntity, DbQueryStatement, IValueSet> serializer, 
				Func<MetaType, TEntity, IDbValueSet, TEntity> deserializer)
			{
				_metaType = metaType;
				_serializer = serializer;
				_deserializer = deserializer;
			}
			#endregion
			
			#region Instance Methods
			protected override TEntity CreateInstance (IDbValueSet source)
			{
				return Activator.CreateInstance<TEntity>(); // possibly IL emit
			}
			
			protected override TEntity Deserialize (TEntity entity, IDbValueSet source)
			{
				return _deserializer(_metaType, entity, source);
			}
			
			public override IValueSet Serialize (TEntity entity, DbQueryStatement statement)
			{
				return _serializer(_metaType, entity, statement);
			}
			#endregion
		}
		#endregion
		
		#region Static Fields
		private static readonly MethodInfo _valueSetContainsKeyMethod = typeof(ValueSet).GetMethod("ContainsAny", 
			BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(IValueSet), typeof(string) }, null);
		private static readonly MethodInfo _valueSetGetItemMethod = typeof(ValueSet).GetMethod("GetValue", 
			BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(IValueSet), typeof(string) }, null);
		private static readonly MethodInfo _metaTypeGetMembersMethod = typeof(MetaType).GetProperty("Members").GetGetMethod();
		private static readonly MethodInfo _metaTypeGetMemberMethod = typeof(IKeyedEnumerable<string, MetaMember>)
			.GetProperty("Item", new Type[] { typeof(string) }).GetGetMethod();
		private static readonly MethodInfo _metaMemberGetAccessorMethod = typeof(MetaMember).GetProperty("Accessor").GetGetMethod();
		private static readonly MethodInfo _metaColumnGetBinderMethod = typeof(MetaColumn).GetProperty("Binder").GetGetMethod();
		private static readonly MethodInfo _columnBinderToPropertyValueMethod = typeof(IDbColumnBinder).GetMethod("ToPropertyValue");
		private static readonly MethodInfo _metaAccessorGetValueMethod = typeof(MetaAccessor).GetMethod("GetValue");
		private static readonly MethodInfo _metaAccessorSetValueMethod = typeof(MetaAccessor).GetMethod("SetValue");
		
		// Association Loaders
		private static readonly MethodInfo _associationLoaderScalarMethod = typeof(MappedAssociationLoader)
			.GetMethod("Scalar", BindingFlags.Public | BindingFlags.Static, null, 
				new Type[] { typeof(MetaColumn), typeof(IValueSet) }, null);
		private static readonly MethodInfo _associationLoaderSingleMethod = typeof(MappedAssociationLoader)
			.GetMethod("Single", BindingFlags.Public | BindingFlags.Static, null, 
				new Type[] { typeof(MetaAssociation), typeof(IValueSet) }, null);
		private static readonly MethodInfo _associationLoaderMultipleMethod = typeof(MappedAssociationLoader)
			.GetMethod("Multiple", BindingFlags.Public | BindingFlags.Static, null, 
				new Type[] { typeof(MetaAssociation), typeof(IValueSet) }, null);
		#endregion
		
		#region Static Methods
		public static IDbSerializer Build (MetaType type)
		{
			Type entityType = type.Type;
			
			// first build the deserializer method
			DynamicMethod method = new DynamicMethod("Deserialize",	entityType, 
				new Type[] { typeof(MetaType), entityType, typeof(IDbValueSet) }, entityType, true);

            ILGenerator generator = method.GetILGenerator();
			short memberVariableIndex = (short)generator.DeclareLocal(typeof(MetaMember)).LocalIndex;
			
			foreach (MetaMember member in type.Members)
			{
				if (member.IsAssociation)
				{
					MetaAssociation association = (MetaAssociation)member;
					if (association.Accessor.IsDeferred)
						EmitDeferredAssociationAssignment(generator, memberVariableIndex, association);
				}
				else
				{
					MetaColumn column = (MetaColumn)member;
					if (column.Accessor.IsDeferred)
						EmitDeferredColumnAssignment(generator, memberVariableIndex, column);
					else
						EmitColumnAssignment(generator, memberVariableIndex, column);
				}
			}
			
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ret);
			
			Type serializerType = typeof(DelegatedDbSerializer<>).MakeGenericType(entityType);
			object deserializer = method.CreateDelegate(typeof(Func<,,,>).MakeGenericType(
				typeof(MetaType), entityType, typeof(IDbValueSet), entityType));
			
			return (IDbSerializer)Activator.CreateInstance(serializerType, new object[] { type, null, deserializer }); 
		}
		
		private static void EmitColumnAssignment (ILGenerator generator, short memberVariableIndex, MetaColumn member) 
		{
			string memberName = member.GetMemberKey();
			
			Label containsKeyLabel = generator.DefineLabel();
			
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Ldstr, memberName);
			generator.Emit(OpCodes.Call, _valueSetContainsKeyMethod);
			generator.Emit(OpCodes.Brfalse, containsKeyLabel);
			
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Callvirt, _metaTypeGetMembersMethod);
			generator.Emit(OpCodes.Ldstr, memberName);
			generator.Emit(OpCodes.Callvirt, _metaTypeGetMemberMethod);
			generator.Emit(OpCodes.Stloc, memberVariableIndex);
			generator.Emit(OpCodes.Ldloc, memberVariableIndex);
			generator.Emit(OpCodes.Callvirt, _metaMemberGetAccessorMethod);
			generator.Emit(OpCodes.Ldarg_1);
			
			if (member.Binder != null) 
			{
				generator.Emit(OpCodes.Ldloc, memberVariableIndex);
				generator.Emit(OpCodes.Castclass, typeof(MetaColumn));
				generator.Emit(OpCodes.Callvirt, _metaColumnGetBinderMethod);
			}
			
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Ldstr, memberName);
			
			if (member.Binder != null)
			{
				generator.Emit(OpCodes.Call, _valueSetGetItemMethod.MakeGenericMethod(typeof(object)));
				generator.Emit(OpCodes.Callvirt, _columnBinderToPropertyValueMethod);
			}
			else
			{
				generator.Emit(OpCodes.Call, _valueSetGetItemMethod.MakeGenericMethod(member.Type));
				if (member.Type.IsValueType)
					generator.Emit(OpCodes.Box, member.Type);
			}
			
			generator.Emit(OpCodes.Callvirt, _metaAccessorSetValueMethod);
			generator.MarkLabel(containsKeyLabel);
		}
		
		private static void EmitDeferredColumnAssignment (ILGenerator generator, short memberVariableIndex, MetaColumn member)
		{
			string memberName = member.GetMemberKey();
			
			Type linkedMemberType = member.Accessor.Type;
			Type linkType = typeof(ILink<>).MakeGenericType(linkedMemberType);
			Type linkSourceType = typeof(Func<>).MakeGenericType(linkedMemberType);
			Type associatorLoaderType = typeof(IAssociationLoader<>).MakeGenericType(linkedMemberType);
			
			MethodInfo associationLoadMethod = associatorLoaderType.GetMethod("Load");
			MethodInfo linkSourceSetterMethod = linkType.GetProperty("Source").GetSetMethod();
			MethodInfo linkValueSetterMethod = linkType.GetProperty("Value").GetSetMethod();
			
			ConstructorInfo typeConstructor = member.Type.GetConstructor(Type.EmptyTypes);
			ConstructorInfo linkSourceConstructor = linkSourceType.GetConstructor(new Type[] { typeof(object), typeof(IntPtr) });
			
			short variableIndex = (short)generator.DeclareLocal(linkType).LocalIndex;
			
			Label linkIsNullLabel = generator.DefineLabel();
			Label containsKeyLabel = generator.DefineLabel();
			Label skipLabel = generator.DefineLabel();
			
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Callvirt, _metaTypeGetMembersMethod);
			generator.Emit(OpCodes.Ldstr, memberName);
			generator.Emit(OpCodes.Callvirt, _metaTypeGetMemberMethod);
			generator.Emit(OpCodes.Stloc, memberVariableIndex);
			generator.Emit(OpCodes.Ldloc, memberVariableIndex);
			generator.Emit(OpCodes.Callvirt, _metaMemberGetAccessorMethod);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Callvirt, _metaAccessorGetValueMethod);
			generator.Emit(OpCodes.Castclass, linkType);
			
			generator.Emit(OpCodes.Stloc, variableIndex);
			generator.Emit(OpCodes.Ldloc, variableIndex);
			
			if (typeConstructor == null)
				generator.Emit(OpCodes.Brtrue, skipLabel);
			else
				generator.Emit(OpCodes.Brtrue, linkIsNullLabel);
				
			generator.Emit(OpCodes.Newobj, typeConstructor);
			generator.Emit(OpCodes.Stloc, variableIndex);
			
			generator.MarkLabel(linkIsNullLabel);
			generator.Emit(OpCodes.Ldloc, variableIndex);
			generator.Emit(OpCodes.Ldloc, memberVariableIndex);
			generator.Emit(OpCodes.Castclass, typeof(MetaColumn));
			generator.Emit(OpCodes.Ldarg_2);		
			generator.Emit(OpCodes.Call, _associationLoaderScalarMethod.MakeGenericMethod(linkedMemberType));
			generator.Emit(OpCodes.Dup);
			generator.Emit(OpCodes.Ldvirtftn, associationLoadMethod);
			generator.Emit(OpCodes.Newobj, linkSourceConstructor);
			generator.Emit(OpCodes.Callvirt, linkSourceSetterMethod);
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Ldstr, memberName);
			generator.Emit(OpCodes.Call, _valueSetContainsKeyMethod);
			generator.Emit(OpCodes.Brfalse, containsKeyLabel);
			generator.Emit(OpCodes.Ldloc, variableIndex);
			
			if (member.Binder != null) 
			{
				generator.Emit(OpCodes.Ldloc, memberVariableIndex);
				generator.Emit(OpCodes.Castclass, typeof(MetaColumn));
				generator.Emit(OpCodes.Callvirt, _metaColumnGetBinderMethod);
			}
			
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Ldstr, memberName);
			
			if (member.Binder != null)
			{
				generator.Emit(OpCodes.Call, _valueSetGetItemMethod.MakeGenericMethod(typeof(object)));
				generator.Emit(OpCodes.Callvirt, _columnBinderToPropertyValueMethod);
				
				if (member.Type.IsValueType)
					generator.Emit(OpCodes.Unbox_Any, linkedMemberType);
				else
					generator.Emit(OpCodes.Castclass, linkedMemberType);
			}
			else 
				generator.Emit(OpCodes.Call, _valueSetGetItemMethod.MakeGenericMethod(linkedMemberType));
			
			generator.Emit(OpCodes.Callvirt, linkValueSetterMethod);
			
			generator.MarkLabel(containsKeyLabel);
			
			generator.Emit(OpCodes.Ldloc, memberVariableIndex);
			generator.Emit(OpCodes.Callvirt, _metaMemberGetAccessorMethod);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloc, variableIndex);
			generator.Emit(OpCodes.Callvirt, _metaAccessorSetValueMethod);
			
			generator.MarkLabel(skipLabel);
		}
		
		private static void EmitDeferredAssociationAssignment (ILGenerator generator, short memberVariableIndex, MetaAssociation member)
		{
			string memberName = member.GetMemberKey();
			
			Type linkedMemberType = member.Accessor.Type;
			Type linkedElementType = linkedMemberType.GetSequenceElementType();
			Type linkType = typeof(ILink<>).MakeGenericType(linkedMemberType);
			Type linkSourceType = typeof(Func<>).MakeGenericType(linkedMemberType);
			Type associatorLoaderType = typeof(IAssociationLoader<>).MakeGenericType(linkedMemberType);
			
			MethodInfo associatorFactoryMethod = (member.IsMany) 
				?  _associationLoaderMultipleMethod.MakeGenericMethod(linkedElementType)
				:  _associationLoaderSingleMethod.MakeGenericMethod(linkedElementType);
			MethodInfo associationLoadMethod = associatorLoaderType.GetMethod("Load");
			MethodInfo linkSourceSetterMethod = linkType.GetProperty("Source").GetSetMethod();
			MethodInfo linkValueSetterMethod = linkType.GetProperty("Value").GetSetMethod();
			
			ConstructorInfo typeConstructor = member.Type.GetConstructor(Type.EmptyTypes);
			ConstructorInfo linkSourceConstructor = linkSourceType.GetConstructor(new Type[] { typeof(object), typeof(IntPtr) });
			
			short variableIndex = (short)generator.DeclareLocal(linkType).LocalIndex;
			
			Label linkIsNullLabel = generator.DefineLabel();
			Label skipLabel = generator.DefineLabel();
			
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Callvirt, _metaTypeGetMembersMethod);
			generator.Emit(OpCodes.Ldstr, memberName);
			generator.Emit(OpCodes.Callvirt, _metaTypeGetMemberMethod);
			generator.Emit(OpCodes.Stloc, memberVariableIndex);
			generator.Emit(OpCodes.Ldloc, memberVariableIndex);
			generator.Emit(OpCodes.Callvirt, _metaMemberGetAccessorMethod);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Callvirt, _metaAccessorGetValueMethod);
			generator.Emit(OpCodes.Castclass, linkType);
			
			generator.Emit(OpCodes.Stloc, variableIndex);
			generator.Emit(OpCodes.Ldloc, variableIndex);
			
			if (typeConstructor == null)
				generator.Emit(OpCodes.Brtrue, skipLabel);
			else
				generator.Emit(OpCodes.Brtrue, linkIsNullLabel);
				
			generator.Emit(OpCodes.Newobj, typeConstructor);
			generator.Emit(OpCodes.Stloc, variableIndex);
			
			generator.MarkLabel(linkIsNullLabel);
			
			generator.Emit(OpCodes.Ldloc, variableIndex);
			generator.Emit(OpCodes.Ldloc, memberVariableIndex);
			generator.Emit(OpCodes.Castclass, typeof(MetaAssociation));
			generator.Emit(OpCodes.Ldarg_2);
			generator.Emit(OpCodes.Call, associatorFactoryMethod);
			generator.Emit(OpCodes.Dup);
			generator.Emit(OpCodes.Ldvirtftn, associationLoadMethod);
			generator.Emit(OpCodes.Newobj, linkSourceConstructor);
			generator.Emit(OpCodes.Callvirt, linkSourceSetterMethod);
			
			// here we attempt to decode and write a value.			
			//generator.MarkLabel(containsKeyLabel);
			
			generator.Emit(OpCodes.Ldloc, memberVariableIndex);
			generator.Emit(OpCodes.Callvirt, _metaMemberGetAccessorMethod);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Ldloc, variableIndex);
			generator.Emit(OpCodes.Callvirt, _metaAccessorSetValueMethod);
			
			generator.MarkLabel(skipLabel);
		}
		#endregion
	}
}