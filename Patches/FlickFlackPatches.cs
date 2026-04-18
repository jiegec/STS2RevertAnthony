using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// FlickFlack v0.99.1 vs current
// v0.99.1: Damage 7
// Current:  Damage 6

[HarmonyPatch(typeof(FlickFlack), "get_CanonicalVars")]
static class FlickFlack_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("flick-flack", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(7m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}
