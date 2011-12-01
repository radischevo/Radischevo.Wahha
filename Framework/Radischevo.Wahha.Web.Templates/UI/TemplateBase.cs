using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using Radischevo.Wahha.Core;
using System.Globalization;

namespace Radischevo.Wahha.Web.Templates
{
	public abstract class TemplateBase : TemplateRenderingBase
	{
		// Keep track of which sections RenderSection has already been called on
		private HashSet<string> _renderedSections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		// Keep track of whether RenderBody has been called
		private bool _renderedBody = false;
		// Action for rendering the body within a layout page
		private Action<TextWriter> _body;

		// TODO(elipton): Figure out if we still need these two writers
		private TextWriter _tempWriter;
		private TextWriter _currentWriter;

		public Stack<TextWriter> OutputStack
		{
			get
			{
				return TemplateContext.Output;
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
				return TemplateContext.SectionWriters;
			}
		}

		protected virtual void Configure(TemplateBase parentPage)
		{
		}

		internal static TemplateContext CreateNestedPageContext<TModel>(TemplateContext parentContext, 
			ValueDictionary data, TModel model, bool isLayoutPage)
		{
			var nestedContext = new TemplateContext(parentContext.Context, null, model);
			nestedContext.Output = parentContext.Output;
			nestedContext.Data = data;

			if (isLayoutPage)
			{
				nestedContext.BodyRenderer = parentContext.BodyRenderer;
				nestedContext.SectionWriters = parentContext.SectionWriters;
			}
			return nestedContext;
		}

		public static TemplateBase CreateInstanceFromVirtualPath(string virtualPath)
		{
			return CreateInstanceFromVirtualPath(virtualPath, VirtualPathFactoryManager.Instance);
		}

		internal static TemplateBase CreateInstanceFromVirtualPath(string virtualPath, VirtualPathFactoryManager manager)
		{
			TemplateBase template = manager.CreateInstance<TemplateBase>(virtualPath);
			template.VirtualPath = virtualPath;

			return template;
		}

		internal virtual TemplateBase CreatePageFromVirtualPath(string path)
		{
			return CreateInstanceFromVirtualPath(path);
		}

		private TemplateContext CreatePageContextFromParameters(bool isLayoutPage, params object[] data)
		{
			object first = null;
			if (data != null && data.Length > 0)
			{
				first = data[0];
			}

			var pageData = new ValueDictionary();
			return CreateNestedPageContext(TemplateContext, pageData, first, isLayoutPage);
		}

		public void DefineSection(string name, SectionWriter action)
		{
			if (SectionWriters.ContainsKey(name))
			{
				throw new HttpException(String.Format("WebPageResources.WebPage_SectionAleadyDefined {0}", name));
			}
			SectionWriters[name] = action;
		}

		internal void EnsurePageCanBeRequestedDirectly(string methodName)
		{
			if (PreviousSectionWriters == null)
			{
				throw new HttpException(String.Format("WebPageResources.WebPage_CannotRequestDirectly {0} {1}", VirtualPath, methodName));
			}
		}

		public void ExecutePageHierarchy(TemplateContext pageContext, TextWriter writer)
		{
			ExecutePageHierarchy(pageContext, writer, null);
		}

		// This method is only used by WebPageBase to allow passing in the view context and writer.
		public void ExecutePageHierarchy(TemplateContext context, TextWriter writer, TemplateRenderingBase startPage)
		{
			PushContext(context, writer);

			if (startPage != null)
			{
				if (startPage != this)
				{
					var startPageContext = CreateNestedPageContext<object>(context, null, null, false);
					startPageContext.Template = startPage;
					startPage.TemplateContext = startPageContext;
				}
				startPage.Execute();
			}
			else
			{
				Execute();
			}
			PopContext();
		}

