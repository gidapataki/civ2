using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;

namespace CivPlayer
{
	class RushStrategy : Strategy
	{

		public RushStrategy(Player player)
			: base(player)
		{}

		public override void PlotMasterPlan()
		{
			//if (HasBudgetFor(Felderito: 2))
			//plan.Want(UnitType.Felderito, Position.Of(
			//(pos => EnemyDistance(pos)).First()), 1);

			if (HasResearch(ResearchType.Barakk))
				plan.Want(UnitType.Tanonc, Position.Of(SortMyCities(pos => -EnemyDistance(pos)).First()), 1);
			else
				plan.Want(ResearchType.Barakk);
			

			// attack:
			foreach (var unit in player.MyUnits)
			{
				var target = FindUnitTarget(unit, pos =>
					 - EnemyDistance(pos)
					+ (IsEnemyCity(pos) ? 100 : 0)
					+ (IsEnemyUnitAt(pos) ? -10 : 0)
				);

				plan.Want(unit, target);
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
			}
		}


		private TrainingData TrainIfNoUnits(UnitType type)
		{
			if (!player.MyUnits.Any() && CanTrain(type))
			{
				return Train(player.MyCities.First(), type);
			}
			return null;
		}


	}
}
