using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// ============================================
// Base CardModel patches for properties not overridden in derived classes
// ============================================

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class CardModel_Rarity_Patch
{
    // ============================================
    // Acrobatics - v0.99.1 vs v0.103.2
    // Old: Common rarity
    // New: Uncommon rarity
    // ============================================
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is Acrobatics && RevertAnthony.IsVersion("acrobatics", "v0.99.1"))
        {
            __result = CardRarity.Common;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class CardModel_CanonicalEnergyCost_Patch
{
    // ============================================
    // Borrowed Time - v0.99.1 vs v0.103.2
    // Old: Cost 0 (set in constructor: base(0, ...))
    // New: Cost 1 (set in constructor: base(1, ...))
    // ============================================
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is BorrowedTime && RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            __result = 0;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class CardModel_Description_Patch
{
    // ============================================
    // Borrowed Time - v0.99.1 vs v0.103.2
    // Old description uses DoomPower + Energy
    // New description uses Energy + ExtraCost
    // ============================================
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is BorrowedTime && RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            __result = new LocString("cards", "BORROWED_TIME_V0991.description");
        }
    }
}
