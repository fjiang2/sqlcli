using System;

namespace Sys.Data.Entity
{
    public interface IConstraint
    {
        Type OtherType { get; }
        string Name { get; set; }
        string ThisKey { get; set; }
        string OtherKey { get; set; }
        bool IsForeignKey { get; set; }
        bool OneToMany { get; set; }
    }
}