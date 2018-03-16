using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Storage.External
{
    public interface ICellStreamReader : IDisposable
    {
        bool ReadCell(out long cellId, out ushort cellType, out byte[] bytes);
    }

    public interface ICellStreamWriter : IDisposable
    {
        void WriteCell(long cellId, ushort cellType, byte[] content);
        Task WriteCellAsync(long cellId, ushort cellType, byte[] content);
    }

    public abstract class CellStream : IDisposable
    {
        // [cell_id, cell_type, cell_size, cell_content]
        protected const int CELLID_BYTES = sizeof(long);
        protected const int CELLTYPE_BYTES = sizeof(ushort);
        protected const int CELLSIZE_BYTES = sizeof(int);

        protected Stream stream;

        public CellStream(Stream stream)
        {
            this.stream = stream;
        }

        public virtual void Dispose()
        {
            if (stream != null)
                stream.Dispose();
        }
    }

    public class CellStreamReader : CellStream, ICellStreamReader
    {
        protected const int DEFAULT_BUFFER_SIZE = 1024 * 1024;
        protected readonly byte[] buffer = new byte[DEFAULT_BUFFER_SIZE];
        private readonly MemoryStream memstream = new MemoryStream();

        public CellStreamReader(Stream stream) : base(stream)
        { }

        public bool ReadCell(out long cellId, out ushort cellType, out byte[] bytes)
        {
            cellId = -1; cellType = 0; bytes = null;

            var nread = ReadBytes(CELLID_BYTES, force:false);
            if (nread == 0)
                return false;

            cellId = BitConverter.ToInt64(memstream.ToArray(), 0);

            ReadBytes(CELLTYPE_BYTES);
            cellType = BitConverter.ToUInt16(memstream.ToArray(), 0);

            ReadBytes(CELLSIZE_BYTES);
            var cellSize = BitConverter.ToInt32(memstream.ToArray(), 0);

            if (cellSize > 0)
            {
                ReadBytes(cellSize);
                bytes = memstream.ToArray();
            }

            return true;
        }

        private int ReadBytes(int count, bool force = true)
        {
            memstream.Seek(0, SeekOrigin.Begin);
            var bytesToRead = count;
            while (bytesToRead > 0)
            {
                var nread = stream.Read(buffer, 0, bytesToRead);
                if (nread == 0)
                    break;
                memstream.Write(buffer, 0, nread);
                bytesToRead -= nread;
            }

            if (bytesToRead == 0 || (bytesToRead == count && !force))
                return count - bytesToRead;

            throw new Exception("Corrupted cell stream");
        }

        public override void Dispose()
        {
            base.Dispose();
            memstream.Dispose();
        }
    }

    public class CellStreamWriter : CellStream, ICellStreamWriter
    {
        public CellStreamWriter(Stream stream) : base(stream)
        { }

        public void WriteCell(long cellId, ushort cellType, byte[] content)
        {
            stream.Write(BitConverter.GetBytes(cellId), 0, CELLID_BYTES);
            stream.Write(BitConverter.GetBytes(cellType), 0, CELLTYPE_BYTES);
            stream.Write(BitConverter.GetBytes(content.Length), 0, CELLSIZE_BYTES);
            stream.Write(content, 0, content.Length);
        }

        public async Task WriteCellAsync(long cellId, ushort cellType, byte[] content)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(BitConverter.GetBytes(cellId), 0, CELLID_BYTES);
                ms.Write(BitConverter.GetBytes(cellType), 0, CELLTYPE_BYTES);
                ms.Write(BitConverter.GetBytes(content.Length), 0, CELLSIZE_BYTES);
                ms.Write(content, 0, content.Length);

                var bytes = ms.ToArray();
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}
