using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// Parry v0.99.1 vs current
// v0.99.1:  ParryPower 6(10), OnUpgrade → +3 power
// Current: ParryPower 10(14), OnUpgrade → +4 power

[HarmonyPatch(typeof(Parry), "get_CanonicalVars")]
static class Parry_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("parry", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new PowerVar<ParryPower>(6m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Parry), "OnUpgrade")]
static class Parry_OnUpgrade_Patch
{
    static bool Prefix(Parry __instance)
    {
        if (RevertAnthony.IsVersion("parry", "v0.99.1"))
        {
            // v0.99.1: +3 power (current: +4)
            __instance.DynamicVars["ParryPower"].UpgradeValueBy(3m);
            return false;
        }
        return true;
    }
}
