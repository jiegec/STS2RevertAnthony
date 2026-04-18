using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// NeowsFury v0.99.1 vs current
// v0.99.1: OnUpgrade → +4 damage
// Current:  OnUpgrade → +4 damage, +1 cards

[HarmonyPatch(typeof(NeowsFury), "OnUpgrade")]
static class NeowsFury_OnUpgrade_Patch
{
    static bool Prefix(NeowsFury __instance)
    {
        if (RevertAnthony.IsVersion("neows-fury", "v0.99.1"))
        {
            // v0.99.1: +4 damage only (current: +4 damage, +1 cards)
            __instance.DynamicVars.Damage.UpgradeValueBy(4m);
            return false;
        }
        return true;
    }
}
