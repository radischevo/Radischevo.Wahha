using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Permissions;
using System.Web;
using System.Web.SessionState;

using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Abstractions
{
    /// <summary>
    /// Encapsulates the HTTP intrinsic object that provides access to session-state 
    /// values, and to session-level settings and lifetime-management methods. 
    /// </summary>
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), 
     AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class HttpSessionStateWrapper : HttpSessionStateBase
    {
        #region Instance Fields
        private readonly HttpSessionState _session;
        #endregion

        #region Constructors
        public HttpSessionStateWrapper(HttpSessionState session)
        {
            Precondition.Require(session, () => Error.ArgumentNull("session"));
            _session = session;
        }
        #endregion

        #region Instance Properties
        public override int CodePage
        {
            get
            {
                return _session.CodePage;
            }
            set
            {
                _session.CodePage = value;
            }
        }

        public override HttpSessionStateBase Contents
        {
            get
            {
                return this;
            }
        }

        public override HttpCookieMode CookieMode
        {
            get
            {
                return _session.CookieMode;
            }
        }

        public override int Count
        {
            get
            {
                return _session.Count;
            }
        }

        public override bool IsCookieless
        {
            get
            {
                return _session.IsCookieless;
            }
        }

        public override bool IsNewSession
        {
            get
            {
                return _session.IsNewSession;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return _session.IsReadOnly;
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return _session.IsSynchronized;
            }
        }

        public override object this[string name]
        {
            get
            {
                return _session[name];
            }
            set
            {
                _session[name] = value;
            }
        }

        public override object this[int index]
        {
            get
            {
                return _session[index];
            }
            set
            {
                _session[index] = value;
            }
        }

        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get
            {
                return _session.Keys;
            }
        }

        public override int LCID
        {
            get
            {
                return _session.LCID;
            }
            set
            {
                _session.LCID = value;
            }
        }

        public override SessionStateMode Mode
        {
            get
            {
                return _session.Mode;
            }
        }

        public override string SessionID
        {
            get
            {
                return _session.SessionID;
            }
        }

        public override HttpStaticObjectsCollectionBase StaticObjects
        {
            get
            {
                return new HttpStaticObjectsCollectionWrapper(_session.StaticObjects);
            }
        }

        public override object SyncRoot
        {
            get
            {
                return _session.SyncRoot;
            }
        }

        public override int Timeout
        {
            get
            {
                return _session.Timeout;
            }
            set
            {
                _session.Timeout = value;
            }
        }
        #endregion

        #region Instance Methods
        public override void Abandon()
        {
            _session.Abandon();
        }

        public override void Add(string name, object value)
        {
            _session.Add(name, value);
        }

        public override void Clear()
        {
            _session.Clear();
        }

        public override void CopyTo(Array array, int index)
        {
            _session.CopyTo(array, index);
        }

        public override IEnumerator GetEnumerator()
        {
            return _session.GetEnumerator();
        }

        public override void Remove(string name)
        {
            _session.Remove(name);
        }

        public override void RemoveAll()
        {
            _session.RemoveAll();
        }

        public override void RemoveAt(int index)
        {
            _session.RemoveAt(index);
        }
        #endregion
    }
}
