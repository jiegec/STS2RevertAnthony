using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// SeekerStrike v0.99.1 vs current
// v0.99.1: Damage 6, Cards 3
// Current:  Damage 9, Cards 3

[HarmonyPatch(typeof(SeekerStrike), "get_CanonicalVars")]
static class SeekerStrike_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("seeker-strike", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(6m, ValueProp.Move),
                new CardsVar(3),
            };
            return false;
        }
        return true;
    }
}
