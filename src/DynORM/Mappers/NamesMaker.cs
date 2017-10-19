using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using DynORM.Helpers;

namespace DynORM.Mappers
{
    internal class NamesMaker<TModel> where TModel : class
    {
        private readonly TModel _item;
        private readonly Dictionary<string, string> _names;
        private readonly ItemHelper _itemHelper;

        public NamesMaker(TModel item)
        {   
            _item = item;
            _names = new Dictionary<string, string>();
            _itemHelper = ItemHelper.Instance;
            Bind();
        }

        public Dictionary<string, string> GetNames()
        {
            return new ReadOnlyDictionary<string, string>(_names).ToDictionary(x => x.Key, x => x.Value);
        }

        public string GetAlias(string name)
        {
            var alias = "#" + name;
            if (_names.ContainsKey(alias))
                return alias;
            return _names.FirstOrDefault(x => x.Value == name).Value;
        }
        
        private void Bind()
        {
            foreach (var property in _item.GetType().GetTypeInfo().GetProperties())
            {
                if (_itemHelper.ColumnIsIgnored(property))
                    continue;

                var name = _itemHelper.GetColumnName(property);
                var alias = "#" + name;
                _names.Add(name, alias);
            }
        }
    }
}
