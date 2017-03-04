using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.TSL.Test.TestModules;
using System.IO;

namespace Trinity.TSL.Test
{
    class TestCaseFactory
    {
        //this code is useful every where!
        static Random r = new Random();

        //Bad design: if we ever invoke Not,Xor,Or,And on BitArray,we mark it true;
        public static bool LogicOpearationOnBitArray = false; 


        static int GetRandom(int range)
        {
            return (int)(r.NextDouble() * range);
        }
        public static string NewTestCase(SpecificationScript script,string outputPath)
        {
            StringBuilder ret = new StringBuilder();
            ret.Append(@"
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using Trinity;
using Trinity;
using Trinity.Storage;
using Trinity.Data;
using Trinity.Diagnostics;
using System.Diagnostics;
using System.Collections;

namespace DMTestFramework
{
    public static class Array_Extension
    {
        public static void Clear(this Array array,int index, int count)
        {
            Array.Clear(array, index, count);
        }
    }
    class Program
    {
        static Random r = new Random();

        static int GetRandom(int range)
        {
            return (int)(r.NextDouble() * range);
        }

        /// <summary>
        /// get a random number within range
        /// </summary>
        /// <param name=""range""></param>
        /// <returns></returns>
        static int GetRandomIncludingLast(int range)
        {
            return (int)(r.NextDouble() * (1+range));
        }

       /// <summary>
       /// get a random range from within ""range"" 
       /// </summary>
       /// <param name=""range""></param>
       /// <param name=""index"">starter of the range</param>
       /// <param name=""count"">number of elements in the range</param>
       /// <returns></returns>
        static void GetRandomRange(int range,ref int index, ref int count)
        {
            index = GetRandom(range);
            count = (int)(r.NextDouble()*(range-index+1));
        }

        static BitArray GetFixedSizeBitArray(int size)
        {
            BitArray temp = new BitArray(size);
            for (int i = 0; i < size; ++i)
            {
                temp[i] = r.Next() % 2 == 0?true:false;
            }
            return temp;
        }

        static void DoNothing(object value)
        {
        }


        static unsafe void  Main(string[] args)
        {
Console.WriteLine(""Starting to do verify work..."");
            TrinityConfig.CurrentRunningMode = Trinity.RunningMode.Embedded;
            bool testResult = true;
            bool temp = true;
            Stopwatch s1 = new Stopwatch();
            Stopwatch s2 = new Stopwatch();
    
");

            Dictionary<string, int> cellIdDic = new Dictionary<string, int>();
            HashSet<int> ids = new HashSet<int>();

            foreach (StructDescriptor cell in script.CellDescriptors)
            {
                ret.Append(cell.Name + " _" + cell.Name + " = ");
                ret.Append("new " + cell.Name + "(");

                //save the new cell into global.localstorage
                int idpoolsize = 1000;
                int newid = GetRandom(idpoolsize);
                while (ids.Contains(newid))
                {
                    idpoolsize += 100;
                    newid = GetRandom(idpoolsize);
                }
                ids.Add(newid);
                cellIdDic.Add("_" + cell.Name, newid);
                ret.Append(newid + ",");

                foreach (Field field in cell.Fields)
                {
                    ret.Append(ObjectGenerator.generate(field.Type));
                    ret.Append(",");
                }
                if (ret[ret.Length - 1] != '(')
                    ret = ret.Remove(ret.Length - 1,1);
                ret.Append(");\r\n");

                ret.Append("Global.LocalStorage.Save" + cell.Name + "(_" + cell.Name + ");\r\n");


            }

            List<string> readySens = new List<string>();
            string codeForCSharpCell = "";
            

            foreach (StructDescriptor cell in script.CellDescriptors)
            {
                ret .Append( "using(var mycellAcc = Global.LocalStorage.Use"+cell.Name+"(" + cellIdDic["_" + cell.Name] + @"))
                {");
                //generate some operations
                const int OperationNum = 1000;
                int fieldsListLength = cell.Fields.Count;
                for (int i = 0; i < OperationNum; ++i)
                {
                    TestCaseFactory.LogicOpearationOnBitArray = false;

                    codeForCSharpCell = "//code for c# cell.\r\n";
                    readySens.Clear();
                    readySens.Add("//this is the " + i + " operation");
                    readySens.Add("try{");
                    //simulate mycellAcc'storage behaviours first.
                    //DONT simulate mycellACC'storage first, because in many cases mycellAcc are more capable than
                    //c# objects, say, it can deal with cell.ARRAY[0].sth when C# object throw NULLREFERENCEEXCEPTION
                    int selectFieldIndex = GetRandom(fieldsListLength);
                    string workingsens = "mycellAcc.";
                    Field field = cell.Fields[selectFieldIndex];
                    workingsens += field.Name;
                    TestCodeTemplate.GenerateTestCode(field.Type, readySens, workingsens, 0,false,false);

                    codeForCSharpCell = readySens[readySens.Count - 1].Replace("mycellAcc","_"+cell.Name);
                    readySens.Add(codeForCSharpCell);

                    if (TestCaseFactory.LogicOpearationOnBitArray)
                    {
                        readySens[readySens.Count - 2] = readySens[readySens.Count - 2].
                            Substring(readySens[readySens.Count - 2].IndexOf('=') + 1);
                    }

                    //switch the order of excution
                    string temp = readySens[readySens.Count - 1];
                    readySens[readySens.Count - 1] = readySens[readySens.Count - 2];
                    readySens[readySens.Count - 2] = temp;


                    //insert profiling mesaurements
                    readySens.Insert(readySens.Count - 2, "s2.Start();");
                    readySens.Insert(readySens.Count - 1, "s2.Stop();");
                    readySens.Insert(readySens.Count - 1, "s1.Start();");
                    readySens.Add("s1.Stop();");


                    foreach (string s in readySens)
                    {
                        ret.Append(s + "\r\n");
                    }
                    ret .Append( @"
                            temp = (mycellAcc == _"+cell.Name + @");
                                    
                            if(!temp){
Console.WriteLine(""the accessor cell:"");
HexDump.Dump(mycellAcc.ToByteArray());
Console.WriteLine(""the c# cell:"");
HexDump.Dump((("+cell.Name+"_Accessor)_"+cell.Name+@").ToByteArray());
                                throw new Exception(""Validation fails here!!!Seeing this message necessarily means an error in tsl. Please report your tsl file to us to fix the problem!"");
}
");
                    ret.Append("Console.WriteLine(\"Opeartion\"+" + i + ");\r\n");
                    ret.Append("Console.WriteLine(temp);\r\n");
                    ret.Append("Console.WriteLine();\r\n\r\n");
                    temp = temp.Replace('"', '?');
                    ret .Append( @"}catch(NullReferenceException){
s2.Stop();
Console.WriteLine(""[NORMAL EXCEPTION]NULLPOINTER to run "
                    + temp + @""");}
catch(ArgumentOutOfRangeException){
s2.Stop();
Console.WriteLine(""[NORMAL EXCEPTION]OutOfRange to run "
                    + temp + @""");}
");
                }
 
                ret .Append( @"
}");

            }
            ret.Append("Console.WriteLine(\"time for accessor:\" + s1.ElapsedTicks);");
            ret.Append("Console.WriteLine(\"time for c# container:\" + s2.ElapsedTicks);");
            ret.Append("Console.WriteLine(\"Validation Passed. In most cases this means that your tsl file is okay. BUT this is NOT is garanteed. Please use this at your own risk.\");");
            ret.Append("Console.WriteLine(\"Press any key to continue...\");");
            ret.Append("Console.ReadLine();");
            ret.Append("}}}");

            string ret_string = ret.ToString();

            return ret_string;
        }
    }
}
