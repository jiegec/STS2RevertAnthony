using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// SerpentForm v0.99.1 vs current
// v0.99.1: OnUpgrade → +1 SerpentFormPower
// Current:  OnUpgrade → +2 SerpentFormPower

[HarmonyPatch(typeof(SerpentForm), "OnUpgrade")]
static class SerpentForm_OnUpgrade_Patch
{
    static bool Prefix(SerpentForm __instance)
    {
        if (RevertAnthony.IsVersion("serpent-form", "v0.99.1"))
        {
            // v0.99.1: +1 power (current: +2)
            __instance.DynamicVars["SerpentFormPower"].UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
