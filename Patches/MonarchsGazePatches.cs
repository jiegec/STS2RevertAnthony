using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// MonarchsGaze v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Cost 3
// v0.107.1: Cost 2

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class MonarchsGaze_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is MonarchsGaze && RevertAnthony.IsVersion("monarchs-gaze", "v0.99.1", "v0.103.2"))
            __result = 3;
    }
}
