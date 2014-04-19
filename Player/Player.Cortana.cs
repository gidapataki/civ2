using System;
using System.Drawing;
using CivSharp.Common;
using System.Linq;


namespace CivPlayer
{
	public class Cortana : Player, IPlayer
	{
		public override string PlayerName // API
		{
			get { return "Cortana"; }
		}
	}
}