		public override void Execute()
		{
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

		protected virtual void Initialize()
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
				// If a layout file was specified, render it passing our page content
				OutputStack.Push(_currentWriter);
				RenderSurrounding(
					Layout,
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

		public void PushContext(TemplateContext context, TextWriter writer)
		{
			_currentWriter = writer;
			TemplateContext = context;
			context.Template = this;

			Initialize();

			// Create a temporary writer
			_tempWriter = new StringWriter(CultureInfo.InvariantCulture);

			// Render the page into it
			OutputStack.Push(_tempWriter);
			SectionWritersStack.Push(new Dictionary<string, SectionWriter>(StringComparer.OrdinalIgnoreCase));

			// If the body is defined in the ViewData, remove it and store it on the instance
			// so that it won't affect rendering of partial pages when they call VerifyRenderedBodyOrSections
			if (TemplateContext.BodyRenderer != null)
			{
				_body = TemplateContext.BodyRenderer;
				TemplateContext.BodyRenderer = null;
			}
		}

		public TemplateResult RenderBody()
		{
			EnsurePageCanBeRequestedDirectly("RenderBody");

			if (_renderedBody)
			{
				throw new HttpException("WebPageResources.WebPage_RenderBodyAlreadyCalled");
			}
			_renderedBody = true;

			// _body should have previously been set in Render(ViewContext,TextWriter) if it
			// was available in the ViewData.
			if (_body != null)
			{
				return new TemplateResult(tw => _body(tw));
			}
			else
			{
				throw new HttpException(String.Format("WebPageResources.WebPage_CannotRequestDirectly {0} {1}", VirtualPath, "RenderBody"));
			}
		}

		public override TemplateResult Render(string path, params object[] data)
		{
			return RenderPageCore(path, false, data);
		}

		private TemplateResult RenderPageCore(string path, bool isLayoutPage, object[] data)
		{
			Precondition.Defined(path, () => Error.ArgumentNull("path"));

			return new TemplateResult(writer => {
				path = NormalizePath(path);
				//Util.EnsureValidPageType(this, path);

				TemplateBase subPage = CreatePageFromVirtualPath(path);
				var pageContext = CreatePageContextFromParameters(isLayoutPage, data);

				subPage.Configure(this);
				subPage.ExecutePageHierarchy(pageContext, writer);
			});
		}

		public TemplateResult RenderSection(string name)
		{
			return RenderSection(name, true);
		}

		public TemplateResult RenderSection(string name, bool required)
		{
			EnsurePageCanBeRequestedDirectly("RenderSection");

			if (PreviousSectionWriters.ContainsKey(name))
			{
				var result = new TemplateResult(tw => {
					if (_renderedSections.Contains(name))
					{
						throw new HttpException(String.Format("WebPageResources.WebPage_SectionAlreadyRendered {0}", name));
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
					_renderedSections.Add(name);
				});
				return result;
			}
			else if (required)
			{
				// If the section is not found, and it is not optional, throw an error.
				throw new HttpException(String.Format("WebPageResources.WebPage_SectionNotDefined {0}", name));
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
			var priorValue = TemplateContext.BodyRenderer;
			TemplateContext.BodyRenderer = body;

			// Render the layout file
			Write(RenderPageCore(partialViewName, true, new object[0]));

			// Restore the state
			TemplateContext.BodyRenderer = priorValue;
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
						if (!_renderedSections.Contains(name))
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
						throw new HttpException(String.Format("WebPageResources.WebPage_SectionsNotRendered {0} {1}", VirtualPath, sectionsNotRendered.ToString()));
					}
				}
				else if (!_renderedBody)
				{
					// There are no sections defined, but RenderBody was NOT called.
					// If a body was defined, then RenderBody should have been called.
					throw new HttpException(String.Format("WebPageResources.WebPage_RenderBodyNotCalled {0}", VirtualPath));
				}
			}
		}

		protected override void Write(TemplateResult result)
		{
			WriteTo(Output, result);
		}

		protected override void Write(object value)
		{
			WriteTo(Output, value);
		}
	}
}
