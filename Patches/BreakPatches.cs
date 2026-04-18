using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Break v0.99.1 vs current
// v0.99.1: Cost 2, OnUpgrade → +5 damage, +2 vulnerable
// Current:  Cost 1, OnUpgrade → +10 damage, +2 vulnerable

[HarmonyPatch(typeof(Break), "OnUpgrade")]
static class Break_OnUpgrade_Patch
{
    static bool Prefix(Break __instance)
    {
        if (RevertAnthony.IsVersion("break", "v0.99.1"))
        {
            // v0.99.1: +5 damage (current: +10)
            __instance.DynamicVars.Damage.UpgradeValueBy(5m);
            __instance.DynamicVars.Vulnerable.UpgradeValueBy(2m);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class Break_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is Break && RevertAnthony.IsVersion("break", "v0.99.1"))
            __result = 2;
    }
}
