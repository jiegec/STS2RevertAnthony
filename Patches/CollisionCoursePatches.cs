using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// CollisionCourse v0.99.1 vs current
// v0.99.1:  Damage 9(12), OnUpgrade → +3 damage
// Current: Damage 11(15), OnUpgrade → +4 damage

[HarmonyPatch(typeof(CollisionCourse), "get_CanonicalVars")]
static class CollisionCourse_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("collision-course", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(9m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CollisionCourse), "OnUpgrade")]
static class CollisionCourse_OnUpgrade_Patch
{
    static bool Prefix(CollisionCourse __instance)
    {
        if (RevertAnthony.IsVersion("collision-course", "v0.99.1"))
        {
            // v0.99.1: +3 damage (current: +4)
            __instance.DynamicVars.Damage.UpgradeValueBy(3m);
            return false;
        }
        return true;
    }
}
