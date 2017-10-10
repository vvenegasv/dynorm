using System;
using System.Collections.Generic;
using System.Text;

namespace DynORM.InternalModels
{
    internal class KeyInfo
    {
        public string KeyName { get; set; }
        public KeyType KeyType { get; set; }
        public IDictionary<ColumnType, ColumnInfo> Columns { get; set; }
    }
}
