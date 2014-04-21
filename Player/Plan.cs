using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;

namespace CivPlayer
{

	public class Plan
	{
		public List<Tuple<UnitType, Position>> TrainingList { get; private set; }
		public List<Position> BuildList { get; private set; }
		public List<ResearchType> ResearchList { get; private set; }
		public List<Tuple<string, Position>> MovementList { get; private set; }

		private List<Tuple<UnitType, Position>> TrainingList2;
		private List<Position> BuildList2;
		private List<ResearchType> ResearchList2;

		public Plan()
		{
			ResearchList = new List<ResearchType>();
			BuildList = new List<Position>();
			TrainingList = new List<Tuple<UnitType, Position>>();
			MovementList = new List<Tuple<string, Position>>();

			ResearchList2 = new List<ResearchType>();
			BuildList2 = new List<Position>();
			TrainingList2 = new List<Tuple<UnitType, Position>>();
		}

		public void Bump()
		{
			ResearchList = ResearchList2;
			BuildList = BuildList2;
			TrainingList = TrainingList2;
			MovementList = new List<Tuple<string, Position>>();

			ResearchList2 = new List<ResearchType>();
			BuildList2 = new List<Position>();
			TrainingList2 = new List<Tuple<UnitType, Position>>();
		}


		public void Want(ResearchType research)
		{
			if (!ResearchList.Contains(research))
				ResearchList.Add(research);
		}

		public void Want(UnitType type, Position pos, int count = 1)
		{
			for (; count > 0; --count)
				TrainingList.Add(Tuple.Create(type, pos));
		}

		public void Want(UnitInfo unit, Position pos)
		{
			var p = Position.Of(unit);
			while (!p.Equals(pos))
			{
				var step = p.Closer(pos);
				MovementList.Add(Tuple.Create(unit.UnitID, step));
				p = step;
			}
		}

		public void WantBuild(Position pos)
		{
			BuildList.Add(pos);
		}

		public void WantNext(ResearchType research)
		{
			ResearchList.Add(research);
		}

		public void WantNext(UnitType type, Position pos, int count = 1)
		{
			for (; count > 0; --count)
				TrainingList.Add(Tuple.Create(type, pos));
		}

		public void WantBuildNext(Position pos)
		{
			BuildList.Add(pos);
		}

		public int PlanCost
		{
			get
			{
				return TrainingList.Sum(p => Constant.UnitCost(p.Item1)) +
				       ResearchList.Sum(p => Constant.ResearchCost(p)) +
				       BuildList.Count()*Constant.ColonyCost;
			}
		}
	}
}

