using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// BundleOfJoy v0.99.1 vs current
// v0.99.1: Cost 2 (current: Cost 1)

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class BundleOfJoy_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is BundleOfJoy && RevertAnthony.IsVersion("bundle-of-joy", "v0.99.1"))
        {
            __result = 2;
        }
    }
}
