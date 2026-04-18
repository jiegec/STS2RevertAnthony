using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// RefineBlade v0.99.1 vs current
// v0.99.1: Forge 6, Energy 1
// Current:  Forge 9, Energy 1

[HarmonyPatch(typeof(RefineBlade), "get_CanonicalVars")]
static class RefineBlade_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("refine-blade", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new ForgeVar(6),
                new EnergyVar(1),
            };
            return false;
        }
        return true;
    }
}
