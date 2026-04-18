using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// SculptingStrike v0.99.1 vs current
// v0.99.1: Damage 8
// Current:  Damage 9

[HarmonyPatch(typeof(SculptingStrike), "get_CanonicalVars")]
static class SculptingStrike_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("sculpting-strike", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(8m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}
