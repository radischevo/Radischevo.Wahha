using System;
using System.IO;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Scripting.Serialization
{
	public class WeakTypeResolver : JavaScriptTypeResolver
	{
		#region Constructors
		public WeakTypeResolver()
		{
		}
		#endregion

		#region Instance Methods
		public override Type ResolveType(string typeId)
		{
			return Type.GetType(typeId, false, true);
		}

		public override string ResolveTypeId(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return String.Format("{0},{1}", type.FullName,
				Path.GetFileNameWithoutExtension(type.Module.Name));
		}
		#endregion
	}
}
