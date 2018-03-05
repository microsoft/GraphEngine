namespace Trinity.Storage.Composite
{
    [System.Serializable]
    public class VersionRecord
    {
        public VersionRecord
            (int cellTypeOffset,
            string tslSrcDir,
            string tslBuildDir,
            string asmLoadDir,
            string moduleName,
            string versionName)
        {
            TslSrcDir = tslSrcDir;
            TslBuildDir = tslBuildDir;
            AsmLoadDir = asmLoadDir;
            Name = versionName;
            CellTypeOffset = cellTypeOffset;
            Namespace = $"Trinity.Extension.{moduleName}";
        }

        public string TslSrcDir { get; }
        public string TslBuildDir { get; }
        public string AsmLoadDir { get; }
        public string Name { get; }
        public int CellTypeOffset { get; }
        public string Namespace { get; }
    }
}
