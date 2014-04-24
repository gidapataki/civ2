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

		private bool rush = false;
		private bool mester = false;

		public RushStrategy(Player player)
			: base(player)
		{}

		public override void PlotMasterPlan()
		{
			var trainAt = Position.Of(SortMyCities(pos => -EnemyDistance(pos)).First());

			//plan.Want(UnitType.Felderito, trainAt);
			//rush = true;

			//if (NumberOfUnits == 0)
			//{
			//	plan.Want(UnitType.Felderito, Position.Of(player.MyCities.First()));
			//}
			//else if (CanColonize())
			//{
			//	var builder = FindUnit(unit => UnitWeight.Set(Felderito: 5).Get(unit));

			//	if (builder != null)
			//	{
			//		var bpos = Position.Of(builder);
			//		var where = FindUnitTarget(builder, pos =>
			//			+ (IsCity(pos) ? -100 : 0)
			//			+ (IsEnemyUnitAt(pos) ? -100 : 0)
			//			- pos.CornerDistance()*10
			//			- pos.EdgeDistance()*5
			//			+ (bpos.x == pos.x ? 2 : 0)
			//			- pos.Distance(bpos)
			//			);

			//		plan.Want(builder, where);
			//		plan.WantBuild(where);
			//	}
			//}

			//if (NumberOfCities >= 4)
			//{
			//	if (HasBudgetFor(Falu: 1, Varoshaza: 1) && !HasResearch(ResearchType.Varoshaza))
			//	{
			//		plan.Want(ResearchType.Falu);
			//		plan.Want(ResearchType.Varoshaza);
			//	}
			//	if (HasBudgetFor(Varos: 1, Bank: 1) && !HasResearch(ResearchType.Bank))
			//	{
			//		plan.Want(ResearchType.Varos);
			//		plan.Want(ResearchType.Bank);
			//	}
			//}



			//if (!HasResearch(ResearchType.Kovacsmuhely))
			//	plan.Want(ResearchType.Kovacsmuhely);

			//if (HasResearch(ResearchType.Kovacsmuhely) && HasBudgetFor(Lovag: 10) || rush)
			//{
			////	mester = true;
			//	plan.Want(UnitType.Lovag, trainAt, 10);
			//	rush = true;
			//}

			if (!HasResearch(ResearchType.Barakk))
			{
				plan.Want(ResearchType.Barakk);
			}
			else
			{
				if (HasBudgetFor(Tanonc: 1))
				{
					rush = true;
				}
				if (rush)
				{
					plan.Want(UnitType.Tanonc, trainAt, 1);
				}
			}


			//if (HasResearch(ResearchType.HarciAkademia))
			//{
			//	if (HasBudgetFor(Mester: 19))
			//	{
			//		rush = true;
			//	}
			//	if (rush)
			//	{
			//		plan.Want(UnitType.Mester, trainAt, 19);
			//	}
			//}
			//else
			//{
			//	plan.Want(ResearchType.HarciAkademia);
			//}


			// attack:
			foreach (var unit in player.MyUnits)
			{
				var target = FindUnitTarget(unit, pos =>
					 - EnemyDistance(pos)
					 + (pos.x != 0 ? 1 : 0)
					+ (IsEnemyCity(pos) ? 100 : 0)
					+ (IsEnemyUnitAt(pos) ? -10 : 0)
				);
	
				if (unit.GetUnitType() != UnitType.Felderito || rush)
					plan.Want(unit, target);
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
