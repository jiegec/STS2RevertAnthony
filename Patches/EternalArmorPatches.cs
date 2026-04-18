using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// EternalArmor v0.99.1 vs current
// v0.99.1: Plating 7(9), OnUpgrade → +2 plating
// Current:  Plating 9(12), OnUpgrade → +3 plating

[HarmonyPatch(typeof(EternalArmor), "get_CanonicalVars")]
static class EternalArmor_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("eternal-armor", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new PowerVar<PlatingPower>(7m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(EternalArmor), "OnUpgrade")]
static class EternalArmor_OnUpgrade_Patch
{
    static bool Prefix(EternalArmor __instance)
    {
        if (RevertAnthony.IsVersion("eternal-armor", "v0.99.1"))
        {
            // v0.99.1: +2 plating (current: +3)
            __instance.DynamicVars["PlatingPower"].UpgradeValueBy(2m);
            return false;
        }
        return true;
    }
}
