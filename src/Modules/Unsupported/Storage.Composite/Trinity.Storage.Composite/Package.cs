namespace Trinity.Storage.Composite
{
    internal class Package
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
