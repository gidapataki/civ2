using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;

namespace CivPlayer
{

	public class TrackedUnit
	{
		public UnitInfo unit { get; set; }

		public TrackedUnit()
		{
			this.unit = null;
		}

		public void Refresh(WorldInfo world)
		{
			if (this.unit != null)
			{
				foreach (var unit in world.Units.Where(unit => unit.UnitID == this.unit.UnitID))
				{
					this.unit = unit.HitPoints >= 0 ? unit : null;
					break;
				}
			}
		}
		
		public static implicit operator UnitInfo(TrackedUnit unit)
		{
			return unit.unit;
		}

		public static implicit operator bool(TrackedUnit unit)
		{
			return unit != null && unit.unit != null;
		}
	}



}
