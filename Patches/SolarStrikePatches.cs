using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// SolarStrike v0.99.1 vs current
// v0.99.1: Damage 8, Stars 1
// Current:  Damage 9, Stars 1

[HarmonyPatch(typeof(SolarStrike), "get_CanonicalVars")]
static class SolarStrike_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("solar-strike", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(8m, ValueProp.Move),
                new StarsVar(1),
            };
            return false;
        }
        return true;
    }
}
