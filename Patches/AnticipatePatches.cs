using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// Anticipate v0.99.1 vs current
// v0.99.1: Dexterity 3(5), OnUpgrade → +2 dexterity
// Current:  Dexterity 2(3), OnUpgrade → +1 dexterity

[HarmonyPatch(typeof(Anticipate), "get_CanonicalVars")]
static class Anticipate_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("anticipate", "v0.99.1"))
        {
            // v0.99.1: 3 dexterity (current: 2)
            __result = new DynamicVar[]
            {
                new PowerVar<DexterityPower>(3m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Anticipate), "OnUpgrade")]
static class Anticipate_OnUpgrade_Patch
{
    static bool Prefix(Anticipate __instance)
    {
        if (RevertAnthony.IsVersion("anticipate", "v0.99.1"))
        {
            // v0.99.1: +2 dexterity (current: +1)
            __instance.DynamicVars.Dexterity.UpgradeValueBy(2m);
            return false;
        }
        return true;
    }
}
