using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// ArsenalPower v0.99.1 vs current
// v0.99.1: AfterCardPlayed - triggers when playing a Colorless card
// Current:  AfterCardGeneratedForCombat - triggers when creating a card

[HarmonyPatch(typeof(ArsenalPower), "AfterCardGeneratedForCombat")]
static class ArsenalPower_AfterCardGeneratedForCombat_Patch
{
    static bool Prefix()
    {
        if (RevertAnthony.IsVersion("arsenal", "v0.99.1"))
        {
            return false; // Skip original method
        }
        return true;
    }
}

[HarmonyPatch(typeof(ArsenalPower), "AfterCardPlayed")]
static class ArsenalPower_AfterCardPlayed_Patch
{
    static bool Prefix(PlayerChoiceContext context, CardPlay cardPlay, ArsenalPower __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("arsenal", "v0.99.1"))
            return true;

        __result = OldAfterCardPlayed(context, cardPlay, __instance);
        return false;
    }

    static async Task OldAfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay, ArsenalPower instance)
    {
        if (cardPlay.Card.Owner == instance.Owner.Player && cardPlay.Card.VisualCardPool.IsColorless)
        {
            // Flash() is protected - use AccessTools to call it
            var flashMethod = AccessTools.Method(typeof(MegaCrit.Sts2.Core.Models.PowerModel), "Flash");
            flashMethod?.Invoke(instance, null);
            await PowerCmd.Apply<StrengthPower>(instance.Owner, instance.Amount, instance.Owner, null);
        }
    }
}
