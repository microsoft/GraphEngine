using System;
using System.Threading.Tasks;

namespace Trinity.DynamicCluster.Persistency
{
    public interface IPersistentUploader : IDisposable
    {
        /// <summary>
        /// Marks the current snapshot as finished. After this method is called,
        /// the snapshot becomes the latest version.
        /// !Note, this method must be called once, when all peers have finished
        /// uploading their parts.
        /// </summary>
        Task FinishUploading();
        /// <summary>
        /// Asynchronously upload a data chunk. Note, a storage implementation
        /// is free to choose whether to store one chunk of data as a single
        /// file, or to use a continuous, append-only model. Therefore, when
        /// later downloaded, the chunk ranges may differ from what is supplied
        /// during uploading.
        /// </summary>
        Task UploadAsync(IPersistentDataChunk payload);
    }
}
