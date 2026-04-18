using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// ============================================
// Dirge - v0.99.1 vs v0.103.2
// Old: Summon 3(4) X times. Add X Souls(+) into your Draw Pile.
// New: Summon 3(4) X times. Add X Souls(+) into your Draw Pile. Exhause
// ============================================

[HarmonyPatch(typeof(Dirge), "get_CanonicalKeywords")]
static class Dirge_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("dirge", "v0.99.1"))
        {
            __result = new CardKeyword[] { };
            return false;
        }
        return true;
    }
}
