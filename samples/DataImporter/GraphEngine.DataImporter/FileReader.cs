using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

    /// <summary>
    /// Extract lines from a block read from a stream reader.
    /// Empty lines are ignored.
    /// </summary>
    class BlockReader
    {
        private const int c_bufferSize = 1<<18;
        private bool m_lastBlock;
        private bool m_endWithLineBreak;
        private int  m_blockSize;
        private char[] m_block = new char[c_bufferSize];
        private Task m_task;
        private List<string> m_strings = new List<string>();

        internal BlockReader(StreamReader reader)
        {
            m_blockSize = reader.ReadBlock(m_block, 0, c_bufferSize);
            m_lastBlock = reader.EndOfStream;
            m_task = Task.Factory.StartNew(ExtractStrings);
            m_endWithLineBreak = IsLineBreak(m_block[m_blockSize - 1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsLineBreak(char x)
        {
            return x == '\r' || x == '\n';
        }

        private unsafe void ExtractStrings()
        {
            fixed (char* buf = m_block)
            {
                char* start = buf;
                char* end   = buf + m_blockSize;
                for (char* p = start; p < end; ++p)
                {
                    if (IsLineBreak(*p))
                    {
                        if (start != p)
                        {
                            m_strings.Add(new string(start, 0, (int)(p - start)));
                        }
                        start = p + 1;
                    }
                }

                if (!m_endWithLineBreak)
                {
                    m_strings.Add(new string(start, 0, (int)(end - start)));
                }
            }
        }

        public bool IsFinished()
        {
            return m_task.IsCompleted;
        }

        public List<string> GetLines(ref string remainder)
        {
            if (m_strings.Count > 0)
            {
                m_strings[0] = remainder + m_strings[0];
                if (!m_endWithLineBreak && !m_lastBlock)
                {
                    //  The last line should continue into the next block,
                    //  so we move it to 'remainder'
                    remainder = m_strings[m_strings.Count - 1];
                    m_strings.RemoveAt(m_strings.Count - 1);
                }
                else
                {
                    remainder = "";
                }
            }
            return m_strings;
        }
    }

    class FileReader
    {
        internal static void ReadAheadProc(StreamReader reader, ConcurrentQueue<List<string>> pc_queue)
        {
            long counter = 0;
            Queue<BlockReader> block_queue = new Queue<BlockReader>();
            //int queue_limit = 1;
            int queue_limit = Environment.ProcessorCount;
            string remainder = "";

            while (true)
            {
                bool enqueued = false, dequeued = false;
                int queued = pc_queue.Sum(_ => _.Count);

                if (!reader.EndOfStream && block_queue.Count < queue_limit)
                {
                    block_queue.Enqueue(new BlockReader(reader));
                    enqueued = true;
                }

                if (block_queue.Count != 0 && block_queue.Peek().IsFinished() && pc_queue.Count < 128<<10)
                {
                    var b = block_queue.Dequeue();
                    var chunk = b.GetLines(ref remainder);
                    counter += chunk.Count;
                    pc_queue.Enqueue(chunk);
                    Console.CursorLeft = 0;
                    Console.Write(String.Format("Loaded {0} lines...", counter, queued));
                    dequeued = true;
                }

                if (reader.EndOfStream && block_queue.Count == 0)
                {
                    break;
                }

                if (!enqueued && !dequeued)
                {
                    Thread.Sleep(10);
                }
            }
        }

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

            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read))
            using (var stream = GetInputStream(fs, mode))
            using (var sr = new StreamReader(stream))
            {
                ConcurrentQueue<List<string>> queue = new ConcurrentQueue<List<string>>();
                Thread readahead_thrd = new Thread(()=> {ReadAheadProc(sr, queue);});
                readahead_thrd.Start();
                while (readahead_thrd.IsAlive || !queue.IsEmpty)
                {
                    List<string> chunk = null;
                    if (queue.TryDequeue(out chunk))
                    {
                        foreach (var s in chunk)
                        {
                            yield return s;
                        }
                    }
                    else
                    {
                        Thread.Yield();
                    }
                }
                readahead_thrd.Join();
            }

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
