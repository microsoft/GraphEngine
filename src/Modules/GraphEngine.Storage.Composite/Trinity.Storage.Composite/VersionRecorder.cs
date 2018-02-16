namespace Trinity.Storage.CompositeExtension
{
    [System.Serializable]
    public class VersionRecorder
    {
        public VersionRecorder
            (int cellTypeOffset,
            string tslSrcDir,
            string tslBuildDir,
            string asmLoadDir,
            string moduleName,
            string versionName)
        {
            _name = versionName;
            _tslSrcDir = tslSrcDir;
            _tslBuildDir = tslBuildDir;
            _asmLoadDir = asmLoadDir;
            _cellTypeOffset = cellTypeOffset;
            _namespace = $"Trinity.Extension.{moduleName}";
        }

        public string TslSrcDir { get => _tslSrcDir; }
        public string TslBuildDir { get => _tslBuildDir; }
        public string AsmLoadDir { get => _asmLoadDir; }
        public string Name { get => _name; }
        public int CellTypeOffset { get => _cellTypeOffset; }
        public string Namespace { get => _namespace; }

        private string _name;

        private string _tslSrcDir;

        private string _tslBuildDir;

        private string _asmLoadDir;

        private int _cellTypeOffset;

        private string _namespace;
    }

}
