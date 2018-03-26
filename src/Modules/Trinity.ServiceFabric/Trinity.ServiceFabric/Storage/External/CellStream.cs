using System;
using System.IO;
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

        protected const int HEAD_SIZE = CELLID_BYTES + CELLTYPE_BYTES + CELLSIZE_BYTES;
        protected const int CELLID_OFFSET = 0;
        protected const int CELLTYPE_OFFSET = CELLID_BYTES;
        protected const int CELLSIZE_OFFSET = CELLID_BYTES + CELLTYPE_BYTES;

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

        public CellStreamReader(Stream stream) : base(stream)
        { }

        public bool ReadCell(out long cellId, out ushort cellType, out byte[] bytes)
        {
            cellId = -1; cellType = 0; bytes = null;

            var head = ReadBytes(HEAD_SIZE);
            if (head == null || head.Length != HEAD_SIZE)
                return false;

            cellId = BitConverter.ToInt64(head, CELLID_OFFSET);
            cellType = BitConverter.ToUInt16(head, CELLTYPE_OFFSET);
            var cellSize = BitConverter.ToInt32(head, CELLSIZE_OFFSET);

            if (cellSize > 0)
                bytes = ReadBytes(cellSize);

            return true;
        }

        private byte[] ReadBytes(int count)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms, count);
                return ms.ToArray();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
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
                await stream.(bytes, 0, bytes.Length);
                // await ms.CopyToAsync(stream);
            }
        }
    }
}
