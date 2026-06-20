using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Fusion v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Cost 2, no keywords, upgrade: -1 cost
// v0.107.1: Cost 1, Exhaust, upgrade: Remove Exhaust

[HarmonyPatch(typeof(Fusion), "CanonicalKeywords", MethodType.Getter)]
static class Fusion_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("fusion", "v0.99.1", "v0.103.2"))
        {
            __result = System.Array.Empty<CardKeyword>();
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Fusion), "OnUpgrade")]
static class Fusion_OnUpgrade_Patch
{
    static bool Prefix(Fusion __instance)
    {
        if (RevertAnthony.IsVersion("fusion", "v0.99.1", "v0.103.2"))
        {
            __instance.EnergyCost.UpgradeBy(-1);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class Fusion_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is Fusion && RevertAnthony.IsVersion("fusion", "v0.99.1", "v0.103.2"))
            __result = 2;
    }
}
