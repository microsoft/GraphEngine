using System;
using System.Collections.Immutable;
using Reactive.Bindings;

namespace IKW.RDFTriple.DemoData
{
    public class ReactiveRDFStatementCollection
    {
        // Backing store for our sample RDF Triple collection
        private ImmutableList<RDFTriple>.Builder _rdfTripleNodeList;
        private ReactiveCollection<RDFTriple> _rdfTriples;
        private ReactiveCommand<RDFTriple> fetchRDFTripleCommand;
        private ReactiveCommand<RDFTriple> storeRDFTripleCommand;

        public ReactiveRDFStatementCollection()
        {
            _rdfTripleNodeList    = ImmutableList.CreateBuilder<RDFTriple>();
            _rdfTriples           = new ReactiveCollection<RDFTriple>();
            fetchRDFTripleCommand = new ReactiveCommand<RDFTriple>();
            storeRDFTripleCommand = new ReactiveCommand<RDFTriple>();




        }
    }
}
