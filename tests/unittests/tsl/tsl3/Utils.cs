using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trinity.Core.Lib;
using NUnit.Framework;

namespace tsl3
{
    public class ResultAttribute : Attribute { }

    public static unsafe class Utils
    {
        public static byte* MakeCopyOfDataInReqWriter(ReqWriter writer)
        {
            byte* newbuf = (byte*)Memory.malloc((ulong)writer.Length);
            Memory.memcpy(newbuf, writer.CellPtr, (ulong)writer.Length);
            return newbuf;
        }

        public static int CalcForSynRsp(int before, IEnumerable<int> nums, byte after, out string response)
        {
            var numList = nums.ToList();
            response = $"{before} {string.Join(" ", numList)} {after}";
            return before + numList.Sum() + after;
        }

        public static int CalcForSyn(int before, IEnumerable<int> nums, byte after)
        {
            List<int> numList = nums.ToList();
            if (before == after)
                return before + after;
            if (numList.Count == 10)
                return numList.Sum();
            return -1;
        }

        public static int CalcForAsyn(int before, IEnumerable<int> nums, byte after)
        {
            List<int> numList = nums.ToList();
            return numList.Sum() - before + after;
        }

        public static void EnsureResults<T, R>(T obj, string nonZeroField, R expectedValue)
        {
            var type = typeof(T);
            var nonZeroProp = (PropertyInfo)type.GetMembers().Single(p => p.MemberType == MemberTypes.Property && p.Name == nonZeroField);
            Assert.AreEqual(expectedValue, (R)nonZeroProp.GetValue(obj));
            EnsureResultsAreDefaultExceptOne<T, R>(obj, nonZeroField);
        }

        public static void EnsureResultsAreDefaultExceptOne<T, R>(T obj, string nonZeroField)
        {
            var type = typeof(T);
            var zeroProps = type.GetMembers()
                .Where(i => i.MemberType == MemberTypes.Property)
                .Where(i => i.CustomAttributes.Any(_ => _.AttributeType == typeof(ResultAttribute)))
                .Where(i => i.Name != nonZeroField)
                .Cast<PropertyInfo>();
            Assert.That(zeroProps.All(p => ((R)p.GetValue(obj)).Equals(default(R))));
        }
    }
}