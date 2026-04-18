using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// ============================================
// Alignment - v0.99.1 vs v0.103.2
// Old: Cost 2 stars
// New: Cost 3 stars
// ============================================

[HarmonyPatch(typeof(Alignment), "get_CanonicalStarCost")]
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
