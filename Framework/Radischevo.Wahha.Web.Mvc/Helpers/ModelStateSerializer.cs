using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Mvc
{
	public class ModelStateSerializer
	{
		#region Nested Types
		private sealed class TokenPersister : PageStatePersister
		{
			#region Constructors
			private TokenPersister(Page page)
				: base(page)
			{
			}
			#endregion

			#region Static Methods
			public static Func<IStateFormatter> CreateFormatterGenerator(bool encrypt, bool sign)
			{
				TextWriter writer = TextWriter.Null;

				HttpResponse response = new HttpResponse(writer);
				HttpRequest request = new HttpRequest("wahha.aspx", HttpContext.Current.Request.Url.ToString(),
					"__EVENTTARGET=true" + ((encrypt) ? "&__VIEWSTATEENCRYPTED=true" : null));
				HttpContext context = new HttpContext(request, response);

				Page page = new Page() {
					EnableViewStateMac = sign,
					ViewStateEncryptionMode = (encrypt)
						? ViewStateEncryptionMode.Always
						: ViewStateEncryptionMode.Never
				};
				page.ProcessRequest(context);
				return () => new TokenPersister(page).StateFormatter;
			}
			#endregion

			#region Instance Methods
			public override void Load()
			{
			}

			public override void Save()
			{
			}
			#endregion
		}
		#endregion

		#region Constants
		private const SerializationMode _defaultMode = SerializationMode.Plaintext;
		#endregion

		#region Static Fields
		private static readonly Dictionary<SerializationMode, Func<IStateFormatter>> _factories;
		#endregion

		#region Constructors
		static ModelStateSerializer()
		{
			_factories = new Dictionary<SerializationMode, Func<IStateFormatter>>();
			_factories.Add(SerializationMode.Plaintext, () => new ObjectStateFormatter());
            _factories.Add(SerializationMode.Encrypted, GetFormatterGenerator(true, false));
            _factories.Add(SerializationMode.Signed, GetFormatterGenerator(false, true));
			_factories.Add(SerializationMode.EncryptedAndSigned, GetFormatterGenerator(true, true));
		}

		public ModelStateSerializer()
		{
		}
		#endregion

		#region Static Methods
		private static IStateFormatter GetFormatter(SerializationMode mode)
		{
			Func<IStateFormatter> formatterFactory;
			if (!_factories.TryGetValue(mode, out formatterFactory))
				throw Error.InvalidModelStateSerializationMode(mode);

			return formatterFactory();
		}

		private static Func<IStateFormatter> GetFormatterGenerator(bool encrypt, bool sign)
		{
			Link<Func<IStateFormatter>> factory = new Link<Func<IStateFormatter>>(
				() => TokenPersister.CreateFormatterGenerator(encrypt, sign)
			);
			return () => {
				return factory.Value();
			};
		}
		#endregion

		#region Instance Methods
		public string Serialize(object state)
		{
			return Serialize(state, _defaultMode);
		}

		public virtual string Serialize(object state, SerializationMode mode)
		{
			IStateFormatter formatter = GetFormatter(mode);
			return formatter.Serialize(state);
		}

		public object Deserialize(string value)
		{
			return Deserialize(value, _defaultMode);
		}

		public virtual object Deserialize(string value, SerializationMode mode)
		{
			Precondition.Defined(value, () => Error.ArgumentNull("value"));
			IStateFormatter formatter = GetFormatter(mode);
			try
			{
				return formatter.Deserialize(value);
			}
			catch (Exception ex)
			{
				throw Error.CouldNotDeserializeModelState(ex);
			}
		}
		#endregion
	}
}
