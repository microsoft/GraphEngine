using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Config;

namespace Trinity.Azure.Storage
{
    internal static class PipeHelper
    {
        public static TOut Then<TIn, TOut>(this TIn input, Func<TIn, TOut> func) => func(input);
    }

    internal class StorageHelper
    {
        private LinearRetry m_retryPolicy;
        private BlobRequestOptions m_reqops_download;
        private BlobRequestOptions m_reqops_upload;
        private OperationContext m_opctx;
        private CancellationToken m_cancel;

        public StorageHelper(CancellationToken cancel)
        {
            m_retryPolicy = new LinearRetry();
            
            m_reqops_download = new BlobRequestOptions
            {
                LocationMode = LocationMode.PrimaryThenSecondary,
                DisableContentMD5Validation = false,
                ParallelOperationThreadCount = DynamicClusterConfig.Instance.ConcurrentDownloads,
                MaximumExecutionTime = null,
                RetryPolicy = m_retryPolicy,
            };

            m_reqops_upload = new BlobRequestOptions
            {
                LocationMode = LocationMode.PrimaryOnly,
                AbsorbConditionalErrorsOnRetry = false,
                ParallelOperationThreadCount = DynamicClusterConfig.Instance.ConcurrentUploads,
                MaximumExecutionTime = null,
                RetryPolicy = m_retryPolicy,
                StoreBlobContentMD5 = true,
            };

            m_opctx = new OperationContext();
            m_cancel = cancel;
        }

        public Task CreateIfNotExistsAsync(CloudBlobContainer container) => container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, m_reqops_upload, m_opctx, m_cancel);

        public async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(CloudBlobDirectory directory, int? maxItems = null, bool flat = true)
        {
            var ret = new List<IListBlobItem>();
            BlobContinuationToken token = null;
            do
            {
                var result = await directory.ListBlobsSegmentedAsync(
                useFlatBlobListing: flat,
                blobListingDetails: BlobListingDetails.None,
                maxResults: maxItems,
                currentToken: token,
                options: m_reqops_download,
                operationContext: m_opctx,
                cancellationToken: m_cancel);
                ret.AddRange(result.Results);
                token = result.ContinuationToken;
                if (ret.Count >= (maxItems ?? int.MaxValue)) break;
            } while (token != null);
            return ret;
        }

        public Task<string> DownloadTextAsync(CloudBlockBlob file) =>
#if CORECLR
            file.DownloadTextAsync(Encoding.UTF8, new AccessCondition(), m_reqops_download, m_opctx, m_cancel);
#else
            file.DownloadTextAsync(m_cancel);
#endif

        public async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(CloudBlobContainer container)
        {
            var ret = new List<IListBlobItem>();
            BlobContinuationToken token = null;
            do
            {
                var result = await container.ListBlobsSegmentedAsync(
                prefix: "",
                useFlatBlobListing: true,
                blobListingDetails: BlobListingDetails.None,
                currentToken: token,
                options: m_reqops_download,
                maxResults: null,
                operationContext: m_opctx,
                cancellationToken: m_cancel);
                ret.AddRange(result.Results);
                token = result.ContinuationToken;
            } while (token != null);
            return ret;
        }

        internal Task UploadTextAsync(CloudBlockBlob file, string text) =>
            file.UploadTextAsync(text, Encoding.UTF8, new AccessCondition(), m_reqops_upload, m_opctx, m_cancel);

        public Task<bool> ExistsAsync(CloudBlob file) => file.ExistsAsync(m_reqops_download, m_opctx, m_cancel);

        public Task UploadDataAsync(CloudBlockBlob blob, byte[] data) =>
#if CORECLR
            blob.UploadFromByteArrayAsync(data, 0, data.Length, new AccessCondition(), m_reqops_upload, m_opctx, m_cancel);
#else
            blob.UploadFromByteArrayAsync(data, 0, data.Length, cancellationToken: m_cancel);
#endif

        public async Task DeleteAsync(CloudBlob file)
        {
#if CORECLR
            await file.DeleteIfExistsAsync(DeleteSnapshotsOption.None, new AccessCondition(), m_reqops_upload, m_opctx, m_cancel);
#else
            await file.DeleteIfExistsAsync(m_cancel);
#endif
        }
    }
}
