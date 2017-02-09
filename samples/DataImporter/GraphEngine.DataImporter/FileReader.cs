using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;

namespace GraphEngine.DataImporter
{
    enum CompressionMode
    {
        None,
        GZip,
        SevenZip,
        Zip,
        Rar,
        Tar,
    }
    class FileReader
    {
        internal static IEnumerable<string> ReadFile(string path)
        {
            //TODO detect by content
            string filename = Path.GetFileName(path);
            string filetype = Path.GetExtension(filename).ToLower();

            CompressionMode mode = CompressionMode.None;

            if (filetype == ".gz")
            {
                mode = CompressionMode.GZip;
            }
            else if (filetype == ".zip")
            {
                mode = CompressionMode.Zip;
            }//TODO more

            long counter = 0;
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read))
            using (var stream = GetInputStream(fs, mode))
            using (var sr = new StreamReader(stream))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    if (++counter % 1000000 == 0)
                    {
                        Log.WriteLine("Loaded {0} lines...", counter);
                    }
                    yield return s;
                }
            }
            Log.WriteLine("Loaded {0} lines.", counter);
            //return File.ReadLines(filename);
        }

        private static Stream GetInputStream(FileStream fs, CompressionMode mode)
        {
            switch (mode)
            {
                case CompressionMode.None:
                    return new BufferedStream(fs);
                case CompressionMode.GZip:
                    return new GZipStream(new BufferedStream(fs), System.IO.Compression.CompressionMode.Decompress);
                case CompressionMode.Zip:
                    //TODO zip have directory structure
                    throw new NotImplementedException();
                default:
                    throw new NotFiniteNumberException();
            }
        }
    }
}
