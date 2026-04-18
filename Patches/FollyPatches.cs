using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Folly v0.99.1 vs current
// v0.99.1: Keywords = Unplayable, Eternal, Innate
// Current:  Keywords = Unplayable, Eternal, Innate, Ethereal

[HarmonyPatch(typeof(Folly), "get_CanonicalKeywords")]
static class Folly_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("folly", "v0.99.1"))
        {
            // v0.99.1: no Ethereal (current adds Ethereal)
            __result = new CardKeyword[]
            {
                CardKeyword.Unplayable,
                CardKeyword.Eternal,
                CardKeyword.Innate,
            };
            return false;
        }
        return true;
    }
}
