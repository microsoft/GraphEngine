namespace Trinity.Storage.Composite
{
    public class SchemaUpdate
    {
        public SchemaUpdate() { }

        public void LoadFromTsl(string file) { }
        public void AddOrUpdateType(ICellDescriptor type) { }
        public void AddOrUpdateField(string type, IFieldDescriptor field) { }
        public void AddOrUpdateAttribute(string type, string key, string value) { }
        public void DeleteType(string type) { }
        public void DeleteField(string type, string field) { }
        public void DeleteAttribute(string type, string key) { }
    }
}
