using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// WroughtInWar v0.99.1 vs current
// v0.99.1: Damage 7, Forge 5
// Current:  Damage 7, Forge 7

[HarmonyPatch(typeof(WroughtInWar), "get_CanonicalVars")]
static class WroughtInWar_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("wrought-in-war", "v0.99.1"))
        {
            // v0.99.1: Forge 5 (current: Forge 7)
            __result = new DynamicVar[]
            {
                new DamageVar(7m, ValueProp.Move),
                new ForgeVar(5),
            };
            return false;
        }
        return true;
    }
}
