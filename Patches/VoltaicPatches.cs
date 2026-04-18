using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Voltaic v0.99.1 vs current
// v0.99.1: Cost 2 (current: Cost 3)

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class Voltaic_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is Voltaic && RevertAnthony.IsVersion("voltaic", "v0.99.1"))
        {
            __result = 2;
        }
    }
}
