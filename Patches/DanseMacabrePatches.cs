using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// DanseMacabre v0.99.1 vs current
// v0.99.1: DanseMacabrePower 3(4), Energy 2, OnUpgrade → +1 power
// Current:  DanseMacabrePower 4(6), Energy 2, OnUpgrade → +2 power

[HarmonyPatch(typeof(DanseMacabre), "get_CanonicalVars")]
static class DanseMacabre_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("danse-macabre", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new PowerVar<DanseMacabrePower>(3m),
                new EnergyVar(2),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(DanseMacabre), "OnUpgrade")]
static class DanseMacabre_OnUpgrade_Patch
{
    static bool Prefix(DanseMacabre __instance)
    {
        if (RevertAnthony.IsVersion("danse-macabre", "v0.99.1"))
        {
            // v0.99.1: +1 power (current: +2)
            __instance.DynamicVars["DanseMacabrePower"].UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
