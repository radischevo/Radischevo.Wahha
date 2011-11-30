using System;
using System.Web;
using System.Web.SessionState;

namespace Radischevo.Wahha.Web.Abstractions
{
	public static class HttpSessionStateExtensions
	{
		#region Extension Methods
		public static void UpdateSessionId (this HttpSessionState session)
		{
			if (session == null)
				return;

			try
			{
				#pragma warning disable 0219
				string id = session.SessionID;
				#pragma warning restore 0219
			}
			catch (HttpException)
			{
			}
		}

		public static void UpdateSessionId (this HttpSessionStateBase session)
		{
			if (session == null)
				return;

			try
			{
				#pragma warning disable 0219
				string id = session.SessionID;
				#pragma warning restore 0219
			}
			catch (HttpException)
			{
			}
		}
		#endregion
	}
}
