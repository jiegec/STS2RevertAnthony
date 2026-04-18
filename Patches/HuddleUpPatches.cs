using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// HuddleUp v0.99.1 vs current
// v0.99.1: Keywords = (none)
// Current:  Keywords = Exhaust

[HarmonyPatch(typeof(HuddleUp), "get_CanonicalKeywords")]
static class HuddleUp_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("huddle-up", "v0.99.1"))
        {
            // v0.99.1: no Exhaust (current: has Exhaust)
            __result = Array.Empty<CardKeyword>();
            return false;
        }
        return true;
    }
}
