using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Hotfix v0.99.1 vs current
// v0.99.1:  Keywords = (none). OnUpgrade → +1 Focus
// Current: Keywords = Exhaust. OnUpgrade → remove Exhaust

[HarmonyPatch(typeof(Hotfix), "get_CanonicalKeywords")]
static class Hotfix_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("hotfix", "v0.99.1"))
        {
            // v0.99.1: no Exhaust (current: starts with Exhaust)
            __result = new CardKeyword[]
            {
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Hotfix), "OnUpgrade")]
static class Hotfix_OnUpgrade_Patch
{
    static bool Prefix(Hotfix __instance)
    {
        if (RevertAnthony.IsVersion("hotfix", "v0.99.1"))
        {
            // v0.99.1: +1 Focus (current: remove Exhaust)
            __instance.DynamicVars["FocusPower"].UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
