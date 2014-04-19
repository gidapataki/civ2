using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;


namespace CivPlayer
{
	public class Rikku : Player, IPlayer
	{
		public override string PlayerName // API
		{
			get { return "Rikku"; }
		}
	}
}
