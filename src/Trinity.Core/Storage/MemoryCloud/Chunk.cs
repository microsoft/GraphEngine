using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    public class Chunk
    {
        public Chunk(long low, long high, Storage s)
        {
            LowKey = low;
            HighKey = high;
            Storage = s;
            Id = Guid.NewGuid();
        }

        public long LowKey { get; }
        public long HighKey { get; }
        public Storage Storage { get; }
        public Guid Id { get; }
        public bool Covers(long cellId) => LowKey <= cellId && cellId <= HighKey;

        public static readonly Chunk FullRangeChunk = new Chunk(long.MinValue, long.MaxValue, Global.LocalStorage);
    }
}
