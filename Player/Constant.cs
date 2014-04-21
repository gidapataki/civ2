

namespace CivPlayer
{
	public class Constant
	{
		public const int ColonyCost = 300;
		public const int BoardSize = 20;

		public static int UnitCost(UnitType type)
		{
			switch (type)
			{
				case UnitType.Felderito: return 50;
				case UnitType.Orzo: return 75;
				case UnitType.Lovag: return 150;
				case UnitType.Tanonc: return 100;
				case UnitType.Mester: return 200;
				default: return 0;
			}
		}

		public static int ResearchCost(ResearchType type)
		{
			switch (type)
			{
				case ResearchType.Falu: return 200;
				case ResearchType.Varos: return 300;
				case ResearchType.OrzokTornya: return 100;
				case ResearchType.Kovacsmuhely: return 150;
				case ResearchType.Barakk: return 200;
				case ResearchType.HarciAkademia: return 500;
				case ResearchType.Varoshaza: return 150;
				case ResearchType.Bank: return 300;
				case ResearchType.Barikad: return 100;
				case ResearchType.Fal: return 200;
				default: return 0;
			}
		}

		public static int MovementPoints(UnitType type)
		{
			switch (type)
			{
				case UnitType.Felderito: return 4;
				case UnitType.Orzo: return 1;
				case UnitType.Lovag: return 1;
				case UnitType.Tanonc: return 2;
				case UnitType.Mester: return 2;
				default: return 0;
			}
		}
	}
}