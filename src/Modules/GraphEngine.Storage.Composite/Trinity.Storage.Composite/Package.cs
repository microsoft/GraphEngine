namespace Trinity.Storage.CompositeExtension
{
    public class Package
    {
        public string Name { get; }
        public string Version { get; }

        public Package(string packageName, string packageVersion)
        {
            Name = packageName;
            Version = packageVersion;
        }
    }
}