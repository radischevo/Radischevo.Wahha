using System.Collections.Generic;

namespace Radischevo.Wahha.Web.Templates.Scope {
    public interface IScopeStorageProvider {
        IDictionary<string, object> CurrentScope {
            get;
            set;
        }

        IDictionary<string, object> GlobalScope {
            get;
        }

    }
}
