using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// GrandFinale v0.99.1 vs current
// v0.99.1:  Damage 50(60), OnUpgrade → +10 damage
// Current: Damage 60(75), OnUpgrade → +15 damage

[HarmonyPatch(typeof(GrandFinale), "get_CanonicalVars")]
static class GrandFinale_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("grand-finale", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(50m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(GrandFinale), "OnUpgrade")]
static class GrandFinale_OnUpgrade_Patch
{
    static bool Prefix(GrandFinale __instance)
    {
        if (RevertAnthony.IsVersion("grand-finale", "v0.99.1"))
        {
            // v0.99.1: +10 damage (current: +15)
            __instance.DynamicVars.Damage.UpgradeValueBy(10m);
            return false;
        }
        return true;
    }
}
