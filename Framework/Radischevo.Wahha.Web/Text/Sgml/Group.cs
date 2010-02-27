using System;
using System.Collections;
using Radischevo.Wahha.Core;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    /// <summary>
    /// Defines a group of elements nested within another element.
    /// </summary>
    public class Group
    {
        #region Instance Fields
        private GroupType _type;
        private ArrayList _members;
        private bool _mixed;
        private Occurrence _occurrence;
        private Group _parent;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new Content Model Group.
        /// </summary>
        /// <param name="parent">The parent model group.</param>
        public Group(Group parent)
        {
            _parent = parent;
            _members = new ArrayList();
            _type = GroupType.None;
            _occurrence = Occurrence.Required;
        }
        #endregion

        #region Instance Properties
        /// <summary>
        /// The parent group of this group.
        /// </summary>
        public Group Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// The <see cref="Occurrence"/> of this group.
        /// </summary>
        public Occurrence Occurrence
        {
            get
            {
                return _occurrence;
            }
        }

        /// <summary>
        /// Checks whether the group contains only text.
        /// </summary>
        /// <value>true if the group is of mixed content 
        /// and has no members, otherwise false.</value>
        public bool TextOnly
        {
            get
            {
                return (_mixed && (_members.Count == 0));
            }
        }
        #endregion

        #region Instance Methods
        public void AddConnector(char c)
        {
            if (!_mixed && _members.Count == 0)
                throw Error.MissingTokenBeforeConnector(c);
            
            GroupType none = GroupType.None;
            switch (c)
            {
                case '&':
                    none = GroupType.And;
                    break;
                case ',':
                    none = GroupType.Sequence;
                    break;
                case '|':
                    none = GroupType.Or;
                    break;
            }
            if (_type != GroupType.None && _type != none)
                throw Error.InconsistentConnector(c, _type);
            
            _type = none;
        }

        public void AddGroup(Group g)
        {
            _members.Add(g);
        }

        public void AddOccurrence(char c)
        {
            Occurrence o = Occurrence.Required;
            switch (c)
            {
                case '?':
                    o = Occurrence.Optional;
                    break;
                case '+':
                    o = Occurrence.OneOrMore;
                    break;
                case '*':
                    o = Occurrence.ZeroOrMore;
                    break;
            }
            _occurrence = o;
        }

        public void AddSymbol(string sym)
        {
            if (sym == "#PCDATA")
                _mixed = true;
            else
                _members.Add(sym);
        }

        public bool CanContain(string name, SgmlDtd dtd)
        {
            Precondition.Require(dtd, () => Error.ArgumentNull("dtd"));

            // Do a simple search of members.
            foreach (object obj in _members)
            {
                if (obj is String)
                {
                    if (String.Equals((string)obj, name, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            // didn't find it, so do a more expensive search over child elements
            // that have optional start tags and over child groups.
            foreach (object obj in _members)
            {
                string s = (obj as string);
                if (s != null)
                {
                    ElementDeclaration e = dtd.FindElement(s);
                    if (e != null)
                    {
                        if (e.StartTagOptional)
                        {                            
                            if (e.CanContain(name, dtd))
                                return true;
                        }
                    }
                }
                else
                {
                    Group m = (Group)obj;
                    if (m.CanContain(name, dtd))
                        return true;
                }
            }
            return false;
        }
        #endregion
    }
}
