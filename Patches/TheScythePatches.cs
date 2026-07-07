using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// TheScythe v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Increase 3
// v0.107.1: Increase 4

[HarmonyPatch(typeof(TheScythe), "CanonicalVars", MethodType.Getter)]
static class TheScythe_CanonicalVars_Patch
{
    static bool Prefix(TheScythe __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("the-scythe", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(__instance.CurrentDamage, ValueProp.Move),
                new IntVar("Increase", 3m),
            };
            return false;
        }
        return true;
    }
}
