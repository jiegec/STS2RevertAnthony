using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// ForgottenRitual v0.99.1 vs current
// v0.99.1: Keywords = (none)
// Current:  Keywords = Exhaust

[HarmonyPatch(typeof(ForgottenRitual), "get_CanonicalKeywords")]
static class ForgottenRitual_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("forgotten-ritual", "v0.99.1"))
        {
            // v0.99.1: no Exhaust keyword (current adds Exhaust)
            __result = Array.Empty<CardKeyword>();
            return false;
        }
        return true;
    }
}
