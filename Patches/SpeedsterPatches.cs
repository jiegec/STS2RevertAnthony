using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Speedster v0.99.1 vs current
// v0.99.1: OnUpgrade → +1 SpeedsterPower
// Current:  OnUpgrade → +Innate

[HarmonyPatch(typeof(Speedster), "OnUpgrade")]
static class Speedster_OnUpgrade_Patch
{
    static bool Prefix(Speedster __instance)
    {
        if (RevertAnthony.IsVersion("speedster", "v0.99.1"))
        {
            // v0.99.1: +1 SpeedsterPower (current: +Innate)
            __instance.DynamicVars["SpeedsterPower"].UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
