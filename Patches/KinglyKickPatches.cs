using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// KinglyKick v0.99.1 vs current
// v0.99.1: Damage 24(30), OnUpgrade → +6 damage
// Current:  Damage 27(35), OnUpgrade → +8 damage

[HarmonyPatch(typeof(KinglyKick), "get_CanonicalVars")]
static class KinglyKick_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("kingly-kick", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(24m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(KinglyKick), "OnUpgrade")]
static class KinglyKick_OnUpgrade_Patch
{
    static bool Prefix(KinglyKick __instance)
    {
        if (RevertAnthony.IsVersion("kingly-kick", "v0.99.1"))
        {
            // v0.99.1: +6 damage (current: +8)
            __instance.DynamicVars.Damage.UpgradeValueBy(6m);
            return false;
        }
        return true;
    }
}
