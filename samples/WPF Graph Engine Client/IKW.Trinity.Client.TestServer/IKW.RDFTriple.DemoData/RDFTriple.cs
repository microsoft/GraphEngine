namespace IKW.RDFTriple.DemoData
{
    // Very simple RDF model - Not production - testing only

    internal class RDFTriple
    {
        private string _subjectNode;
        private string _predicateNode;
        private string _objectNode;

        public RDFTriple(string rdfSubject,
                             string rdfPredicate,
                             string rdfObject)
        {
            _subjectNode   = rdfSubject;
            _predicateNode = rdfPredicate;
            _objectNode    = rdfObject;
        }

        public string SubjectNode
        {
            get => _subjectNode;
            set => _subjectNode = value;
        }

        public string PredicateNode
        {
            get => _predicateNode;
            set => _predicateNode = value;
        }

        public string ObjectNode
        {
            get => _objectNode;
            set => _objectNode = value;
        }
    }
}