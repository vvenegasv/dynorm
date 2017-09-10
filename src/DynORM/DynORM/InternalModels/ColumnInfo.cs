using DynORM.Models;

namespace DynORM.InternalModels
{
    internal class ColumnInfo
    {
        public string Name { get; set; }
        public PropertyType PropertyType { get; set; }
        public ColumnType ColumnType { get; set; }
    }
}
