namespace Trinity.DynamicCluster.Persistency
{
    public struct PersistedCell
    {
        public long CellId;
        public byte[] Buffer;
        public ushort CellType;
        public int Offset;
        public int Length;
    }
}
