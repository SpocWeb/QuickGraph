using System;

namespace QuickGraph.Petri
{
    [Serializable]
    internal sealed class Transition<Token> : ITransition<Token>
    {
		private string name;
		private IConditionExpression<Token> condition = new AllwaysTrueConditionExpression<Token>();

		public Transition(string name)
		{this.name=name;}

		public IConditionExpression<Token> Condition
		{
			get => condition;
            set => condition=value;
        }

		public string Name => name;

        public override string ToString()
		{
			return string.Format("T({0})",name);
		}
	}
}
