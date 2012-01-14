using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	[Serializable]
	internal sealed class ScalarMappedAssociationLoader<TAssociation> : IAssociationLoader<TAssociation>
	{
		#region Nested Types
		private sealed class Operation : DbScalarOperation<object> 
		{
			#region Instance Fields
			private DbCommandDescriptor _command;
			#endregion
			
			#region Constructors
			public Operation (DbCommandDescriptor command)
				: base()
			{
				_command = command;
			}
			#endregion
			
			#region Instance Methods
			protected override DbCommandDescriptor CreateCommand ()
			{
				return _command;
			}
			#endregion
		}
		#endregion
		
		#region Instance Fields
		private DbCommandDescriptor _command;
		private IDbColumnBinder _binder;
		#endregion
		
		#region Constructors
		public ScalarMappedAssociationLoader (
			DbCommandDescriptor command, IDbColumnBinder binder)
		{
			Precondition.Require(command, () => Error.ArgumentNull("command"));
			
			_command = command;
			_binder = binder;
		}
		#endregion
		
		#region Instance Methods
		public TAssociation Load () 
		{
			DbScalarOperation<object> operation = new Operation(_command);
			using (DbOperationScope scope = new DbOperationScope())
			{
				object result = scope.Execute(operation);
				if (_binder == null)
					return (TAssociation)result;
				
				return (TAssociation)_binder.ToPropertyValue(result);
			}
		}
		#endregion
	}
}

