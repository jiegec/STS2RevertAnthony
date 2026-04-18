using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// BansheesCry v0.99.1 vs current
// v0.99.1: Cost 6, OnUpgrade → +6 damage
// Current: Cost 9, OnUpgrade → -2 cost

[HarmonyPatch(typeof(BansheesCry), "OnUpgrade")]
static class BansheesCry_OnUpgrade_Patch
{
    static bool Prefix(BansheesCry __instance)
    {
        if (RevertAnthony.IsVersion("banshees-cry", "v0.99.1"))
        {
            __instance.DynamicVars.Damage.UpgradeValueBy(6m);
            return false;
        }
        return true;
    }
}
