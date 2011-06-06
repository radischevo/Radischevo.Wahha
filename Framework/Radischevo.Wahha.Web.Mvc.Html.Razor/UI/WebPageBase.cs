using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Radischevo.Wahha.Web.Mvc.Html.Razor
{
	public abstract class WebPageBase : WebPageRenderingBase
	{
		private HashSet<string> _sections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private bool _renderedBody = false;
		private Action<TextWriter> _body;

		// TODO(elipton): Figure out if we still need these two writers
		private TextWriter _tempWriter;
		private TextWriter _currentWriter;

		public override string Layout
		{
			get;
			set;
		}

		public TextWriter Output
		{
			get
			{
				return OutputStack.Peek();
			}
		}

		public Stack<TextWriter> OutputStack
		{
			get
			{
				return PageContext.OutputStack;
			}
		}

		public override IDictionary<object, dynamic> PageData
		{
			get
			{
				return PageContext.PageData;
			}
		}

		public override dynamic Page
		{
			get
			{
				if (_dynamicPageData == null)
				{
					_dynamicPageData = new DynamicPageDataDictionary<dynamic>((PageDataDictionary<dynamic>)PageData);
				}
				return _dynamicPageData;
			}
		}

		// Retrieves the sections defined in the calling page. If this is null, that means
		// this page has been requested directly.
		private Dictionary<string, SectionWriter> PreviousSectionWriters
		{
			get
			{
				var top = SectionWritersStack.Pop();
				var previous = SectionWritersStack.Count > 0 ? SectionWritersStack.Peek() : null;
				SectionWritersStack.Push(top);
				return previous;
			}
		}

		// Retrieves the current Dictionary of sectionWriters on the stack without poping it.
		// There should be at least one on the stack which is added when the Render(ViewData,TextWriter)
		// is called.
		private Dictionary<string, SectionWriter> SectionWriters
		{
			get
			{
				return SectionWritersStack.Peek();
			}
		}

		private Stack<Dictionary<string, SectionWriter>> SectionWritersStack
		{
			get
			{
				return PageContext.SectionWritersStack;
			}
		}

		protected virtual void ConfigurePage(WebPageBase parentPage)
		{
		}

		public static WebPageBase CreateInstanceFromVirtualPath(string virtualPath)
		{
			return CreateInstanceFromVirtualPath(virtualPath, VirtualPathFactoryManager.Instance);
		}

		internal static WebPageBase CreateInstanceFromVirtualPath(string virtualPath, Func<string, Type, object> createInstanceMethod)
		{
			return CreateInstanceFromVirtualPath(virtualPath, VirtualPathFactoryManager.CreateFromLambda(virtualPath, createInstanceMethod));
		}

		internal static WebPageBase CreateInstanceFromVirtualPath(string virtualPath, VirtualPathFactoryManager virtualPathFactoryManager)
		{
			// Get the compiled object (through the VPP)
			try
			{
				WebPageBase webPage = virtualPathFactoryManager.CreateInstance<WebPageBase>(virtualPath);

				// Give it its virtual path
				webPage.VirtualPath = virtualPath;

				return webPage;
			}
			catch (HttpException e)
			{
				Util.ThrowIfUnsupportedExtension(virtualPath, e);
				throw;
			}
		}

		internal virtual WebPageBase CreatePageFromVirtualPath(string path)
		{
			return CreateInstanceFromVirtualPath(path);
		}

		private WebPageContext CreatePageContextFromParameters(bool isLayoutPage, params object[] data)
		{
			object first = null;
			if (data != null && data.Length > 0)
			{
				first = data[0];
			}

			var pageData = PageDataDictionary<dynamic>.CreatePageDataFromParameters(PageData, data);

			return Util.CreateNestedPageContext(PageContext, pageData, first, isLayoutPage);
		}

		public void DefineSection(string name, SectionWriter action)
		{
			if (SectionWriters.ContainsKey(name))
			{
				throw new HttpException(String.Format(CultureInfo.InvariantCulture, WebPageResources.WebPage_SectionAleadyDefined, name));
			}
			SectionWriters[name] = action;
		}

		internal void EnsurePageCanBeRequestedDirectly(string methodName)
		{
			if (PreviousSectionWriters == null)
			{
				throw new HttpException(String.Format(CultureInfo.CurrentCulture, WebPageResources.WebPage_CannotRequestDirectly, VirtualPath, methodName));
			}
		}

		public void ExecutePageHierarchy(WebPageContext pageContext, TextWriter writer)
		{
			ExecutePageHierarchy(pageContext, writer, startPage: null);
		}

		// This method is only used by WebPageBase to allow passing in the view context and writer.
		public void ExecutePageHierarchy(WebPageContext pageContext, TextWriter writer, WebPageRenderingBase startPage)
		{
			PushContext(pageContext, writer);

			if (startPage != null)
			{
				if (startPage != this)
				{
					var startPageContext = Util.CreateNestedPageContext<object>(parentContext: pageContext, pageData: null, model: null, isLayoutPage: false);
					startPageContext.Page = startPage;
					startPage.PageContext = startPageContext;
				}
				startPage.ExecutePageHierarchy();
			}
			else
			{
				ExecutePageHierarchy();
			}
			PopContext();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We really don't care if SourceHeader fails, and we don't want it to fail any real requests ever")]
		public override void ExecutePageHierarchy()
		{
			// Unlike InitPages, for a WebPage there is no hierarchy - it is always
			// the last file to execute in the chain. There can still be layout pages
			// and partial pages, but they are never part of the hierarchy.

			// (add server header for falcon debugging)
			// call to MapPath() is expensive. If we are not emiting source files to header, 
			// don't bother to populate the SourceFiles collection. This saves perf significantly.
			if (WebPageHttpHandler.ShouldGenerateSourceHeader(Context))
			{
				try
				{
					string vp = this.VirtualPath;
					if (vp != null)
					{
						string path = Context.Request.MapPath(vp);
						if (!path.IsEmpty())
						{
							PageContext.SourceFiles.Add(path);
						}
					}
				}
				catch
				{
					// we really don't care if this ever fails, so we swallow all exceptions
				}
			}


			TemplateStack.Push(Context, this);
			try
			{
				// Execute the developer-written code of the WebPage
				Execute();
			}
			finally
			{
				TemplateStack.Pop(Context);
			}
		}

		protected virtual void InitializePage()
		{
		}

		public bool IsSectionDefined(string name)
		{
			EnsurePageCanBeRequestedDirectly("IsSectionDefined");
			return PreviousSectionWriters.ContainsKey(name);
		}

		public void PopContext()
		{
			string renderedContent = _tempWriter.ToString();
			OutputStack.Pop();

			if (!String.IsNullOrEmpty(Layout))
			{
				string layoutPagePath = NormalizeLayoutPagePath(Layout);
				Util.EnsureValidPageType(this, layoutPagePath);
				// If a layout file was specified, render it passing our page content
				OutputStack.Push(_currentWriter);
				RenderSurrounding(
					layoutPagePath,
					w => w.Write(renderedContent));
				OutputStack.Pop();
			}
			else
			{
				// Otherwise, just render the page
				_currentWriter.Write(renderedContent);
			}

			VerifyRenderedBodyOrSections();
			SectionWritersStack.Pop();
		}

		public void PushContext(WebPageContext pageContext, TextWriter writer)
		{
			_currentWriter = writer;
			PageContext = pageContext;
			pageContext.Page = this;

			InitializePage();

			// Create a temporary writer
			_tempWriter = new StringWriter(CultureInfo.InvariantCulture);

			// Render the page into it
			OutputStack.Push(_tempWriter);
			SectionWritersStack.Push(new Dictionary<string, SectionWriter>(StringComparer.OrdinalIgnoreCase));

			// If the body is defined in the ViewData, remove it and store it on the instance
			// so that it won't affect rendering of partial pages when they call VerifyRenderedBodyOrSections
			if (PageContext.BodyAction != null)
			{
				_body = PageContext.BodyAction;
				PageContext.BodyAction = null;
			}
		}

		public HelperResult RenderBody()
		{
			EnsurePageCanBeRequestedDirectly("RenderBody");

			if (_renderedBody)
			{
				throw new HttpException(WebPageResources.WebPage_RenderBodyAlreadyCalled);
			}
			_renderedBody = true;

			// _body should have previously been set in Render(ViewContext,TextWriter) if it
			// was available in the ViewData.
			if (_body != null)
			{
				return new HelperResult(tw => _body(tw));
			}
			else
			{
				throw new HttpException(String.Format(CultureInfo.CurrentCulture, WebPageResources.WebPage_CannotRequestDirectly, VirtualPath, "RenderBody"));
			}
		}

		public override HelperResult RenderPage(string path, params object[] data)
		{
			return RenderPageCore(path, isLayoutPage: false, data: data);
		}

		private HelperResult RenderPageCore(string path, bool isLayoutPage, object[] data)
		{
			if (String.IsNullOrEmpty(path))
			{
				throw ExceptionHelper.CreateArgumentNullOrEmptyException("path");
			}

			return new HelperResult(writer => {
				path = NormalizePath(path);
				Util.EnsureValidPageType(this, path);

				WebPageBase subPage = CreatePageFromVirtualPath(path);
				var pageContext = CreatePageContextFromParameters(isLayoutPage, data);

				subPage.ConfigurePage(this);
				subPage.ExecutePageHierarchy(pageContext, writer);
			});
		}

		public HelperResult RenderSection(string name)
		{
			return RenderSection(name, required: true);
		}

		public HelperResult RenderSection(string name, bool required)
		{
			EnsurePageCanBeRequestedDirectly("RenderSection");

			if (PreviousSectionWriters.ContainsKey(name))
			{
				var result = new HelperResult(tw => {
					if (_sections.Contains(name))
					{
						throw new HttpException(String.Format(CultureInfo.InvariantCulture, WebPageResources.WebPage_SectionAleadyRendered, name));
					}
					var body = PreviousSectionWriters[name];
					// Since the body can also call RenderSection, we need to temporarily remove
					// the current sections from the stack.
					var top = SectionWritersStack.Pop();

					bool pushed = false;
					try
					{
						if (Output != tw)
						{
							OutputStack.Push(tw);
							pushed = true;
						}

						body();
					}
					finally
					{
						if (pushed)
						{
							OutputStack.Pop();
						}
					}
					SectionWritersStack.Push(top);
					_sections.Add(name);
				});
				return result;
			}
			else if (required)
			{
				// If the section is not found, and it is not optional, throw an error.
				throw new HttpException(String.Format(CultureInfo.InvariantCulture, WebPageResources.WebPage_SectionNotDefined, name));
			}
			else
			{
				// If the section is optional and not found, then don't do anything.
				return null;
			}
		}

		private void RenderSurrounding(string partialViewName, Action<TextWriter> body)
		{
			// Save the previous body action and set ours instead.
			// This value will be retrieved by the sub-page being rendered when it runs
			// Render(ViewData, TextWriter).
			var priorValue = PageContext.BodyAction;
			PageContext.BodyAction = body;

			// Render the layout file
			Write(RenderPageCore(partialViewName, isLayoutPage: true, data: new object[0]));

			// Restore the state
			PageContext.BodyAction = priorValue;
		}

		// Verifies that RenderBody is called, or that RenderSection is called for all sections
		private void VerifyRenderedBodyOrSections()
		{
			// The _body will be set within a layout page because PageContext.BodyAction was set by RenderSurrounding, 
			// which is only called in the case of rendering layout pages.
			// Using RenderPage will not result in a _body being set in a partial page, thus the following checks for
			// sections should not apply when RenderPage is called.
			// Dev10 bug 928341 
			if (_body != null)
			{
				if (SectionWritersStack.Count > 1 && PreviousSectionWriters != null && PreviousSectionWriters.Count > 0)
				{
					// There are sections defined. Check that all sections have been rendered.
					StringBuilder sectionsNotRendered = new StringBuilder();
					foreach (var name in PreviousSectionWriters.Keys)
					{
						if (!_sections.Contains(name))
						{
							if (sectionsNotRendered.Length > 0)
							{
								sectionsNotRendered.Append("; ");
							}
							sectionsNotRendered.Append(name);
						}
					}
					if (sectionsNotRendered.Length > 0)
					{
						throw new HttpException(String.Format(CultureInfo.CurrentCulture, WebPageResources.WebPage_SectionsNotRendered, VirtualPath, sectionsNotRendered.ToString()));
					}
				}
				else if (!_renderedBody)
				{
					// There are no sections defined, but RenderBody was NOT called.
					// If a body was defined, then RenderBody should have been called.
					throw new HttpException(String.Format(CultureInfo.CurrentCulture, WebPageResources.WebPage_RenderBodyNotCalled, VirtualPath));
				}
			}
		}

		public override void Write(HelperResult result)
		{
			WriteTo(Output, result);
		}

		public override void Write(object value)
		{
			WriteTo(Output, value);
		}

		public override void WriteLiteral(object value)
		{
			Output.Write(value);
		}
	}
}
