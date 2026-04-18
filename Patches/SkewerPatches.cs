using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Skewer v0.99.1 vs current
// v0.99.1: Damage 7 per hit (current: Damage 8)

[HarmonyPatch(typeof(Skewer), "get_CanonicalVars")]
static class Skewer_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("skewer", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(7m, ValueProp.Move)
            };
            return false;
        }
        return true;
    }
}
