using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// MinionDiveBomb v0.99.1 vs current
// v0.99.1: Cost 1 (current: Cost 0)

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class MinionDiveBomb_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is MinionDiveBomb && RevertAnthony.IsVersion("minion-dive-bomb", "v0.99.1"))
            __result = 1;
    }
}
