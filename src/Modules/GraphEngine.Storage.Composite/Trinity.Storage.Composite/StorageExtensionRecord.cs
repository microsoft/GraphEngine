namespace Trinity.Storage.Composite
{
    [System.Serializable]
    public class StorageExtensionRecord
    {
        public StorageExtensionRecord
            (int cellTypeOffset,
            string moduleName,
            string assemblyName)
        {
            AssemblyName   = assemblyName;
            CellTypeOffset = cellTypeOffset;
            ModuleName     = moduleName;
        }

        public int CellTypeOffset { get; }
        public string AssemblyName { get; }
        public string ModuleName { get; }
    }
}
