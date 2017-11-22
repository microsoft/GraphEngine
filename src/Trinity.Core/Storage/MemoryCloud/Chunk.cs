using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    public class Chunk
    {
        public Chunk(long lowKey, long highKey, Storage storage)
            : this(lowKey, highKey, storage, Guid.NewGuid()) { }

        public Chunk(long lowKey, long highKey, Storage storage, Guid id)
        {
            LowKey = lowKey;
            HighKey = highKey;
            Storage = storage;
            Id = id;
        }

        public long LowKey { get; }
        public long HighKey { get; }
        public Storage Storage { get; }
        public Guid Id { get; }
        public bool Covers(long cellId) => LowKey <= cellId && cellId <= HighKey;

        public static readonly Chunk FullRangeChunk = new Chunk(long.MinValue, long.MaxValue, Global.LocalStorage);
    }
}
