using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;

namespace CivPlayer
{

	
	class UnicornStrategy : Strategy
	{

		public UnicornStrategy(Player player)
			: base(player)
		{}


		public override void PlotMasterPlan()
		{
			var attack = false;

			foreach (var city in SortMyCities(pos => -EnemyDistance(pos)))
			{
				var cpos = Position.Of(city);
				foreach (var enemy in player.EnemyUnits)
				{
					if (TravelRounds(enemy, cpos) <= 1)
					{
						attack = true;
						if (ChanceToLose(cpos) > 0.5)
						{
							var success = DefendCity(city, UnitType.Lovag);
							if (!success && !IsDefendedBy(city, UnitType.Felderito) && HasBudgetFor(Felderito: 1))
							{
								plan.Want(UnitType.Felderito, cpos);
							}
						}
						break;
					}
				}
			}
			// colonize and defend!

			if (attack)
			{

			}
			else
			{
				if (BeforeCanColonize() && NumberOfUnits == 0)
				{
					// check if deposit needed
					// TravelRounds(unitinfo, pos)
					plan.Want(UnitType.Felderito, Position.Of(player.MyCities.First()));
				}

				if (CanColonize(NumberOfCities > 1 ? 150 - Income : 0))
				{
					var builder = FindUnit(unit => UnitWeight.Set(Felderito: 5).Get(unit));

					if (builder != null)
					{
						var bpos = Position.Of(builder);
						var where = FindUnitTarget(builder, pos =>
							+ (IsCity(pos) ? -100 : 0)
							+ (IsEnemyUnitAt(pos) ? -100 : 0)
							- pos.CornerDistance()*10
							- pos.EdgeDistance()*5
							+ (bpos.x == pos.x ? 2 : 0)
							- pos.Distance(bpos)
							);

						plan.Want(builder, where);
						plan.WantBuild(where);
					}
				}
			}

			if (NumberOfCities == 4)
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
			}
		}

	}
}
