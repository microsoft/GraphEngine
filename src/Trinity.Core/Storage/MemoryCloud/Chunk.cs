using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    /// <summary>
    /// Represents
    /// </summary>
    public class Chunk : IEquatable<Chunk>
    {
        public Chunk(long lowKey, long highKey)
            : this(lowKey, highKey, Guid.NewGuid()) { }

        public Chunk(long lowKey, long highKey, Guid id)
        {
            LowKey = lowKey;
            HighKey = highKey;
            Id = id;
        }

        public long LowKey { get; }
        public long HighKey { get; }
        public Guid Id { get; }
        public bool Covers(long cellId) => LowKey <= cellId && cellId <= HighKey;

        public bool Equals(Chunk other)
        {
            return Id == other.Id && LowKey == other.LowKey && HighKey == other.HighKey;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static readonly Guid FullRangeChunkGuid = new Guid("5C1A0664-7257-42E8-8D02-9CB615DA680A");
        public static readonly Guid NullChunkGuid = new Guid("B0C5B39B-94FE-4A47-A947-318B4CFFDBFF");

        public static readonly Chunk FullRangeChunk = new Chunk(long.MinValue, long.MaxValue, FullRangeChunkGuid);
        public static readonly Chunk NullChunk = new Chunk(long.MaxValue, long.MinValue, NullChunkGuid);
    }
}
