using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
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
// The override was removed in current version. Since parameter name differs between
// versions (context vs choiceContext), we patch manually to avoid Harmony name matching.
static class ArsenalPowerAfterCardPlayedPatch
{
    private static bool _patched;

    public static void Apply(Harmony harmony)
    {
        if (_patched)
            return;

        var method = typeof(AbstractModel).GetMethod("AfterCardPlayed",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null,
            new[] { typeof(PlayerChoiceContext), typeof(CardPlay) }, null);

        if (method == null)
        {
            Log.Warn("RevertAnthony: AbstractModel.AfterCardPlayed not found");
            return;
        }

        harmony.Patch(method, prefix: new HarmonyMethod(typeof(ArsenalPowerAfterCardPlayedPatch), nameof(Prefix)));
        Log.Info("RevertAnthony: Patched AbstractModel.AfterCardPlayed for ArsenalPower");
        _patched = true;
    }

    static bool Prefix(AbstractModel __instance, object[] __args)
    {
        if (!(__instance is ArsenalPower arsenalPower))
            return true;

        if (!RevertAnthony.IsVersion("arsenal", "v0.99.1"))
            return true;

        var context = (PlayerChoiceContext)__args[0];
        var cardPlay = (CardPlay)__args[1];

        // v0.99.1 behavior: trigger when playing a Colorless card
        _ = TriggerArsenalPower(context, cardPlay, arsenalPower);
        return true;
    }

    static async Task TriggerArsenalPower(PlayerChoiceContext context, CardPlay cardPlay, ArsenalPower instance)
    {
        if (cardPlay.Card.Owner == instance.Owner.Player && cardPlay.Card.VisualCardPool.IsColorless)
        {
            // Flash() is protected - use AccessTools to call it
            var flashMethod = AccessTools.Method(typeof(MegaCrit.Sts2.Core.Models.PowerModel), "Flash");
            flashMethod?.Invoke(instance, null);
            await PowerCmd.Apply<StrengthPower>(context, instance.Owner, instance.Amount, instance.Owner, null);
        }
    }
}
