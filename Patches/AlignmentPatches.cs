using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Alignment v0.99.1 vs current
// v0.99.1: Star Cost 2
// Current:  Star Cost 3

[HarmonyPatch(typeof(Alignment), "CanonicalStarCost", MethodType.Getter)]
static class Alignment_CanonicalStarCost_Patch
{
    static bool Prefix(ref int __result)
    {
        if (RevertAnthony.IsVersion("alignment", "v0.99.1"))
        {
            __result = 2;
            return false;
        }
        return true;
    }
}
