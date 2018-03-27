using System;
using System.IO;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Storage.External
{
    public interface ICellStreamReader : IDisposable
    {
        Task<Tuple<long /*cellId*/, ushort /*cellType*/, byte[] /*bytes*/>> ReadCellAsync();
    }

    public interface ICellStreamWriter : IDisposable
    {
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

        public async Task<Tuple<long, ushort, byte[]>> ReadCellAsync()
        {
            var head = await ReadBytesAsync(HEAD_SIZE);
            if (head == null || head.Length != HEAD_SIZE)
                return new Tuple<long, ushort, byte[]>(0, 0, null);

            var cellId = BitConverter.ToInt64(head, CELLID_OFFSET);
            var cellType = BitConverter.ToUInt16(head, CELLTYPE_OFFSET);
            var cellSize = BitConverter.ToInt32(head, CELLSIZE_OFFSET);
            var bytes = cellSize > 0 ? await ReadBytesAsync(cellSize) : new byte[0];

            return new Tuple<long, ushort, byte[]>(cellId, cellType, bytes);
        }

        private async Task<byte[]> ReadBytesAsync(int count)
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms, count);
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
