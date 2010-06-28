using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Radischevo.Wahha.Core;
using Radischevo.Wahha.Web.Text;
using Radischevo.Wahha.Web.Mvc.UI;

namespace Radischevo.Wahha.Web.Mvc.Ajax
{
    public class ScriptManager
    {
        #region Nested Types
        public sealed class ScriptBlock : IHideObjectMembers
        {
            #region Instance Fields
            private ScriptManager _manager;
            private List<Action> _scripts;
            private Action<string> _wrapper;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the 
            /// <see cref="Radischevo.Wahha.Web.Mvc.Ajax.ScriptBlock"/> class.
            /// </summary>
            /// <param name="manager">A parent script manager instance.</param>
            public ScriptBlock(ScriptManager manager)
            {
                Precondition.Require(manager, () => Error.ArgumentNull("manager"));
                _manager = manager;
                _scripts = new List<Action>();
            }
            #endregion

            #region Instance Methods
            internal bool IsEmpty
            {
                get
                {
                    return (_scripts.Count < 1);
                }
            }

            /// <summary>
            /// Appends a script to the script block.
            /// </summary>
            /// <param name="script">A script code string to add.</param>
            public ScriptBlock Append(Action script)
            {
                _scripts.Add(script);
                return this;
            }

            /// <summary>
            /// Wraps the script block using
            /// the specified <paramref name="wrapper"/>.
            /// </summary>
            /// <param name="wrapper">A wrapper method to 
            /// decorate the script block with.</param>
            public void Wrap(Action<string> wrapper)
            {
                Precondition.Require(wrapper, () => Error.ArgumentNull("wrapper"));
                _wrapper = wrapper;
            }

            /// <summary>
            /// Renders a script block to the specified 
            /// <paramref name="writer"/>.
            /// </summary>
            /// <param name="writer">A <see cref="System.IO.TextWriter"/> 
            /// to render the script block to.</param>
            internal void Render(TextWriter writer)
            {
                HtmlHelper helper = new HtmlHelper(_manager._context);
                StringBuilder builder = new StringBuilder(Environment.NewLine);

                foreach (string code in _scripts.Where(a => a != null)
                    .Select(a => helper.Block(a)))
                    builder.AppendLine(code);

                if (builder.Length > Environment.NewLine.Length)
                    writer.WriteLine((_wrapper == null) ?
                        builder.ToString() :
                        helper.Block(_wrapper, builder.ToString()));
            }
            #endregion
        }
        #endregion

        #region Instance Fields
        private ViewContext _context;
        private Dictionary<string, string> _includes;
        private Dictionary<string, ScriptBlock> _blocks;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Radischevo.Wahha.Web.Mvc.Ajax.ScriptManager"/> class.
        /// </summary>
        /// <param name="context">The current 
        /// <see cref="Radischevo.Wahha.Web.Mvc.ViewContext"/> instance.</param>
        public ScriptManager(ViewContext context)
        {
            Precondition.Require(context, () => Error.ArgumentNull("context"));
            _context = context;
            _includes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _blocks = new Dictionary<string, ScriptBlock>(StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Instance Methods
        /// <summary>
        /// Adds a script file reference to the page.
        /// </summary>
        /// <param name="path">The URL of the script file.</param>
        public ScriptManager Include(string path)
        {
            return Include(Guid.NewGuid().ToString(), path);
        }

        /// <summary>
        /// Adds a script file reference to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="path">The URL of the script file.</param>
        public ScriptManager Include(string key, string path)
        {
            if (!_includes.ContainsKey(key) && !String.IsNullOrEmpty(path))
            {
                if (path.StartsWith("~/"))
                    path = VirtualPathUtility.ToAbsolute(path);
                
                _includes.Add(key, path);
            }
            return this;
        }

        /// <summary>
        /// Adds an inline script block to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script block.</param>
        public ScriptBlock Block(string key)
        {
            return Block(key, null);
        }

        /// <summary>
        /// Adds an inline script block to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script block.</param>
        /// <param name="script">The JavaScript code to include in the Page.</param>
        public ScriptBlock Block(string key, Action script)
        {
            ScriptBlock block;
            if (!_blocks.TryGetValue(key, out block))
            {
                block = new ScriptBlock(this);
                _blocks[key] = block;
            }
            if (script != null)
                block.Append(script);

            return block;
        }

        /// <summary>
        /// Renders the ScriptManager to the page.
        /// </summary>
        public void Render()
        {
            TextWriter writer = _context.Context.Response.Output;
            HtmlElementBuilder builder = new HtmlElementBuilder("script", 
				new { Type = "application/javascript" }, String.Empty);

            foreach (string path in _includes.Values)
            {
                builder.Attributes.Merge("src", path, true);
                writer.WriteLine(builder.ToString());
            }

            if (_blocks.Values.Any(b => !b.IsEmpty))
            {
                builder.Attributes.Remove("src");
                writer.WriteLine(builder.ToString(HtmlElementRenderMode.StartTag));
                writer.WriteLine("//<![CDATA[");

                foreach (ScriptBlock script in _blocks.Values)
                    script.Render(writer);

                writer.WriteLine("//]]>");
                writer.WriteLine(builder.ToString(HtmlElementRenderMode.EndTag));
            }
            _blocks.Clear();
            _includes.Clear();
        }
        #endregion
    }
}
