using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// CelestialMight v0.99.1 vs current
// v0.99.1: OnUpgrade → +2 damage
// Current:  OnUpgrade → +1 repeat

[HarmonyPatch(typeof(CelestialMight), "OnUpgrade")]
static class CelestialMight_OnUpgrade_Patch
{
    static bool Prefix(CelestialMight __instance)
    {
        if (RevertAnthony.IsVersion("celestial-might", "v0.99.1"))
        {
            // v0.99.1: +2 damage (current: +1 repeat)
            __instance.DynamicVars.Damage.UpgradeValueBy(2m);
            return false;
        }
        return true;
    }
}
