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
			foreach (var city in player.MyCities)
			{
				var cpos = Position.Of(city);
				foreach (var enemy in player.EnemyUnits)
				{
					if (TravelRounds(enemy, cpos) == 1 && !IsDefended(city))
					{
						if (!HasResearch(ResearchType.OrzokTornya) && HasMoneyFor(OrzokTornya: 1, Orzo: 1))
						{
							plan.Want(ResearchType.OrzokTornya);
							plan.Want(UnitType.Orzo, cpos);
						}
						else if (HasResearch(ResearchType.OrzokTornya) && HasMoneyFor(Orzo: 1))
						{
							plan.Want(UnitType.Orzo, cpos);
						}
						else if (HasMoneyFor(Felderito: 1))
						{
							plan.Want(UnitType.Felderito, cpos);
						}
					}
				}
			}

			if (BeforeCanColonize() && NumberOfUnits == 0) 
			{ 
				// check if deposit needed
				// TravelRounds(unitinfo, pos)
				plan.Want(UnitType.Felderito, Position.Of(player.MyCities.First()));
			}

			if (CanColonize())
			{
				var builder = FindUnit(unit => 0);
				
				if (builder != null)
				{
					var bpos = Position.Of(builder);
					var where = FindUnitTarget(builder, pos =>
						+ (IsCity(pos) ? -100 : 0)
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

			if (NumberOfCities == 4)
			{
				if (HasMoneyFor(Falu: 1, Varoshaza: 1) && !HasResearch(ResearchType.Varoshaza))
				{
					plan.Want(ResearchType.Falu);
					plan.Want(ResearchType.Varoshaza);
				}
				if (HasMoneyFor(Varos: 1, Bank: 1) && !HasResearch(ResearchType.Bank))
				{
					plan.Want(ResearchType.Varos);
					plan.Want(ResearchType.Bank);
				}
			}
		}

	}
}
