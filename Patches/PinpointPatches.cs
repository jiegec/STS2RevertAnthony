using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Pinpoint v0.99.1 vs current
// v0.99.1: Damage 17(22), OnUpgrade → +5 damage
// Current:  Damage 15(19), OnUpgrade → +4 damage

[HarmonyPatch(typeof(Pinpoint), "get_CanonicalVars")]
static class Pinpoint_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("pinpoint", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(17m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Pinpoint), "OnUpgrade")]
static class Pinpoint_OnUpgrade_Patch
{
    static bool Prefix(Pinpoint __instance)
    {
        if (RevertAnthony.IsVersion("pinpoint", "v0.99.1"))
        {
            // v0.99.1: +5 damage (current: +4)
            __instance.DynamicVars.Damage.UpgradeValueBy(5m);
            return false;
        }
        return true;
    }
}
