using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Production v0.99.1 vs current
// v0.99.1: OnUpgrade → remove Exhaust keyword
// Current:  OnUpgrade → +1 energy

[HarmonyPatch(typeof(Production), "OnUpgrade")]
static class Production_OnUpgrade_Patch
{
    static bool Prefix(Production __instance)
    {
        if (RevertAnthony.IsVersion("production", "v0.99.1"))
        {
            // v0.99.1: remove Exhaust (current: +1 energy)
            __instance.RemoveKeyword(CardKeyword.Exhaust);
            return false;
        }
        return true;
    }
}
