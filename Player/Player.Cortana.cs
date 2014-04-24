using System;
using System.Drawing;
using CivSharp.Common;
using System.Linq;


namespace CivPlayer
{
	public class Cortana : Player, IPlayer
	{
		public Cortana()
		{
			strategy = new UnicornStrategy(this);
		}

		public override string PlayerName // API
		{
			get { return "omelette du fromage"; }
		}
	}
}
