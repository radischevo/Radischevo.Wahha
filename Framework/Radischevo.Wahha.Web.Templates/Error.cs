using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radischevo.Wahha.Web.Templates
{
	internal static class Error
	{
		internal static Exception ArgumentNull(string name)
		{
			return new ArgumentNullException(name);
		}
	}
}
