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
		{
			// rush please
			useFallback = false;
		}


		private Position BasePosition
		{
			get { return Position.Of(SortMyCities(pos => EnemyDistance(pos)).First());  }
		}


		private void DoFarming()
		{
			if (NumberOfUnits == 0)
			{
				plan.Want(UnitType.Felderito, BasePosition);
			}
			else if (CanColonize())
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
			}
		}

		private Position BattleCity
		{
			get { return Position.Of(SortMyCities(pos => -EnemyDistance(pos)).First());	}
		}

		private void NeedUnitRequirements(UnitType type)
		{
			var req = Unit.Requirement(type);
			while (!HasResearch(Research.Requirement(req)))
			{
				req = Research.Requirement(req);
			}

			if (!HasResearch(req))
			{
				plan.Want(req);
			}
		}

		private bool NeedUnit(UnitType type, Position pos, int n = 1, bool startRush = false)
		{
			NeedUnitRequirements(type);
			if (HasResearch(Unit.Requirement(type)) && HasBudget(Unit.Cost(type)*n))
			{
				plan.Want(type, pos, n);
				rush |= startRush;
				return true;
			}
			return false;
		}

		public override void PlotMasterPlan()
		{
			DoFarming();
			NeedUnit(UnitType.Tanonc, BattleCity, 1, true);
			//if (!rush)
			//{
			//}
			//else
			//{
			//	NeedUnit(UnitType.Mester, BattleCity, 1);
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
				return TrainingDataFor(player.MyCities.First(), type);
			}
			return null;
		}


	}
}
