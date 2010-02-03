using System;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Scripting.Serialization;

namespace Radischevo.Wahha.Web.Scripting.Templates
{
	public class JavaScriptTemplateHandler : IHttpHandler
	{
		#region Nested Types
		[Serializable]
		public class TemplateInfo
		{
			#region Instance Fields
			private string _virtualPath;
			private string _bufferVariableName;
			private HttpCacheability _cacheability;
			private int _cacheDuration;
			private bool _debug;
			#endregion

			#region Constructors
			public TemplateInfo()
			{
				_bufferVariableName = "$c";
				_cacheability = HttpCacheability.Private;
				_cacheDuration = 10;				
			}
			#endregion

			#region Instance Properties
			public string VirtualPath
			{
				get
				{
					return _virtualPath;
				}
				set
				{
					_virtualPath = value;
				}
			}

			public HttpCacheability Cacheability
			{
				get
				{
					return _cacheability;
				}
				set
				{
					_cacheability = value;
				}
			}

			public int CacheDuration
			{
				get
				{
					return _cacheDuration;
				}
				set
				{
					_cacheDuration = value;
				}
			}

			public string BufferVariableName
			{
				get
				{
					return _bufferVariableName;
				}
				set
				{
					_bufferVariableName = value;
				}
			}

			public bool Debug
			{
				get
				{
					return _debug;
				}
				set
				{
					_debug = value;
				}
			}
			#endregion
		}
		#endregion

		#region Constants
		private const string CacheKeyFormat = "wahha.scripting.template.cache";
		#endregion

		#region Constructors
		public JavaScriptTemplateHandler()
		{
		}
		#endregion

		#region Instance Properties
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Static Methods
		public static string GenerateUrl(string handlerUrl, string virtualPath)
		{
			return GenerateUrl(handlerUrl, virtualPath, false,
				null, HttpCacheability.Private, 10);
		}

		public static string GenerateUrl(string handlerUrl, string virtualPath,
			bool debug)
		{
			return GenerateUrl(handlerUrl, virtualPath, debug,
				null, HttpCacheability.Private, 10);
		}

		public static string GenerateUrl(string handlerUrl, string virtualPath,
			bool debug, string codeBufferVariableName)
		{
			return GenerateUrl(handlerUrl, virtualPath, debug, 
				codeBufferVariableName, HttpCacheability.Private, 10);
		}

		public static string GenerateUrl(string handlerUrl, string virtualPath,
			bool debug, string codeBufferVariableName, HttpCacheability cacheability, 
			int cacheDuration)
		{
			Precondition.Require(!String.IsNullOrEmpty(virtualPath), 
				Error.ArgumentNull("virtualPath"));

			TemplateInfo info = new TemplateInfo();

			info.VirtualPath = virtualPath;
			info.BufferVariableName = codeBufferVariableName;
			info.Cacheability = cacheability;
			info.CacheDuration = cacheDuration;
			info.Debug = debug;

			return GenerateUrl(handlerUrl, info);
		}

		private static string GenerateUrl(string handlerUrl, TemplateInfo info)
		{
			Precondition.Require(!String.IsNullOrEmpty(handlerUrl), 
				Error.ArgumentNull("handlerUrl"));

			string serializedData;
			string parameter = (handlerUrl.IndexOf('?') > -1) ? "&v=" : "?v=";

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			serializedData = Convert.ToBase64String(
				new UTF8Encoding(false).GetBytes(serializer.Serialize(info)));

			return String.Concat(handlerUrl, parameter, serializedData);
		}
		#endregion

		#region Instance Methods
		protected virtual string CreateCacheKey(TemplateInfo info)
		{
			StringBuilder sb = new StringBuilder();
			MD5 md5 = MD5.Create();
			Encoding encoding = Encoding.UTF8;

			sb.Append(CacheKeyFormat).Append("::")
				.Append(info.VirtualPath).Append("=>{")
				.Append("debug=").Append(info.Debug)
				.Append("buffer=").Append(info.BufferVariableName)
				.Append("}");

			return Converter.ToBase16String(
				md5.ComputeHash(encoding.GetBytes(sb.ToString())));
		}

		protected virtual void SetHttpCachePolicy(HttpCachePolicyBase policy, TemplateInfo info)
		{
			policy.SetCacheability(info.Cacheability);
			policy.SetExpires(DateTime.Now.AddMinutes(info.CacheDuration));
			policy.SetMaxAge(TimeSpan.FromMinutes(info.CacheDuration));
			policy.SetValidUntilExpires(true);
			policy.SetETagFromFileDependencies();
			policy.SetLastModifiedFromFileDependencies();
		}

		protected virtual CompiledTemplate CompileTemplate(TemplateInfo info)
		{
			TemplateParser parser = new TemplateParser();
			ParsedTemplate template = parser.ParseFile(info.VirtualPath);

			JavaScriptTemplateCompiler compiler =
				new JavaScriptTemplateCompiler(info.Debug);

			if (!String.IsNullOrEmpty(info.BufferVariableName))
				compiler.CodeBufferVariableName = info.BufferVariableName;

			return compiler.Build(template);
		}

		private bool ModifiedSince(HttpRequestBase request, string physicalPath)
		{
			string headerValue = request.Headers.GetValue<string>("If-Modified-Since");
			DateTime modifiedSince;

			if (DateTime.TryParse(headerValue, CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal, out modifiedSince))
				return modifiedSince <= File.GetLastWriteTimeUtc(physicalPath);

			return true;
		}

		private void ProcessRequest(HttpContext context)
		{
			Precondition.Require(context, Error.ArgumentNull("context"));
			ProcessRequestInternal(new HttpContextWrapper(context));
		}

		private void ProcessRequestInternal(HttpContextBase context)
		{
			string encodedInfo = context.Request.Parameters.GetValue<string>("v");
			byte[] encodedBytes = Convert.FromBase64String(encodedInfo);

			JavaScriptSerializer serializer = new JavaScriptSerializer();
			TemplateInfo info = serializer.Deserialize<TemplateInfo>(
				new UTF8Encoding(false).GetString(encodedBytes));

			string physicalPath = context.Server.MapPath(info.VirtualPath);
			context.Response.ContentType = "text/javascript";
			context.Response.AddFileDependency(physicalPath);
			SetHttpCachePolicy(context.Response.Cache, info);

			if (ModifiedSince(context.Request, physicalPath))
			{
				context.Response.StatusCode = 200;
				RenderTemplate(context, info, physicalPath);
			}
			else
			{
				context.Response.StatusCode = 304;
			}
		}

		private void RenderTemplate(HttpContextBase context, 
			TemplateInfo info, string physicalPath)
		{
			Precondition.Require(context, Error.ArgumentNull("context"));
			Precondition.Require(info, Error.ArgumentNull("info"));

			string cacheKey = CreateCacheKey(info);

			CompiledTemplate template = (CompiledTemplate)context.Cache.Get(cacheKey);
			if (template == null)
			{
				template = CompileTemplate(info);
				context.Cache.Insert(cacheKey, template, new CacheDependency(physicalPath),
					Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(10));
			}
			context.Response.Output.Write(template.Source);
		}
		#endregion

		#region IHttpHandler Members
		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			ProcessRequest(context);
		}
		#endregion
	}
}
