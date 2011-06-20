using System;

namespace Radischevo.Wahha.Web.Routing
{
	public enum SecureConnectionOption : byte
	{
		Inherit = 0,
		Never = 1,
		Preferred = 2,
		Always = 3
	}
}
