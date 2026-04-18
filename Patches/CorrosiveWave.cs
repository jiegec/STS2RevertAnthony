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

namespace RevertAnthony;

// ============================================
// Corrosive Wave - v0.99.1 vs v0.103.2
// Old: Whenever you draw a card this turn, apply 3(4) Poison to ALL enemies.
// New: Whenever you draw a card this turn, apply 2(3) Poison to ALL enemies.
// ============================================

[HarmonyPatch(typeof(CorrosiveWave), "get_CanonicalVars")]
static class CorrosiveWave_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("corrosive-wave", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DynamicVar("CorrosiveWave", 3m),
            };
            return false;
        }
        return true;
    }
}
