using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Radischevo.Wahha.Core.Serialization
{
	public class RepeatedFieldIO : FieldIOBase
	{
		#region Nested Types
		protected class ForEachLoop
		{
			#region Instance Fields
			private readonly ILGenerator _generator;
			private readonly MethodInfo _getField;
			private readonly MethodInfo _getEnumerator;
			private LocalBuilder _enumerator;
			private Label _next;
			private Label _top;
			#endregion

			#region Constructors
			public ForEachLoop(ILGenerator generator, PropertyInfo property)
			{
				_generator = generator;
				_getEnumerator = property.PropertyType.GetMethod("GetEnumerator");
				_getField = property.GetGetMethod();
			}
			#endregion

			#region Instance Properties
			public bool EnumeratorCreated
			{
				get
				{
					return (_enumerator != null);
				}
			}
			#endregion

			#region Instance Methods
			private Type GetEnumeratorType()
			{
				if (typeof(IEnumerator<>).IsAssignableFrom(_getEnumerator.ReturnType))
					return _getEnumerator.ReturnType;

				return typeof(IEnumerator);
			}

			private void Begin()
			{
				_next = _generator.DefineLabel();
				_top = _generator.DefineLabel();

				if (EnumeratorCreated)
					Reset();
				else
					CreateEnumerator();

				_generator.Emit(OpCodes.Br, _next);
				_generator.MarkLabel(_top);
			}

			private void End()
			{
				_generator.MarkLabel(_next);
				_generator.Emit(OpCodes.Ldloc, _enumerator);
				_generator.Emit(OpCodes.Callvirt, typeof(IEnumerator).GetMethod("MoveNext"));
				_generator.Emit(OpCodes.Brtrue_S, _top);
			}

			private void Reset()
			{
				_generator.Emit(OpCodes.Ldloc, _enumerator);
				_generator.Emit(OpCodes.Callvirt, typeof(IEnumerator).GetMethod("Reset"));
			}

			private void CreateEnumerator()
			{
				_enumerator = _generator.DeclareLocal(GetEnumeratorType());
				_generator.Emit(OpCodes.Ldloc_0);
				_generator.Emit(OpCodes.Call, _getField);
				_generator.Emit(OpCodes.Call, _getEnumerator);

				if (_getEnumerator.ReturnType.IsValueType)
					_generator.Emit(OpCodes.Box, _getEnumerator.ReturnType);

				_generator.Emit(OpCodes.Stloc, _enumerator);
			}

			public void Create(Action<ILGenerator> body)
			{
				Begin();
				body(_generator);

				End();
			}

			public void LoadCurrentAs(Type type)
			{
				_generator.Emit(OpCodes.Ldloc, _enumerator.LocalIndex);
				_generator.Emit(OpCodes.Callvirt, GetEnumeratorType()
					.GetProperty("Current").GetGetMethod());

				if (GetEnumeratorType() == typeof(IEnumerator))
					_generator.Emit(OpCodes.Unbox_Any, type);
			}
			#endregion
		}
		#endregion

		#region Instance Fields
		private MethodInfo _addMethod;
		#endregion

		#region Constructors
		protected RepeatedFieldIO(PropertyInfo property, MethodInfo addItem)
			: base(property)
		{
			_addMethod = addItem;
		}
		#endregion

		#region Instance Properties
		protected MethodInfo AddMethod
		{
			get
			{
				return _addMethod;
			}
		}

		public override Type FieldType
		{
			get
			{
				return _addMethod.GetParameters()[0].ParameterType;
			}
		}
		#endregion

		#region Static Methods
		protected static bool TryCreate(PropertyInfo property, 
			Func<MethodInfo, IFieldIO> creator, out IFieldIO io)
		{
			MethodInfo add = property.PropertyType.GetMethod("Add");
			MethodInfo getEnumerator = property.PropertyType.GetMethod("GetEnumerator");
			if (add == null || getEnumerator == null)
			{
				io = null;
				return false;
			}
			io = creator(add);
			return true;
		}

		public static bool TryCreate(PropertyInfo property, out IFieldIO io)
		{
			return TryCreate(property, add => new RepeatedFieldIO(property, add), out io);
		}
		#endregion

		#region Instance Methods
		protected virtual void AppendMessageHeaderCore(ILGenerator generator,
			ForEachLoop loop, MessageField field)
		{
		}

		public override void AppendRead(ILGenerator generator, MessageField field)
		{
			generator.Emit(OpCodes.Ldloc_0);
			generator.Emit(OpCodes.Call, Property.GetGetMethod());

			generator.Emit(OpCodes.Ldarg_1);
			field.AppendReadField(generator);

			generator.Emit(OpCodes.Callvirt, _addMethod);
		}

		public override void AppendWrite(ILGenerator generator, MessageField field)
		{
			ForEachLoop loop = new ForEachLoop(generator, Property);
			AppendMessageHeaderCore(generator, loop, field);

			loop.Create(body => {
				field.AppendHeader(generator);
				loop.LoadCurrentAs(FieldType);
				field.AppendWriteField(generator);
				body.Emit(OpCodes.Pop);
			});
		}
		#endregion
	}
}
