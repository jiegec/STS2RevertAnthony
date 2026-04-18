using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// BladeOfInk v0.99.1 vs current — complete rework
// v0.99.1: Gain 2(3) BladeOfInkPower (Strength per shiv played)
//          ExtraHoverTips → StrengthPower, OnUpgrade → +1 Strength
// Current: Add 2(3) Shivs to hand with Inky enchantment
//          ExtraHoverTips → Shiv + Inky, OnUpgrade → +1 Shiv

[HarmonyPatch(typeof(BladeOfInk), "get_ExtraHoverTips")]
static class BladeOfInk_ExtraHoverTips_Patch
{
    static bool Prefix(ref IEnumerable<IHoverTip> __result)
    {
        if (RevertAnthony.IsVersion("blade-of-ink", "v0.99.1"))
        {
            __result = new IHoverTip[]
            {
                HoverTipFactory.FromPower<StrengthPower>(),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(BladeOfInk), "get_CanonicalVars")]
static class BladeOfInk_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("blade-of-ink", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new PowerVar<StrengthPower>(2m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(BladeOfInk), "OnPlay")]
static class BladeOfInk_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, BladeOfInk __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("blade-of-ink", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: Apply BladeOfInkPower equal to Strength value
    // Current: Create Shivs in hand, each enchanted with Inky
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, BladeOfInk instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<BladeOfInkPower>(instance.Owner.Creature, instance.DynamicVars.Strength.IntValue, instance.Owner.Creature, instance);
    }
}

[HarmonyPatch(typeof(BladeOfInk), "OnUpgrade")]
static class BladeOfInk_OnUpgrade_Patch
{
    static bool Prefix(BladeOfInk __instance)
    {
        if (RevertAnthony.IsVersion("blade-of-ink", "v0.99.1"))
        {
            // v0.99.1: +1 Strength
            // Current: +1 Shiv
            __instance.DynamicVars.Strength.UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
