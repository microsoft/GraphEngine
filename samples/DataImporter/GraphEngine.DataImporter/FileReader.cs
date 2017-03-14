using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
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
            long counter = 0;
            if (filetype == ".gz")
            {
                mode = CompressionMode.GZip;
            }
            else if (filetype == ".zip")
            {
                mode = CompressionMode.Zip;
                {
                    using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            using (var sr = new StreamReader(entry.Open()))
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
                        }
                    }
                }
            }           
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
                    //{
                    //    BufferedStream stream = null;
                    //    long count = 0;
                    //    using (ZipArchive archive = new ZipArchive(new BufferedStream(fs), ZipArchiveMode.Read))
                    //    {
                    //        foreach (ZipArchiveEntry entry in archive.Entries)
                    //        {

                    //            BufferedStream s = new BufferedStream(entry.Open());
                    //            if (count == 0)
                    //            {
                    //                stream = s;
                    //            }
                    //            else
                    //            {
                    //                s.CopyTo(stream);
                    //            }
                    //            count++;
                    //        } 

                    //    }
                    //    return  stream;
                    //}
                    //TODO zip have directory structure
                    throw new NotImplementedException();
                default:
                    throw new NotFiniteNumberException();
            }
        }
    }
}
