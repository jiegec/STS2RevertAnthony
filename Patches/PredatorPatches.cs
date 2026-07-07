using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Predator v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Rarity Uncommon
// v0.107.1: Rarity Common

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class Predator_Rarity_Patch
{
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is Predator && RevertAnthony.IsVersion("predator", "v0.99.1", "v0.103.2"))
            __result = CardRarity.Uncommon;
    }
}
