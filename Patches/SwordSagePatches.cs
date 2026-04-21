using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony.Patches;

[HarmonyPatch(typeof(AbstractModel), "TryModifyEnergyCostInCombat")]
static class SwordSagePower_TryModifyEnergyCostInCombat_Patch
{
    static void Postfix(AbstractModel __instance, CardModel card, decimal originalCost, ref decimal modifiedCost, ref bool __result)
    {
        if (__instance is not SwordSagePower power)
            return;

        if (!RevertAnthony.IsVersion("sword-sage", "v0.99.1"))
            return;

        if (card.Owner.Creature != power.Owner)
            return;

        if (card is not SovereignBlade)
            return;

        modifiedCost = originalCost + (decimal)power.Amount;
        __result = true;
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class SwordSage_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is SwordSage && RevertAnthony.IsVersion("sword-sage", "v0.99.1"))
        {
            __result = new LocString("cards", "SWORD_SAGE_V0991.description");
        }
    }
}
