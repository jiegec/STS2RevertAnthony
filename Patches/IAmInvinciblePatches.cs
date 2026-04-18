using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// IAmInvincible v0.99.1 vs current
// v0.99.1: Block 9
// Current:  Block 10

[HarmonyPatch(typeof(IAmInvincible), "get_CanonicalVars")]
static class IAmInvincible_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("i-am-invincible", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new BlockVar(9m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}
