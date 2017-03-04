using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class StorageCodeTemplate
    {
        private static string GenerateAbstractCellConverter(SpecificationScript script)
        {
            CodeWriter ret = new CodeWriter();
            ret += @"
    public abstract class CellConverter
    {
        internal static Dictionary<Type, CellType> CellTypeMap = new Dictionary<Type, CellType>
        {";
            foreach (var desc in script.CellDescriptors)
            {
                ret += @"
            { typeof(" + desc.Name + "), CellType." + desc.Name + "} ,";
            }
            ret += @"
        };
        internal CellConverter(CellType source, CellType ptr1)
        {
            SourceCellType = source;
            TargetCellType = ptr1;
        }
        public readonly CellType SourceCellType;
        public readonly CellType TargetCellType;
        internal CellTransformAction<long, int, ushort> _action;
    }";
            return ret;
        }
        private static string GenerateGenericCellConverter(SpecificationScript script)
        {
            string ret = @"
    public class CellConverter<TInput, TOutput>
        : CellConverter
    {
        static internal CellType _SourceCellType;
        static internal CellType _TargetCellType;
        static internal Func<long, long, object> InputFunction = null;
        static internal Func<object, byte[]> OutputFunction = null;
        static internal bool InvalidConverter;";
            foreach (var desc in script.CellDescriptors)
            {
                ret += @"
        [ThreadStatic]
        static internal " + desc.Name + "_Accessor Static_" + desc.Name + "_Accessor;";
            }
            ret += @"
        static unsafe CellConverter()
        {
            Type tIn = typeof(TInput);
            Type tOut = typeof(TOutput);
            try
            {
                _SourceCellType = CellConverter.CellTypeMap[tIn];
                _TargetCellType = CellConverter.CellTypeMap[tOut];
            }
            catch (Exception)
            {
                InvalidConverter = true;
            }
            switch(_SourceCellType)
            {";
            foreach (var desc in script.CellDescriptors)
            {
                ret += @"
                case CellType." + desc.Name + @":
                    InputFunction = (CellID, long_ptr) =>
                        {
                            if (Static_" + desc.Name + "_Accessor == default(" + desc.Name + @"_Accessor))
                                Static_" + desc.Name + "_Accessor = new " + desc.Name + @"_Accessor(null);
                            Static_" + desc.Name + @"_Accessor.CellPtr = (byte*)long_ptr; // + 1;
                            return (" + desc.Name + ")Static_" + desc.Name + @"_Accessor;
                        };
                    break;";
            }
            ret += @"
            }
            switch(_TargetCellType)
            {";
            foreach (var desc in script.CellDescriptors)
            {
                ret += @"
                case CellType." + desc.Name + @":
                    OutputFunction = (cell) =>
                        {
                            Static_" + desc.Name + "_Accessor = (" + desc.Name + @")cell;
                            return Static_" + desc.Name + @"_Accessor.ToByteArray();
                        };
                    break;";
            }
            ret += @"
            }
        }
        public unsafe CellConverter(Func<TInput, TOutput> ConvertFunction) :
            base(_SourceCellType, _TargetCellType)
        {
            if (InvalidConverter)
            {
                throw new Exception(string.Format(
                    ""This type of converter ({0} to {1}) is not supported, make sure you pass in a cell-type-to-cell-type converter."",
                    typeof(TInput), typeof(TOutput)));
            }
            _action = (byte* ptr_long, long id, int count, ref ushort cellType) =>
                {
                    TInput input = (TInput)InputFunction(id, (long)ptr_long);
                    TOutput output = ConvertFunction(input);
                    cellType = (ushort)_TargetCellType;
                    return OutputFunction(output);
                };
        }
    }";
            return ret;
        }
    }
}