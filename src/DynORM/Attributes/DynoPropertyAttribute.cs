using System;
using System.Collections.Generic;
using System.Text;
using DynORM.Models;

namespace DynORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DynoPropertyAttribute: Attribute
    {
        public PropertyType DatabaseColumnType { get; set; }

        public Type Converter { get; set; }
    }
}
