using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// LeadingStrike v0.99.1 vs current
// v0.99.1: Shivs 1, Damage 7
// Current:  Shivs 2, Damage 3

[HarmonyPatch(typeof(LeadingStrike), "get_CanonicalVars")]
static class LeadingStrike_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("leading-strike", "v0.99.1"))
        {
            // v0.99.1: fewer shivs but more damage (current: more shivs, less damage)
            __result = new DynamicVar[]
            {
                new CardsVar("Shivs", 1),
                new DamageVar(7m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}
