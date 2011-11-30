using System;
using System.Web;
using System.Web.SessionState;

using Radischevo.Wahha.Web.Abstractions;

namespace Radischevo.Wahha.Web
{
	public static class HttpSessionStateExtensions
	{
		#region Extension Methods
		public static void UpdateSessionId(this HttpSessionStateBase session)
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

		/// <summary>
		/// Creates a wrapper over the specified 
		/// <see cref="System.Web.HttpSessionStateBase"/>
		/// making it act as <see cref="Radischevo.Wahha.Web.IHttpValueSet"/>.
		/// </summary>
		/// <param name="session">The session state to wrap over.</param>
		public static IHttpValueSet AsValueSet(this HttpSessionStateBase session)
		{
			return new HttpSessionStateSet(session);
		}
		#endregion
	}
}
