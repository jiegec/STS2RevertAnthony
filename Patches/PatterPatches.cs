using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Patter v0.99.1 vs current
// v0.99.1: Block 8, Vigor 2
// Current:  Block 9, Vigor 2

[HarmonyPatch(typeof(Patter), "get_CanonicalVars")]
static class Patter_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("patter", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new BlockVar(8m, ValueProp.Move),
                new PowerVar<VigorPower>(2m),
            };
            return false;
        }
        return true;
    }
}
