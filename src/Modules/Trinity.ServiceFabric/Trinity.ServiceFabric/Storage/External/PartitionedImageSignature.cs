using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.ServiceFabric.Storage.External
{
    public class ImagePartitionSignature
    {
        public int PartitionId { get; set; }

        /// <summary>
        /// Gets or sets the version of the image partition.
        /// A null version means the partition does not exist.
        /// </summary>
        public ulong? Version { get; set; } = null;

        public string Signature { get; set; }
    }

    public class PartitionedImageSignature
    {
        private Dictionary<int, ImagePartitionSignature> signatures;

        /// <summary>
        /// Checks if the partitioned image is valid.
        /// A partitioned image is valid if and only if versions of all image partitions are identical.
        /// </summary>
        public bool IsValid => signatures.Values.All(s => s.Version == signatures.Values.First().Version);

        /// <summary>
        /// Gets the version of the partitioned image.
        /// A null version means the image is not valid or
        /// empty (versions of image partitions are all null).
        /// </summary>
        public ulong? Version => IsValid ? signatures.Values.First().Version : null;

        public ulong NextVersion => IsValid && Version != null ? Version.Value + 1 : 0;

        public PartitionedImageSignature(IEnumerable<ImagePartitionSignature> signatures)
        {
            if (signatures == null || signatures.Count() == 0)
                throw new ArgumentException("Null or empty signatures");

            this.signatures = new Dictionary<int, ImagePartitionSignature>();
            foreach (var sig in signatures)
            {
                if (sig == null)
                    throw new ArgumentNullException("Null image partition signature");
                this.signatures.Add(sig.PartitionId, sig);
            }
        }

        public void Update(ulong nextVersion, IDictionary<int, string> dict)
        {
            lock(signatures)
            {
                if (Version != null && nextVersion <= Version.Value)
                    throw new ArgumentException($"Invalid new version number {nextVersion}.");

                // update versions for all image partition signatures,
                // and update the signature strings for those partitions in the dict.
                foreach (var kv in signatures)
                {
                    kv.Value.Version = nextVersion;

                    if (dict.ContainsKey(kv.Key))
                        kv.Value.Signature = dict[kv.Key];
                }
            }
        }

        public ImagePartitionSignature GetPartitionSignature(int partition)
        {
            lock(signatures)
            {
                return signatures[partition];
            }
        }
    }
}
