using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Furnace v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Forge 4
// v0.107.1: Forge 5

[HarmonyPatch(typeof(Furnace), "CanonicalVars", MethodType.Getter)]
static class Furnace_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("furnace", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new ForgeVar(4),
            };
            return false;
        }
        return true;
    }
}
