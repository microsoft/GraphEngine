// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Utilities.IO
{
    enum DataFieldType
    {
        Double, Float, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, String
    }

    struct DataCellUnion
    {
        public double[][] DoubleCells;
        public float[][] FloatCells;
        public byte[][] ByteCells;
        public sbyte[][] SByteCells;
        public Int16[][] Int16Cells;
        public UInt16[][] UInt16Cells;
        public Int32[][] Int32Cells;
        public UInt32[][] UInt32Cells;
        public Int64[][] Int64Cells;
        public UInt64[][] UInt64Cells;
        public string[][] StringCells;
    }

    internal class DataTable
    {
        string[][] Cells;
        string[] Header;
        string DataFile;
        bool WithHeader;
        bool IsLargeFile;
        char[] Separators;
        int[] SkipColumns;

        int StartLine = 0;
        public int FieldCount = 0;
        public int RecordCount = 0;

        public DataTable(string file, char[] separators, int[] skipColumns = null, bool withHeader = false, bool isLargeFile = false)
        {
            DataFile = file;
            WithHeader = withHeader;
            IsLargeFile = isLargeFile;
            Separators = separators;
            SkipColumns = skipColumns;
        }

        public void Load()
        {
            if (!IsLargeFile)
            {
                StartLine = 0;
                string[] lines = File.ReadAllLines(DataFile);

                if (lines.Length == 0)
                {
                    Console.Error.WriteLine("No data is found in {0}.", DataFile);
                    return;
                }

                if (WithHeader)
                {
                    StartLine = 1;
                    Header = lines[0].Split(Separators);

                }

                if (lines.Length <= StartLine)
                {
                    Console.Error.WriteLine("No data is found in {0}.", DataFile);
                    return;
                }

                string probe_line = lines[StartLine];

                string[] probe_fields = probe_line.Split(Separators);

                if (WithHeader && Header.Length != probe_fields.Length)
                {
                    Console.Error.WriteLine("Header and data do not match: header has {0} fields, data has {1} fields.", Header.Length, probe_fields.Length);
                }

                int AllFieldCount = probe_fields.Length;

                FieldCount = AllFieldCount - SkipColumns.Length;

                RecordCount = lines.Length - StartLine;

                int[] columnIndexes = new int[FieldCount];

                int p = 0;

                Console.WriteLine("Selected columns:");

                for (int j = 0; j < AllFieldCount; j++)
                {
                    if (SkipColumns == null || (!SkipColumns.Contains(j)))
                    {
                        columnIndexes[p++] = j;
                        Console.Write("{0} ", j);
                    }
                }

                Console.WriteLine();

                Cells = new string[RecordCount][];

                for (int i = 0; i < RecordCount; i++)
                {
                    string[] temp = lines[StartLine + i].Split(Separators);
                    Cells[i] = new string[FieldCount];
                    p = 0;
                    foreach (int j in columnIndexes)
                    {
                        if (temp.Length != AllFieldCount)
                        {
                            Console.Error.WriteLine("Data file line {0} does not match the detected data format.", i);
                        }
                        else
                        {
                            Cells[i][p++] = temp[j];
                        }
                    }
                }
                Console.WriteLine("Loaded {0} lines from {1}.", RecordCount, DataFile);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public DataCellUnion Convert(DataFieldType dataType)
        {
            DataCellUnion dataUnion = new DataCellUnion();
            switch (dataType)
            {
                case DataFieldType.Double:
                    dataUnion.DoubleCells = new double[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.DoubleCells[i] = new double[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            double value = 0.0d;
                            if (!double.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.DoubleCells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to double values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.Float:
                    dataUnion.FloatCells = new float[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.FloatCells[i] = new float[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            float value = 0.0f;
                            if (!float.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.FloatCells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to float values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.Byte:
                    dataUnion.ByteCells = new byte[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.ByteCells[i] = new byte[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            byte value = 0;
                            if (!byte.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.ByteCells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to byte values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.SByte:
                    dataUnion.SByteCells = new sbyte[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.SByteCells[i] = new sbyte[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            sbyte value = 0;
                            if (!sbyte.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.SByteCells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to signed byte values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.Int16:
                    dataUnion.Int16Cells = new Int16[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.Int16Cells[i] = new Int16[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            Int16 value = 0;
                            if (!Int16.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.Int16Cells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to Int16 values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.UInt16:
                    dataUnion.UInt16Cells = new UInt16[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.UInt16Cells[i] = new UInt16[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            UInt16 value = 0;
                            if (!UInt16.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.UInt16Cells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to UInt16 values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.Int32:
                    dataUnion.Int32Cells = new Int32[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.Int32Cells[i] = new Int32[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            Int32 value = 0;
                            if (!Int32.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.Int32Cells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to Int32 values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.UInt32:
                    dataUnion.UInt32Cells = new UInt32[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.UInt32Cells[i] = new UInt32[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            UInt32 value = 0;
                            if (!UInt32.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.UInt32Cells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to UInt32 values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.Int64:
                    dataUnion.Int64Cells = new Int64[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.Int64Cells[i] = new Int64[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            Int64 value = 0;
                            if (!Int64.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.Int64Cells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to Int64 values.", RecordCount * FieldCount);
                    break;
                case DataFieldType.UInt64:
                    dataUnion.UInt64Cells = new UInt64[RecordCount][];
                    for (int i = 0; i < RecordCount; i++)
                    {
                        dataUnion.UInt64Cells[i] = new UInt64[FieldCount];
                        for (int j = 0; j < FieldCount; j++)
                        {
                            UInt64 value = 0;
                            if (!UInt64.TryParse(Cells[i][j], out value))
                            {
                                Console.Error.WriteLine("Warning: cannot parse data cell {0}({1}, {2}), set its value = 0.0", Cells[i][j], i, j);
                            }
                            dataUnion.UInt64Cells[i][j] = value;
                        }
                    }
                    Console.WriteLine("Convert the loaded {0} cells to UInt64 values.", RecordCount * FieldCount);
                    break;
                default:
                    dataUnion.StringCells = Cells;
                    Console.WriteLine("Convert the loaded {0} cells to string values.", RecordCount * FieldCount);
                    break;
            }
            return dataUnion;
        }

        public void Peek(int maxRow)
        {
            for (int i = 0; i < maxRow; i++)
            {
                for (int j = 0; j < FieldCount; j++)
                {
                    Console.Write("{0}\t", Cells[i][j]);
                }
                Console.WriteLine();
            }
        }
    }
}
