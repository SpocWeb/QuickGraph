using System;
using System.Collections.Generic;

namespace QuickGraph.Petri
{
    [Serializable]
    public sealed class PetriNetSimulator<Token>
    {
		private IPetriNet<Token> net;
        private Dictionary<ITransition<Token>, TransitionBuffer> transitionBuffers = new Dictionary<ITransition<Token>, TransitionBuffer>();

        public PetriNetSimulator(IPetriNet<Token> net)
		{
            if (net == null)
                throw new ArgumentNullException("net");
            this.net = net;
		}

		public IPetriNet<Token> Net
		{
			get
			{
				return net;
			}
		}

		public void Initialize()
		{
			transitionBuffers.Clear();
			foreach(ITransition<Token> tr in Net.Transitions)
			{
				transitionBuffers.Add(tr, new TransitionBuffer());
			}
		}

		public void SimulateStep()
		{
			// first step, iterate over arc and gather tokens in transitions
			foreach(IArc<Token> arc in Net.Arcs)
			{
				if(!arc.IsInputArc)
					continue;

				IList<Token> tokens = transitionBuffers[arc.Transition].Tokens;
				// get annotated tokens
                IList<Token> annotatedTokens = arc.Annotation.Eval(arc.Place.Marking);
                //add annontated tokens
                foreach(Token annotatedToken in annotatedTokens)
                    tokens.Add(annotatedToken);
            }

			// second step, see which transition was enabled
			foreach(ITransition<Token> tr in Net.Transitions)
			{
				// get buffered tokens
                IList<Token> tokens = transitionBuffers[tr].Tokens;
                // check if enabled, store value
                transitionBuffers[tr].Enabled = tr.Condition.IsEnabled(tokens);
            }

			// third step, iterate over the arcs
			foreach(IArc<Token> arc in Net.Arcs)
			{
				if (!transitionBuffers[arc.Transition].Enabled)
					continue;

				if(arc.IsInputArc)
				{
					// get annotated tokens
                    IList<Token> annotatedTokens = arc.Annotation.Eval(arc.Place.Marking);
                    // remove annotated comments from source place
                    foreach(Token annotatedToken in annotatedTokens)
                        arc.Place.Marking.Remove(annotatedToken);
                }
				else
				{
                    IList<Token> tokens = transitionBuffers[arc.Transition].Tokens;
                    // get annotated tokens
                    IList<Token> annotatedTokens = arc.Annotation.Eval(tokens);
                    // IList<Token> annotated comments to target place
                    foreach(Token annotatedToken in annotatedTokens)
                        arc.Place.Marking.Add(annotatedToken);
                }
			}
			// step four, clear buffers
			foreach(ITransition<Token> tr in Net.Transitions)
			{
				transitionBuffers[tr].Tokens.Clear();
                transitionBuffers[tr].Enabled = false;
            }
		}

        private sealed class TransitionBuffer
        {
            private readonly IList<Token> tokens = new List<Token>();
            private bool enabled = true;

            public IList<Token> Tokens
            {
                get { return tokens;}
            }
            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
            }
        }
    }
}
