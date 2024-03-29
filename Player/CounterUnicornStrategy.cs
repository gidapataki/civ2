﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;

namespace CivPlayer
{


	class CounterUnicornStrategy : Strategy
	{

		private bool needWall = false;
		private bool sentInel = false;
		private bool attack = false;
		private bool incoming = false;

		public CounterUnicornStrategy(Player player)
			: base(player)
		{
			useFallback = true;
		}


		public override void UnitLost(string unitID)
		{
			if (player.world.Units.Where(u => u.Owner == player.PlayerName && u.UnitID == unitID).Any())
			{
				incoming = true;
			}
		}
		

		public override void PlotMasterPlan()
		{
			var grow = true;

			foreach (var city in player.EnemyCities)
			{
				plan.Attack(Position.Of(city));
			}

			while (grow)
			{
				grow = false;
				foreach (var city in SortMyCities(pos => -EnemyDistance(pos)))
				{
					var cpos = Position.Of(city);
					foreach (var enemy in player.EnemyUnits)
					{
						if (TravelRounds(enemy, cpos) <= 1)
						{
							var threat = ChanceToLose(cpos);
							if (threat > 0.5)
							{
								var success = false;
								if (!IsDefendedBy(city, UnitType.Mester) && 
									IsDefendedBy(city, UnitType.Lovag) &&
									HasResearch(ResearchType.HarciAkademia))
								{
									success = DefendCity(city, UnitType.Mester);
								}
								if (!success)
								{
									success =  DefendCity(city, UnitType.Lovag);
								}
								if (success) { grow = true; }
								if (!success && !IsMyUnitAt(Position.Of(city)) && HasBudgetFor(Felderito: 1))
								//if (!success && !IsDefendedBy(city, UnitType.Felderito) && HasBudgetFor(Felderito: 1))
								{
									plan.Want(UnitType.Felderito, cpos);
								}
							}
							if (enemy.GetUnitType() != UnitType.Felderito)
							{
								plan.Attack(Position.Of(enemy));
							}
							break;
						}
					}
				}
			}

			if (BeforeCanColonize() && NumberOfUnits == 0)
			{
				plan.Want(UnitType.Felderito, Position.Of(player.MyCities.First()));
			}

			if (CanColonize(NumberOfCities > 1 ? 150 - Income : 0))
			{
				var builder = FindUnit(unit => UnitWeight.Set(Felderito: 5).Get(unit));

				if (builder != null)
				{
					var bpos = Position.Of(builder);
					var where = FindUnitTarget(builder, pos =>
						+(IsCity(pos) ? -100 : 0)
						+ (IsEnemyUnitAt(pos) ? -100 : 0)
						- pos.CornerDistance() * 10
						- pos.EdgeDistance() * 5
						+ (bpos.x == pos.x ? 2 : 0)
						- pos.Distance(bpos)
						);

					plan.Want(builder, where);
					plan.WantBuild(where);
				}
			}

			if (NumberOfCities >= 4)
			{
				if (HasBudgetFor(Falu: 1, Varoshaza: 1) && !HasResearch(ResearchType.Varoshaza))
				{
					plan.Want(ResearchType.Falu);
					plan.Want(ResearchType.Varoshaza);
				}
				if (HasBudgetFor(Varos: 1, Bank: 1) && !HasResearch(ResearchType.Bank))
				{
					plan.Want(ResearchType.Varos);
					plan.Want(ResearchType.Bank);
				}
				
				
				if (HasBudgetFor(Barakk: 1, HarciAkademia: 1) && !HasResearch(ResearchType.Barakk)
					&& HasResearch(ResearchType.Bank) && HasResearch(ResearchType.Kovacsmuhely))
				{
					plan.Want(ResearchType.Barakk);
					plan.Want(ResearchType.HarciAkademia);
				}

				
				if (HasBudgetFor(Barikad: 1) && !HasResearch(ResearchType.Barikad) && incoming
					) { plan.Want(ResearchType.Barikad); }
				if (HasBudgetFor(Fal: 1) && !HasResearch(ResearchType.Fal) && HasResearch(ResearchType.Barikad)) { plan.Want(ResearchType.Fal); }
			}


			if (player.Turn > 75 && player.EnemyMoney >= player.Money - Income)
			{
				attack = true;
			}

			if (attack)
			{
				foreach (var unit in player.MyUnits)
				{
					var target = FindUnitTarget(unit, pos =>
						-EnemyDistance(pos)
						+ (pos.x != 0 ? 1 : 0)
						+ (IsEnemyCity(pos) ? 100 : 0)
						+ (IsEnemyUnitAt(pos) ? -10 : 0)
						);

					if (unit.GetUnitType() == UnitType.Felderito)
					{
						sentInel = true;
						plan.Want(unit, target);
					}
				}
			}

		}
	}
}
