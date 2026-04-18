using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// MinionStrike v0.99.1 vs current
// v0.99.1: Damage 7 (current: Damage 6)

[HarmonyPatch(typeof(MinionStrike), "get_CanonicalVars")]
static class MinionStrike_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("minion-strike", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(7m, ValueProp.Move),
                new CardsVar(1)
            };
            return false;
        }
        return true;
    }
}
