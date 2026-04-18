using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// RipAndTear v0.99.1 vs current
// v0.99.1: Uncommon rarity (current: Event)

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class RipAndTear_Rarity_Patch
{
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is RipAndTear && RevertAnthony.IsVersion("rip-and-tear", "v0.99.1"))
        {
            __result = CardRarity.Uncommon;
        }
    }
}
