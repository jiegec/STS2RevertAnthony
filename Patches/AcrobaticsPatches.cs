using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Acrobatics v0.99.1 vs current
// v0.99.1: Common rarity (current: Uncommon)

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class Acrobatics_Rarity_Patch
{
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is Acrobatics && RevertAnthony.IsVersion("acrobatics", "v0.99.1"))
        {
            __result = CardRarity.Common;
        }
    }
}
