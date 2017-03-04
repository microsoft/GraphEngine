using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class StorageCodeTemplate
    {
        private static string GenerateForEachExtension()
        {
            return @"
    static public class Storage_ForEach_Extension
    {
        /// <summary>
        /// Perform actions to each corresponding cell in Trinity local storage.
        /// </summary>
        /// <param name=""actions"">A list of CellAccessorActions.</param>
        public static unsafe void ForEach(this Trinity.Storage.LocalMemoryStorage storage, params CellAccessorAction[] actions)
        {
            Dictionary<ushort, Action<long, long, int>> actionMap = new Dictionary<ushort, Action<long, long, int>>();
            foreach (var action in actions)
            {
                actionMap[(ushort)action.CellType] = action._action;
            }

            foreach (var cellInfo in Global.LocalStorage)
            {
                Action<long, long, int> action;
                if (actionMap.TryGetValue(cellInfo.CellType, out action))
                {
                    action(cellInfo.CellId, (long)cellInfo.CellPtr, cellInfo.CellEntryIndex);
                }
            }
        }
    }
";
        }
    }
}