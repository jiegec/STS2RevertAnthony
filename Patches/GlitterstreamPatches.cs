using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Glitterstream v0.99.1 vs current
// v0.99.1: Block 11, BlockNextTurn 4
// Current:  Block 11, BlockNextTurn 5

[HarmonyPatch(typeof(Glitterstream), "get_CanonicalVars")]
static class Glitterstream_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("glitterstream", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new BlockVar(11m, ValueProp.Move),
                new BlockVar("BlockNextTurn", 4m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}
