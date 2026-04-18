using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Defy v0.99.1 vs current
// v0.99.1: OnUpgrade → +1 block, +1 weak
// Current:  OnUpgrade → +3 block

[HarmonyPatch(typeof(Defy), "OnUpgrade")]
static class Defy_OnUpgrade_Patch
{
    static bool Prefix(Defy __instance)
    {
        if (RevertAnthony.IsVersion("defy", "v0.99.1"))
        {
            // v0.99.1: +1 block, +1 weak (current: +3 block only)
            __instance.DynamicVars.Block.UpgradeValueBy(1m);
            __instance.DynamicVars.Weak.UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
