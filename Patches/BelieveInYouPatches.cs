using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// BelieveInYou v0.99.1 vs current
// v0.99.1: Energy 3(4)
// Current: Energy 2(3)

[HarmonyPatch(typeof(BelieveInYou), "get_CanonicalVars")]
static class BelieveInYou_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("believe-in-you", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new EnergyVar(3),
            };
            return false;
        }
        return true;
    }
}
