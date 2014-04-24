using CivSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivPlayer
{

	public class ColonizeStrategy : Strategy
	{

		private TrackedUnit builder;
		private Position location;

		public ColonizeStrategy(Player player)
			: base(player)
		{
			builder = new TrackedUnit();
		}

		public override TrainingData OnTraining()
		{
			// train.scout if no units
			if (!player.MyUnits.Any() && CanTrain(UnitType.Felderito))
			{
				return TrainingDataFor(player.MyCities[0], UnitType.Felderito);
			}
			return null;
		}

		public override void BeforeMovement()
		{
			// move.[somewhere uncolonized] if CanColonize()
			Track(builder, CanColonize()
				? FindUnit((UnitInfo unit) =>
					-Position.Of(unit).Distance(Position.Of(player.MyCities[0]))
					+ UnitWeight.Set(Felderito: 100).Get(unit)
				) : null);

			if (builder)
			{
				location = FindUnitTarget(builder, (Position pos) =>
					+(IsCity(pos) ? -100 : 0)
					+ random.Next(50)
					);
			}
		}

		public override MovementData OnMovement()
		{
			if (builder)
			{
				var pos = Position.Of(builder).Closer(location);
				if (CanMove(builder, pos)) { return Move(builder, pos); }
			}
			return null;
		}

		public override BuildingData OnBuilding()
		{
			if (builder && CanBuild(builder))
			{
				return BuildingDataFor(builder);
			}
			return null;
		}

		public override ResearchData OnResearch()
		{
			if (CanResearch(ResearchType.Falu))
				return ResearchDataFor(ResearchType.Falu);
			if (CanResearch(ResearchType.Varoshaza))
				return ResearchDataFor(ResearchType.Varoshaza);
			
			if (CanResearch(ResearchType.Varos))
				return ResearchDataFor(ResearchType.Varos);
			if (CanResearch(ResearchType.Bank))
				return ResearchDataFor(ResearchType.Bank);
			
			return null;
		}
	}
}
