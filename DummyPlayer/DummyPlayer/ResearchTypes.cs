﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DummyPlayer
{
    public enum ResearchTypes
    {
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
}
