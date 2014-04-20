using System.ComponentModel;

namespace CivPlayer.Enums
{
	public enum UnitType
	{
		[Description("")]
		None,

		[Description("felderítő")]
		Felderito,

		[Description("lovag")]
		Lovag,

		[Description("őrző")]
		Orzo,

		[Description("vívó tanonc")]
		Tanonc,
	
		[Description("vívó mester")]
		Mester
	}
}
