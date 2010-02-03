using System;

namespace Radischevo.Wahha.Web.Text.Sgml
{
    public enum AttributeType : byte
    {
        Default,
        CData,
        Entity,
        Entities,
        Id,
        IdRef,
        IdRefs,
        Name,
        Names,
        NameToken,
        NameTokens,
        Number,
        Numbers,
        NumberToken,
        NumberTokens,
        Notation,
        Enumeration
    }
}
