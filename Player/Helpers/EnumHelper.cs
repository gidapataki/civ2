using CivPlayer.Enums;
using CivSharp.Common;
using System;
using System.ComponentModel;
using System.Linq;

namespace CivPlayer.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

		public static UnitType GetUnitType(this UnitInfo unit)	// should be a helper somewhere else
		{
			return Enum.GetValues(typeof(UnitType)).Cast<UnitType>().FirstOrDefault(type => type.GetDescription() == unit.UnitTypeName);
		}
    }
}
