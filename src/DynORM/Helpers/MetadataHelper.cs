using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.DynamoDBv2.DataModel;
using DynORM.Attributes;
using DynORM.Models;
using Newtonsoft.Json.Converters;

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
            typeof(decimal), typeof(float), typeof(Single), typeof(double),
            typeof(Int16?), typeof(Int32?), typeof(Int64?),
            typeof(UInt16?), typeof(UInt32?), typeof(UInt64?),
            typeof(decimal?), typeof(float?), typeof(Single?), typeof(double?)
        };

        private readonly IEnumerable<Type> _dateTypes = new List<Type>()
        {
            typeof(DateTime), typeof(DateTime?)
        };

        private readonly IEnumerable<Type> _boolTypes = new List<Type>()
        {
            typeof(bool),
            typeof(Boolean),
            typeof(bool?),
            typeof(Boolean?)
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
            return _numberTypes.Contains(type);
        }

        public bool IsBoolean(Type type)
        {
            return _boolTypes.Contains(type);
        }

        public bool IsDate(Type type)
        {
            return _dateTypes.Contains(type);
        }

        public bool IsStream(Type type)
        {
            return type == typeof(MemoryStream);
        }

        public bool IsCustomObject(Type type)
        {
            return !type.GetTypeInfo().IsPrimitive && type.GetTypeInfo().IsClass && type != typeof(string);
        }

        public bool IsSingleValue(Type type)
        {
            return IsNumber(type) || IsDate(type) || IsBoolean(type) || type == typeof(string);
        }

        public bool GenericArgumentIsNumber(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            var argType = underlyingType?.GenericTypeArguments?[0];
            return argType != null && IsNumber(argType);
        }

        public bool GenericArgumentIsBoolean(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            var argType = underlyingType?.GenericTypeArguments?[0];
            return argType != null && IsBoolean(argType);
        }

        public bool GenericArgumentIsDate(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            var argType = underlyingType?.GenericTypeArguments?[0];
            return argType != null && IsDate(argType);
        }

        public bool GenericArgumentIsStream(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            var argType = underlyingType?.GenericTypeArguments?[0];
            return argType != null && IsStream(argType);
        }

        public bool GenericArgumentIsCustomObject(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            var argType = underlyingType?.GenericTypeArguments?[0];
            return argType != null && IsCustomObject(argType);
        }

        public bool GenericArgumentIsSingleValue(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            var argType = underlyingType?.GenericTypeArguments?[0];
            return argType != null &&
                   (IsNumber(argType) || IsDate(argType) || IsBoolean(argType) || argType == typeof(string));
        }

        public bool GenericArgumentIsString(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            var argType = underlyingType?.GenericTypeArguments?[0];
            return argType != null && argType == typeof(string);
        }

        public bool IsList(Type type)
        {
            var underlyingType = type.GetTypeInfo().UnderlyingSystemType;
            return underlyingType != null
                   && underlyingType.Namespace.Equals("System.Collections.Generic")
                   && underlyingType.Name.StartsWith("List");
        }
    }
}
