using System.IO;

namespace Trinity.Azure.Storage.Test
{
    public static class ConfigInit
    {
        public static void Init()
        {
            BlobStorageConfig.Instance.ConnectionString = File.ReadAllText("key.txt");
            BlobStorageConfig.Instance.ContainerName = "testcontainer";
        }
    }
}
