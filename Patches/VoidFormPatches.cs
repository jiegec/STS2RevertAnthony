using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// VoidForm v0.99.1 vs current
// v0.99.1:  Keywords = (none). OnUpgrade → remove Ethereal
// Current: Keywords = Ethereal. OnUpgrade → +1 VoidFormPower

[HarmonyPatch(typeof(VoidForm), "get_CanonicalKeywords")]
static class VoidForm_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("void-form", "v0.99.1"))
        {
            // v0.99.1: no Ethereal, gains removal on upgrade (current: starts with Ethereal)
            __result = new CardKeyword[]
            {
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(VoidForm), "OnUpgrade")]
static class VoidForm_OnUpgrade_Patch
{
    static bool Prefix(VoidForm __instance)
    {
        if (RevertAnthony.IsVersion("void-form", "v0.99.1"))
        {
            // v0.99.1: +1 VoidFormPower (current: remove Ethereal)
            __instance.DynamicVars["VoidFormPower"].UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
