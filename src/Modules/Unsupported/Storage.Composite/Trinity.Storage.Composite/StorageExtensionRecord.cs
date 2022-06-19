using System.IO;

namespace Trinity.Storage.Composite
{
    [System.Serializable]
    public class StorageExtensionRecord
    {
        public StorageExtensionRecord
            (int cellTypeOffset,
            string rootNamespace,
            string assemblyName)
        {
            AssemblyName   = assemblyName;
            RootNamespace  = rootNamespace;
            CellTypeOffset = cellTypeOffset;
        }

        public int CellTypeOffset { get; }
        public string RootNamespace { get; }
        public string AssemblyName { get; }
    }
}
