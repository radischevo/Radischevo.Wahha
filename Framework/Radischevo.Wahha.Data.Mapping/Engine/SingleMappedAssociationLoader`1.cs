using System;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	[Serializable]
	internal sealed class SingleMappedAssociationLoader<TAssociation> : IAssociationLoader<TAssociation>
	{
		#region Nested Types
		private sealed class Operation : MappedDbSingleOperation<TAssociation> 
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
		#endregion
		
		#region Constructors
		public SingleMappedAssociationLoader (DbCommandDescriptor command)
		{
			Precondition.Require(command, () => Error.ArgumentNull("command"));
			_command = command;
		}
		#endregion
		
		#region Instance Methods
		public TAssociation Load () 
		{
			DbOperation<TAssociation> operation = new Operation(_command);
			using (DbOperationScope scope = new DbOperationScope())
			{
				return scope.Execute(operation);
			}
		}
		#endregion
	}
}

