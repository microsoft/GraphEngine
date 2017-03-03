// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Storage;

namespace Trinity.Modules.Spark
{
    public class DataType
    {
        public string TypeName { get; set; }

        public static DataType ConvertFromType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException();

            if (type.IsPrimitive)
                return new DataType { TypeName = type.FullName };

            if (type.IsValueType)
            {
                if (type.IsGenericType) // Nullable value type
                {
                    return new NullableValueType
                    {
                        TypeName = typeof(NullableValueType).Name,
                        ArgumentType = ConvertFromType(type.GenericTypeArguments[0])
                    };
                }
                else if (type != typeof(DateTime) && type != typeof(decimal) && type != typeof(Guid))
                {
                    return new StructType
                    {
                        TypeName = typeof(StructType).Name,
                        Fields = type.GetFields().Select(f =>
                        {
                            var fieldType = ConvertFromType(f.FieldType);
                            return new StructField
                            {
                                Name = f.Name,
                                Type = fieldType,
                                Nullable = fieldType is NullableValueType
                            };
                        })
                    };
                }
            }

            if (type.IsGenericType)
            {
                if (type.FullName.StartsWith("System.Collections.Generic.List"))
                {
                    return new ArrayType
                    {
                        TypeName = typeof(ArrayType).Name,
                        ElementType = ConvertFromType(type.GetGenericArguments()[0])
                    };
                }
            }

            return new DataType
            {
                TypeName = type.FullName
            };
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
