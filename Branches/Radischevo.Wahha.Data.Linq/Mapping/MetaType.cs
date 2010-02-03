using System;

namespace Jeltofiol.Wahha.Data.Linq.Mapping
{
    public abstract class MetaType
    {
        public abstract string MappingID { get; }
        public abstract Type ElementType { get; }
        public abstract Type EntityType { get; }
    }
}
