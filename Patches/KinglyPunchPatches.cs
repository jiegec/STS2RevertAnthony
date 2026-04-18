using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// KinglyPunch v0.99.1 vs current
// v0.99.1:  Damage 8, Increase 3. OnUpgrade → +2 Increase only
// Current: Damage 8, Increase 4. OnUpgrade → +2 damage, +2 Increase

[HarmonyPatch(typeof(KinglyPunch), "get_CanonicalVars")]
static class KinglyPunch_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("kingly-punch", "v0.99.1"))
        {
            // v0.99.1: Increase 3 (current: Increase 4)
            __result = new DynamicVar[]
            {
                new DamageVar(8m, ValueProp.Move),
                new DynamicVar("Increase", 3m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(KinglyPunch), "OnUpgrade")]
static class KinglyPunch_OnUpgrade_Patch
{
    static bool Prefix(KinglyPunch __instance)
    {
        if (RevertAnthony.IsVersion("kingly-punch", "v0.99.1"))
        {
            // v0.99.1: +2 Increase only (current: +2 damage, +2 Increase)
            __instance.DynamicVars["Increase"].UpgradeValueBy(2m);
            return false;
        }
        return true;
    }
}
