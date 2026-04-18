using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// HeirloomHammer v0.99.1 vs current
// v0.99.1: Damage 17, Repeat 1
// Current:  Damage 20, Repeat 1

[HarmonyPatch(typeof(HeirloomHammer), "get_CanonicalVars")]
static class HeirloomHammer_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("heirloom-hammer", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(17m, ValueProp.Move),
                new RepeatVar(1),
            };
            return false;
        }
        return true;
    }
}
