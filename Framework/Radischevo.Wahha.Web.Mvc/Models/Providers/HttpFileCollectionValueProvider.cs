using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web.Mvc
{
	public sealed class HttpFileCollectionValueProvider : ValueProviderBase
	{
		#region Nested Types
		private sealed class HttpFileValueProviderResult : ValueProviderResult
		{
			#region Constructors
			public HttpFileValueProviderResult(HttpPostedFileBase[] files)
				: base(files, CultureInfo.InvariantCulture)
			{
			}
			#endregion

			#region Instance Properties
			public new HttpPostedFileBase[] Value
			{
				get
				{
					return (HttpPostedFileBase[])base.Value;
				}
			}
			#endregion

			#region Static Methods
			private static bool ValidateOutputType(Type type)
			{
				if (type == typeof(object))
					return true;

				if (type == typeof(HttpPostedFileBase))
					return true;

				if (type == typeof(IEnumerable<HttpPostedFileBase>))
					return true;

				return false;
			}

			private static TValue Cast<TValue>(object value)
			{
				return (TValue)value;
			}
			#endregion

			#region Instance Methods
			public override TValue GetValue<TValue>(TValue defaultValue)
			{
				Type type = typeof(TValue);
				Precondition.Require(ValidateOutputType(type),
					() => Error.HttpPostedFileSetTypeLimitations(type));

				if (type == typeof(HttpPostedFileBase))
					return Cast<TValue>(Value.FirstOrDefault());

				return Cast<TValue>(Value);
			}
			#endregion
		}
		#endregion

		#region Static Fields
		private static readonly Dictionary<string, HttpPostedFileBase[]> _empty = 
			new Dictionary<string, HttpPostedFileBase[]>();
		#endregion

		#region Instance Fields
		private HashSet<string> _prefixes;
		private Dictionary<string, HttpPostedFileBase[]> _values;
		#endregion

		#region Constructors
		public HttpFileCollectionValueProvider(ControllerContext context)
			: base(CultureInfo.InvariantCulture)
		{
			Precondition.Require(context, () => Error.ArgumentNull("context"));

			_prefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			_values = GetDataSource(context);
			Initialize(context);
		}
		#endregion

		#region Instance Properties
		public override IEnumerable<string> Keys
		{
			get
			{
				return _values.Keys;
			}
		}
		#endregion

		#region Static Methods
		private static HttpPostedFileBase ChooseFileOrNull(HttpPostedFileBase file)
		{
			if (file == null)
				return null;
			
			if (file.ContentLength == 0 && 
				String.IsNullOrEmpty(file.FileName))
				return null;

			return file;
		}

		private static Dictionary<string, HttpPostedFileBase[]> GetDataSource(ControllerContext context)
		{
			HttpFileCollectionBase files = context.Context.Request.Files;
			if (files.Count == 0)
				return _empty;

			List<KeyValuePair<string, HttpPostedFileBase>> mapping = 
				new List<KeyValuePair<string, HttpPostedFileBase>>();

			string[] keys = files.AllKeys.ToArray();
			for (int i = 0; i < files.Count; i++)
			{
				string key = keys[i];
				if (key != null)
					mapping.Add(new KeyValuePair<string, HttpPostedFileBase>(key, ChooseFileOrNull(files[i])));
			}
			return mapping.GroupBy(e => e.Key, e => e.Value, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(g => g.Key, g => g.ToArray(), StringComparer.OrdinalIgnoreCase);
		}
		#endregion

		#region Instance Methods
		private void Initialize(ControllerContext context)
		{
			foreach (string key in _values.Keys)
				_prefixes.UnionWith(GetPrefixes(key));
		}

		public override bool Contains(string prefix)
		{
			if (String.IsNullOrEmpty(prefix))
				return _values.Any();

			return _prefixes.Contains(prefix);
		}

		public override ValueProviderResult GetValue(string key)
		{
			HttpPostedFileBase[] files;
			if (_values.TryGetValue(key, out files))
				return new HttpFileValueProviderResult(files);

			return null;
		}
		#endregion
	}

	public sealed class HttpFileCollectionValueProviderFactory : IValueProviderFactory
	{
		#region Constructors
		public HttpFileCollectionValueProviderFactory()
			: base()
		{
		}
		#endregion

		#region Instance Methods
		public IValueProvider Create(ControllerContext context)
		{
			return new HttpFileCollectionValueProvider(context);
		}
		#endregion
	}
}
