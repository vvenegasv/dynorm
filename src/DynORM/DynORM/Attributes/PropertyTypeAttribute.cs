using System;
using System.Collections.Generic;
using System.Text;
using DynORM.Models;

namespace DynORM.Attributes
{
    public class PropertyTypeAttribute: Attribute
    {
        public PropertyType PropertyType { get; set; }
    }
}
