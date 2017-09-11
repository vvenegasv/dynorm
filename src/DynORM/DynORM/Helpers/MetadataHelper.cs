using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using DynORM.Attributes;
using DynORM.Models;

namespace DynORM.Helpers
{
    internal class MetadataHelper
    {
        private static volatile MetadataHelper _instance;
        private static object _syncRoot = new Object();
        private readonly IEnumerable<Type> _numberTypes = new List<Type>()
        {
            typeof(Int16), typeof(Int32), typeof(Int64),
            typeof(UInt16), typeof(UInt32), typeof(UInt64),
            typeof(decimal), typeof(float)
        };

        private MetadataHelper()
        {
            
        }

        public static MetadataHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MetadataHelper();
                    }
                }
                return _instance;
            }
        }

        public bool TryCastTo<TOuput>(object input, out TOuput output) where TOuput: class
        {
            output = default(TOuput);

            if (input is TOuput)
            {
                output = (TOuput)input;
                return true;
            }
            

            try
            {
                output = (TOuput)Convert.ChangeType(input, typeof(TOuput));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsNumber(Type type)
        {
            if (_numberTypes.Contains(type))
                return true;
            
            return false;
        }
    }
}
