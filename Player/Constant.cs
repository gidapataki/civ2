

using System.Runtime.ExceptionServices;

namespace CivPlayer
{

	public class Stats
	{
		public int Damage { get; set; }
		public int Hp { get; set; }
		public int Attack { get; set; }
		public int Defense { get; set; }
		public int Movement { get; set; }

		public Stats(int dmg, int hp, int att, int def, int mp)
		{
			Damage = dmg;
			Hp = hp;
			Attack = att;
			Defense = def;
			Movement = mp;
		}
	}

	public class Constant
	{
		public const int ColonyCost = 300;
		public const int BoardSize = 20;


		public static double ChanceToHit(int attack, int defense)
		{
			return attack/(double) (attack + defense);
		}
		
		public static Stats UnitStats(UnitType type)
		{
			switch (type)
			{
				case UnitType.Felderito: return new Stats(2, 10, 2, 3, 4);
				case UnitType.Orzo: return new Stats(5, 30, 6, 10, 1);
				case UnitType.Lovag: return new Stats(15, 20, 12, 8, 1);
				case UnitType.Tanonc: return new Stats(10, 10, 6, 6, 2);
				case UnitType.Mester: return new Stats(10, 10, 100, 6, 2);
				default: return new Stats(0, 0, 0, 0, 0);
			}
		}

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
	}
}