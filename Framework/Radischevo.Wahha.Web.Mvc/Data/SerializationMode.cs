using System;

namespace Radischevo.Wahha.Web.Mvc
{
	[Flags]
	public enum SerializationMode
	{
		Plaintext = 0,
		Encrypted = 1 << 0,
		Signed = 1 << 1,
		EncryptedAndSigned = Encrypted | Signed
	}
}
