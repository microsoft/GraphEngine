// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Trinity.Storage;

namespace Trinity.Modules.Spark
{
    public class DataType
    {
        public string TypeName { get; set; }

        public static DataType ConvertFromType(Type type)
        {
            if (type == null)
                return null;

            if (type.IsPrimitive ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(Guid) ||
                type == typeof(string))
                return new DataType { TypeName = type.FullName };

            if (type.IsGenericType && type.FullName.StartsWith(typeof(Nullable<>).FullName))
                return new NullableValueType
                {
                    TypeName = typeof(NullableValueType).Name,
                    ArgumentType = ConvertFromType(type.GenericTypeArguments[0])
                };

            if (type.IsGenericType && type.FullName.StartsWith(typeof(List<>).FullName))
                return new ArrayType
                {
                    TypeName = typeof(ArrayType).Name,
                    ElementType = ConvertFromType(type.GetGenericArguments()[0])
                };

            if (type.IsArray)
                return new ArrayType
                {
                    TypeName = typeof(ArrayType).Name,
                    ElementType = ConvertFromType(type.GetElementType())
                };

            if (type.IsValueType && !type.IsEnum || type.IsClass && !type.IsGenericType)
            {
                var fieldsTypes = new List<StructField>();
                fieldsTypes.AddRange(type.GetFields().Select(f => StructField.ConvertFromFieldInfo(f)));
                fieldsTypes.AddRange(type.GetProperties().Select(p => StructField.ConvertFromPropertyInfo(p)));
                return new StructType
                {
                    TypeName = typeof(StructType).Name,
                    Fields = fieldsTypes
                };
            }

            return new DataType { TypeName = type.FullName };
        }
    }

    public class NullableValueType : DataType
    {
        public DataType ArgumentType { get; set; }
    }

    public class ArrayType : DataType
    {
        public DataType ElementType { get; set; }
    }

    public class StructField
    {
        public string Name { get; set; }

        public DataType Type { get; set; }

        public bool Nullable { get; set; }

        public static StructField ConvertFromFieldDescriptor(IFieldDescriptor fd)
        {
            if (fd == null)
                return null;

            var type = DataType.ConvertFromType(fd.Type);
            return new StructField
            {
                Name = fd.Name,
                Type = type,
                Nullable = fd.Optional || (type is NullableValueType)
            };
        }

        public static StructField ConvertFromFieldInfo(FieldInfo field)
        {
            var fieldType = DataType.ConvertFromType(field.FieldType);
            return new StructField
            {
                Name = GetMemberName(field),
                Type = fieldType,
                Nullable = fieldType is NullableValueType
            };
        }

        public static StructField ConvertFromPropertyInfo(PropertyInfo prop)
        {
            var propType = DataType.ConvertFromType(prop.PropertyType);
            return new StructField
            {
                Name = GetMemberName(prop),
                Type = propType,
                Nullable = propType is NullableValueType
            };
        }

        static string GetMemberName(MemberInfo member)
        {
            var dataMemberAttr = member.GetCustomAttribute<DataMemberAttribute>();
            return dataMemberAttr == null ? member.Name : dataMemberAttr.Name;
        }
    }

    public class StructType : DataType
    {
        public IEnumerable<StructField> Fields { get; set; }

        public static StructType ConvertFromCellDescriptor(ICellDescriptor cd)
        {
            if (cd == null)
                return null;

            var fields = new List<StructField>();
            fields.Add(new StructField
            {
                Name = "CellID",
                Type = new DataType { TypeName = typeof(long).FullName },
                Nullable = false
            });

            fields.AddRange(cd.GetFieldDescriptors().Select(fd => StructField.ConvertFromFieldDescriptor(fd)));

            return new StructType
            {
                TypeName = typeof(StructType).Name,
                Fields = fields
            };
        }
    }
}
