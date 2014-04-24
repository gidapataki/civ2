using System;

namespace CivPlayer
{
	public class Battallion
	{
		public int Attack { get; set; }
		public int Defense { get; set; }
		public int Damage { get; set; }
		public double Hp { get; set; }
		public int Movement { get; set; }
		private double maxHp;
		private double heal;
		private int maxMovement;

		public Battallion(UnitType type, int hp, int dist = 1)
		{
			var stats = Unit.Stats(type);
			Attack = stats.Attack;
			Defense = stats.Defense;
			Hp = hp;
			Movement = stats.Movement - (dist - 1);
			Damage = stats.Damage;
			maxHp = stats.Hp;
			maxMovement = stats.Movement;
			heal = stats.Heal;
		}

		public void NextTurn()
		{
			Hp = Math.Min(Hp + heal, maxHp);
			Movement = maxMovement;
		}
	}
}