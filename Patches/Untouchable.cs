using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// ============================================
// Untouchable - v0.99.1 vs v0.103.2
// Old: Sly. Gain 9(12) Block.
// New: Sly. Gain 6(8) Block.
// ============================================

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
        if (RevertAnthony.IsVersion("untouchable", "v0.99.1"))
        {
            __instance.DynamicVars.Block.UpgradeValueBy(3m);
            return false;
        }
        return true;
    }
}
