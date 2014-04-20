using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CivPlayer
{

	public enum Phase
	{
		Turn, Movement, Research, Building, Training
	}


	public enum ResearchType
	{
		[Description("")]
		None,

		[Description("falu")]
		Falu,

		[Description("város")]
		Varos,

		[Description("őrzők tornya")]
		OrzokTornya,

		[Description("kovácsműhely")]
		Kovacsmuhely,

		[Description("barakk")]
		Barakk,

		[Description("harci akadémia")]
		HarciAkademia,

		[Description("városháza")]
		Varoshaza,

		[Description("bank")]
		Bank,

		[Description("barikád")]
		Barikad,

		[Description("fal")]
		Fal
	}


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
