using System;
using System.Linq;
using CivSharp.Common;

namespace CivPlayer
{
	public class Battle
	{
		private Battallion[] attackers;
		private Battallion[] attackNext;
		private Battallion[] defenders;

		private Battle(WorldInfo world, UnitInfo[] att, Position def)
		{
			defenders = world.Units.Where(u => u.HitPoints > 0 && Position.Of((UnitInfo) u).Equals(def)).Select(u => new Battallion(u.GetUnitType(), u.HitPoints)).ToArray();
			attackers = att.Select(u => new Battallion(u.GetUnitType(), u.HitPoints, Position.Of(u).Distance(def))).ToArray();
			attackNext = new Battallion[] { };
		}

		public static bool WillDefendersSurvive(WorldInfo world, UnitInfo[] att, Position def, UnitType[] planned)
		{
			var battle = new Battle(world, att, def);
			battle.defenders = battle.defenders.Concat(planned.Select(u => new Battallion(u, Unit.Stats(u).Hp))).ToArray();

			while (battle.attackers.Any())
			{
				while (battle.attackers.Any() && battle.defenders.Any())
				{
					battle.Fight();
				}
				battle.NextTurn();
			}
			return battle.defenders.Any();
		}

		private Tuple<double, double> ExpectedInjury(Battallion att, Battallion def)
		{
			var chance = Unit.ChanceToHit(att.Attack, def.Defense);
			return Tuple.Create((1 - chance)*def.Damage, chance*att.Damage);
		}

		private void NextTurn()
		{
			foreach (var u in defenders)
			{
				u.NextTurn();
			}

			attackers = attackNext;
			attackNext = new Battallion[] { };

			foreach (var u in attackers)
			{
				u.NextTurn();
			}
		}


		private void Fight()
		{
			// assert not empty
			var def = defenders.OrderBy(u => - u.Defense*100 - u.Hp).First();
			var att = attackers.OrderBy(u => - u.Attack).First();
			var inj = ExpectedInjury(att, def);
			att.Hp -= inj.Item1;
			def.Hp -= inj.Item2;
			att.Movement -= 1;

			attackNext = attackNext.Concat(attackers.Where(u => u.Movement == 0 && u.Hp > 0)).ToArray();
			attackers = attackers.Where(u => u.Movement > 0 && u.Hp > 0).ToArray();
			defenders = defenders.Where(u => u.Hp > 0).ToArray();
		}
	}
}