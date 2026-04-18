using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Arsenal v0.99.1 vs current
// v0.99.1: OnUpgrade → +1 ArsenalPower
// Current: OnUpgrade → +Innate

[HarmonyPatch(typeof(Arsenal), "OnUpgrade")]
static class Arsenal_OnUpgrade_Patch
{
    static bool Prefix(Arsenal __instance)
    {
        if (RevertAnthony.IsVersion("arsenal", "v0.99.1"))
        {
            __instance.DynamicVars["ArsenalPower"].UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
