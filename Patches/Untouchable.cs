using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Untouchable v0.99.1/v0.103.2 vs current
// v0.99.1: Block 9(12), OnUpgrade → +3 block
// v0.103.2: Block 6(8), OnUpgrade → +2 block
// v0.107.1: Block 6(9), OnUpgrade → +3 block

[HarmonyPatch(typeof(Untouchable), "get_CanonicalVars")]
static class Untouchable_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("untouchable", "v0.99.1"))
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

[HarmonyPatch(typeof(Untouchable), "OnUpgrade")]
static class Untouchable_OnUpgrade_Patch
{
    static bool Prefix(Untouchable __instance)
    {
        if (RevertAnthony.IsVersion("untouchable", "v0.103.2"))
        {
            // v0.103.2: +2 block (v0.99.1: +3, v0.107.1: +3)
            __instance.DynamicVars.Block.UpgradeValueBy(2m);
            return false;
        }
        if (RevertAnthony.IsVersion("untouchable", "v0.99.1"))
        {
            // v0.99.1: +3 block (current: +3)
            __instance.DynamicVars.Block.UpgradeValueBy(3m);
            return false;
        }
        return true;
    }
}
