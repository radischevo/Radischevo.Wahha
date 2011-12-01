using System.Collections.Generic;
using System.Linq;
using Radischevo.Wahha.Web.Abstractions;
using Radischevo.Wahha.Web.Templates.Scope;

namespace Radischevo.Wahha.Web.Templates {

    public abstract class Template : TemplateBase {

        private static List<IWebPageRequestExecutor> _executors = new List<IWebPageRequestExecutor>();

        private HttpContextBase _context;
        private object _model;

        internal bool TopLevelPage {
            get;
            set;
        }

        public override HttpContextBase Context {
            get {
                if (_context == null) {
                    return TemplateContext.Context;
                }
                return _context;
            }
            set {
                _context = value;
            }
        }

        public static void RegisterPageExecutor(IWebPageRequestExecutor executor) {
            _executors.Add(executor);
        }

        public override void Execute() {
            using (ScopeStorage.CreateTransientScope(new ScopeStorageDictionary(ScopeStorage.CurrentScope, Data))) {
                Execute(_executors);
            }
        }

        internal void Execute(IEnumerable<IWebPageRequestExecutor> executors) {
            // Call all the executors until we find one that wants to handle it. This is used to implement features
            // such as AJAX Page methods without having to bake them into the framework.
            // Note that we only do this for 'top level' pages, as these are request-level executors that should not run for each user control/master
            if (!TopLevelPage || !executors.Any(executor => executor.Execute(this))) {
                // No executor handled the request, so use normal processing
                base.Execute();
            }
        }

        protected override void Initialize() {
            base.Initialize();
        }
    }
}