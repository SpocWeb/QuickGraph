using System;
using System.Collections.Generic;
using System.IO;

namespace QuickGraph.Petri
{
    [Serializable]
    internal sealed class Place<Token> : IPlace<Token>
    {
		private string name;
		private IList<Token> marking = new List<Token>();

		public Place(string name)
		{
			this.name=name;
		}

		public IList<Token> Marking
		{
			get
			{
				return marking;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string ToStringWithMarking()
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine(ToString());
			foreach(object token in marking)
				sw.WriteLine("\t{0}",token.GetType().Name);

			return sw.ToString();

		}
		public override string ToString()
		{
			return string.Format("P({0}|{1})",name,marking.Count);
		}
	}
}
