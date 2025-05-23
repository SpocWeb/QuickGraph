using System;

namespace QuickGraph.Petri
{
    [Serializable]
    internal sealed class Arc<Token> 
        : Edge<IPetriVertex>
        , IArc<Token>
	{
		private bool isInputArc;
		private IPlace<Token> place;
		private ITransition<Token> transition;
		private IExpression<Token> annotation = new IdentityExpression<Token>();

		public Arc(IPlace<Token> place, ITransition<Token> transition)
            :base(place,transition)
		{
			this.place=place;
			this.transition=transition;
			isInputArc=true;
		}
		public Arc(ITransition<Token> transition,IPlace<Token> place)
            :base(place,transition)
        {
            this.place=place;
			this.transition=transition;
			isInputArc=false;
		}

		public bool IsInputArc => isInputArc;

        public IPlace<Token> Place => place;

        public ITransition<Token> Transition => transition;

        public IExpression<Token> Annotation
		{
			get => annotation;
            set => annotation=value;
        }

		public override string ToString()
		{
			if (IsInputArc)
				return string.Format("{0} -> {1}",place,transition);
			else
				return string.Format("{0} -> {1}",transition,place);
		}
	}
}
