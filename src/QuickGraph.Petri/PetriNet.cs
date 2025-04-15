using System;
using System.Collections.Generic;
using System.IO;

namespace QuickGraph.Petri
{
    [Serializable]
    public sealed class PetriNet<Token> 
        : IMutablePetriNet<Token>
        , ICloneable
    {
        private readonly List<IPlace<Token>> places = new List<IPlace<Token>>();
        private readonly List<ITransition<Token>> transitions = new List<ITransition<Token>>();
        private readonly List<IArc<Token>> arcs = new List<IArc<Token>>();
        private readonly PetriGraph<Token> graph = new PetriGraph<Token>();      

		public PetriNet()
		{}

        private PetriNet(PetriNet<Token> other)
        {
            places.AddRange(other.places);
            transitions.AddRange(other.transitions);
            arcs.AddRange(other.arcs);
            graph = new PetriGraph<Token>();
            graph.AddVerticesAndEdgeRange(other.graph.Edges);
        }

		public IPetriGraph<Token> Graph => graph;

        public IPlace<Token> AddPlace(string name)
		{
			IPlace<Token> p = new Place<Token>(name);
			places.Add(p);
			graph.AddVertex(p);
			return p;
		}
		public ITransition<Token> AddTransition(string name)
		{
			ITransition<Token> tr = new Transition<Token>(name);
			transitions.Add(tr);
			graph.AddVertex(tr);
			return tr;
		}
		public IArc<Token> AddArc(IPlace<Token> place, ITransition<Token> transition)
		{
            IArc<Token> arc = new Arc<Token>(place, transition);
            arcs.Add(arc);
			graph.AddEdge(arc);
			return arc;
		}
		public IArc<Token> AddArc(ITransition<Token> transition,IPlace<Token> place)
		{
			IArc<Token> arc=new Arc<Token>(transition,place);
			arcs.Add(arc);
			graph.AddEdge(arc);
			return arc;
		}

		public IList<IPlace<Token>> Places => places;

        public IList<ITransition<Token>> Transitions => transitions;

        public IList<IArc<Token>> Arcs => arcs;

        public override string ToString()
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine("-----------------------------------------------");
			sw.WriteLine("Places ({0})",places.Count);
            foreach (IPlace<Token> place in places)
            {
				sw.WriteLine("\t{0}",place.ToStringWithMarking());
			}

			sw.WriteLine("Transitions ({0})",transitions.Count);
            foreach (ITransition<Token> transition in transitions)
            {
				sw.WriteLine("\t{0}",transition);
			}

			sw.WriteLine("Arcs");
            foreach (IArc<Token> arc in arcs)
            {
				sw.WriteLine("\t{0}",arc);
			}
			return sw.ToString();
		}


        public PetriNet<Token> Clone()
        {
            return new PetriNet<Token>(this);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

	}
}
