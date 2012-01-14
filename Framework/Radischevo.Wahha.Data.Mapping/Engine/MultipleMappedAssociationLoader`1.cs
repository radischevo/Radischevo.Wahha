using System;
using System.Collections.Generic;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Data.Mapping
{
	[Serializable]
	internal sealed class MultipleMappedAssociationLoader<TAssociation> : IAssociationLoader<IEnumerable<TAssociation>>
	{
		#region Nested Types
		private sealed class Operation : MappedDbSelectOperation<TAssociation> 
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
		public MultipleMappedAssociationLoader (DbCommandDescriptor command)
		{
			Precondition.Require(command, () => Error.ArgumentNull("command"));
			_command = command;
		}
		#endregion
		
		#region Instance Methods
		public IEnumerable<TAssociation> Load () 
		{
			DbOperation<IEnumerable<TAssociation>> operation = new Operation(_command);
			using (DbOperationScope scope = new DbOperationScope())
			{
				return scope.Execute(operation);
			}
		}
		#endregion
	}
}

