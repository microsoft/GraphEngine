using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.ServiceFabric.Diagnostics;

namespace Trinity.ServiceFabric.Storage.External
{
    public class PartitionedImage : ITrinityStorageImage
    {
        public const int DEFAULT_IMAGE_SLOTS = 2;

        public IEnumerable<int> AllPartitions => Enumerable.Range(0, Global.CloudStorage.GetTotalPartitionCount());
        public IEnumerable<int> LocalPartitions => AllPartitions.Where(p => Global.CloudStorage.IsLocalPartition(p));

        private IPartitionedImageStorage[] ImageSlots { get; set; }
        private PartitionedImageSignature[] ImageSignatures { get; set; }
        private int CurrentSlotIndex { get; set; } = int.MinValue;
        private int NextSlotIndex => (CurrentSlotIndex + 1) % ImageSlots.Length;
        private IPartitionedImageStorage CurrentSlot => ImageSlots[CurrentSlotIndex];
        private IPartitionedImageStorage NextSlot => ImageSlots[NextSlotIndex];
        private PartitionedImageSignature CurrentSignature => UseImageSignature(CurrentSlotIndex);
        private PartitionedImageSignature NextSignature => UseImageSignature(NextSlotIndex);

        public PartitionedImage(Func<int, IPartitionedImageStorage> imageStorageFactory, int slots = DEFAULT_IMAGE_SLOTS)
        {
            this.ImageSlots = Enumerable.Range(0, slots).Select(slot => imageStorageFactory(slot)).ToArray();
        }

        public bool LoadLocalStorage()
        {
            try
            {
                // determine the current slot
                CalculateCurrentSlot();

                if (CurrentSignature.Version != null)
                {
                    // load all local image partitions
                    Log.Info("Loading image from storage slot[{0}]", CurrentSlotIndex);
                    var signatures = new ConcurrentDictionary<int, string>();
                    Parallel.ForEach(LocalPartitions, p => signatures.TryAdd(p, CurrentSlot.LoadImagePartition(p)));

                    // compare image partition signatures
                    if (!signatures.All(kv => kv.Value == CurrentSignature.GetPartitionSignature(kv.Key).Signature))
                    {
                        Log.Fatal("Failed to load storage: Image signature mismatch.");
                        return false;
                    }
                }
                else
                {
                    Log.Warn("Nothing has been loaded from the invalid or empty slot[{0}]", CurrentSlotIndex);
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Fatal("Failed to load storage: {0}", e);
                return false;
            }
        }

        public bool SaveLocalStorage()
        {
            try
            {
                CalculateCurrentSlot();

                // save all local partitions into the NEXT slot
                Log.Info("Saving image into storage slot[{0}]...", NextSlotIndex);
                var signatures = new ConcurrentDictionary<int, string>();
                Parallel.ForEach(LocalPartitions, p => signatures.TryAdd(p, NextSlot.SaveImagePartition(p)));

                // save local image partition signatures
                NextSignature.Update(CurrentSignature.NextVersion, signatures);
                Log.Info("Saving image signature into storage slot[{0}]...", NextSlotIndex);
                Parallel.ForEach(LocalPartitions, p => NextSlot.SavePartitionSignature(NextSignature.GetPartitionSignature(p)));

                // the next slot becomes the current slot
                CurrentSlotIndex = NextSlotIndex;

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Failed to save storage: {0}", e);
                return false;
            }
        }

        private void CalculateCurrentSlot()
        {
            var signatures = ImageSlots.Select((_, slotIndex) => new Tuple<int, PartitionedImageSignature>(slotIndex, UseImageSignature(slotIndex)));
            
            // the latest version can be null
            var latestVersion = signatures.Max(tuple => tuple.Item2.Version);
            var latestSlots = signatures.Where(tuple => tuple.Item2.Version == latestVersion);

            CurrentSlotIndex = latestSlots.Last().Item1;
        }

        private PartitionedImageSignature UseImageSignature(int slotIndex)
        {
            if (ImageSignatures == null)
            {
                Log.Info("Loading image signatures from storage slots[0..{0}]...", ImageSlots.Length - 1);
                ImageSignatures = ImageSlots.Select(slot =>
                {
                    var signatures = new ConcurrentQueue< ImagePartitionSignature>();
                    Parallel.ForEach(AllPartitions, p => signatures.Enqueue(slot.LoadPartitionSignature(p)));
                    return new PartitionedImageSignature(signatures);
                }).ToArray();
            }
            return ImageSignatures[slotIndex];
        }
    }
}
