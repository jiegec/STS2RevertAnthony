using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// ============================================
// Hemokinesis - v0.99.1 vs v0.103.2
// Old: Deal 14 damage
// New: Deal 15 damage
// ============================================

[HarmonyPatch(typeof(Hemokinesis), "get_CanonicalVars")]
static class Hemokinesis_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("hemokinesis", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new HpLossVar(2m),
                new DamageVar(14m, ValueProp.Move)
            };
            return false;
        }
        return true;
    }
}
