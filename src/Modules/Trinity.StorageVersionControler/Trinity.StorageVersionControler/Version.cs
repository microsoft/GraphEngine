using System;
using System.Collections.Generic;
using System.Text;

namespace Trinity.StorageVersionControler
{
    public class AsmVersion
    {
        
        public AsmVersion(int cellTypeOffset, string versionName, string tslsrcDir, string tslBuildDir, string asmLoadDir)
        {
            _name = versionName;
            _tslSrcDir = tslsrcDir;
            _tslBuildDir = tslBuildDir;
            _asmLoadDir = asmLoadDir;
            _cellTypeOffset = cellTypeOffset;
        }

        public string TslSrcDir { get => _tslSrcDir; }
        public string TslBuildDir { get => _tslBuildDir; }
        public string AsmLoadDir { get => _asmLoadDir; }
        public string Name { get => _name; }
        public int CellTypeOffset { get => _cellTypeOffset; }

        private string _name;

        private string _tslSrcDir;

        private string _tslBuildDir;

        private string _asmLoadDir;

        private int _cellTypeOffset;

    }

}
