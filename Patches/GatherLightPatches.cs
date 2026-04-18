using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// GatherLight v0.99.1 vs current
// v0.99.1: Block 7, Stars 1
// Current:  Block 8, Stars 1

[HarmonyPatch(typeof(GatherLight), "get_CanonicalVars")]
static class GatherLight_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("gather-light", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new BlockVar(7m, ValueProp.Move),
                new StarsVar(1),
            };
            return false;
        }
        return true;
    }
}
