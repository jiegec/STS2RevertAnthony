using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Synthesis v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Damage 12
// v0.107.1: Damage 14

[HarmonyPatch(typeof(Synthesis), "CanonicalVars", MethodType.Getter)]
static class Synthesis_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("synthesis", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(12m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}
