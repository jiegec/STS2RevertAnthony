using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
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
            return false; // Skip current behavior (card creation trigger)
        }
        return true;
    }
}

// v0.99.1 ArsenalPower overrides AfterCardPlayed (inherited from AbstractModel)
// Since the override was removed in current version, we patch the base class method
// to inject the old behavior when the instance is ArsenalPower and v0.99.1 is selected
[HarmonyPatch(typeof(AbstractModel), "AfterCardPlayed")]
static class ArsenalPower_AbstractModel_AfterCardPlayed_Patch
{
    static bool Prefix(AbstractModel __instance, PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (!(__instance is ArsenalPower arsenalPower))
            return true;

        if (!RevertAnthony.IsVersion("arsenal", "v0.99.1"))
            return true;

        // v0.99.1 behavior: trigger when playing a Colorless card
        _ = TriggerArsenalPower(context, cardPlay, arsenalPower);
        return true; // Continue with original method (base AfterCardPlayed is empty for powers)
    }

    static async Task TriggerArsenalPower(PlayerChoiceContext context, CardPlay cardPlay, ArsenalPower instance)
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
