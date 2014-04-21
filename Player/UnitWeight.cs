using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;

namespace CivPlayer
{

	// hp * v   30 


	// IsDefendedBy(pos, unittype


	public class UnitWeight
	{
		public int Felderito { get; set; }
		public int Lovag { get; set; }
		public int Orzo { get; set; }
		public int Tanonc { get; set; }
		public int Mester { get; set; }

		public UnitWeight(int F, int L, int O, int T, int M)
		{
			Felderito = F;
			Lovag = L;
			Orzo = O;
			Tanonc = T;
			Mester = M;
		}

		public static UnitWeight Set(int Felderito = 0, int Lovag = 0, int Orzo = 0, int Tanonc = 0, int Mester = 0)
		{
			return new UnitWeight(Felderito, Lovag, Orzo, Tanonc, Mester);
		}

		public int Get(UnitType type)
		{
			switch (type)
			{
				case UnitType.Felderito: return Felderito;
				case UnitType.Lovag: return Lovag;
				case UnitType.Orzo: return Orzo;
				case UnitType.Tanonc: return Tanonc;
				case UnitType.Mester: return Mester;
				default: return 0;
			}
		}

		public int Get(UnitInfo unit)
		{
			return Get(unit.GetUnitType());
		}
	}

}
