using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// GraveWarden v0.99.1 vs current
// v0.99.1: ExtraHoverTips → Soul (upgraded=true), OnPlay: create Souls (upgrade them if GraveWarden upgraded)
//          OnUpgrade → +2 block
// Current:  ExtraHoverTips → Soul (no upgraded param), OnPlay: create unupgraded Souls only
//          OnUpgrade → +3 block

[HarmonyPatch(typeof(GraveWarden), "get_ExtraHoverTips")]
static class GraveWarden_ExtraHoverTips_Patch
{
    static bool Prefix(GraveWarden __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (RevertAnthony.IsVersion("grave-warden", "v0.99.1"))
        {
            // v0.99.1: Soul tooltip with IsUpgraded flag (current: no upgraded param)
            __result = new IHoverTip[]
            {
                HoverTipFactory.FromCard<Soul>(__instance.IsUpgraded),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(GraveWarden), "OnPlay")]
static class GraveWarden_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, GraveWarden __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("grave-warden", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: create Souls, upgrade them if GraveWarden is upgraded
    // Current:  create Souls (never upgrade them)
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, GraveWarden instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await CreatureCmd.GainBlock(instance.Owner.Creature, instance.DynamicVars.Block, cardPlay);
        List<Soul> list = Soul.Create(instance.Owner, instance.DynamicVars.Cards.IntValue, instance.CombatState).ToList();
        if (instance.IsUpgraded)
        {
            foreach (Soul item in list)
            {
                CardCmd.Upgrade(item);
            }
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Draw, addedByPlayer: true, CardPilePosition.Random));
    }
}

[HarmonyPatch(typeof(GraveWarden), "OnUpgrade")]
static class GraveWarden_OnUpgrade_Patch
{
    static bool Prefix(GraveWarden __instance)
    {
        if (RevertAnthony.IsVersion("grave-warden", "v0.99.1"))
        {
            // v0.99.1: +2 block (current: +3)
            __instance.DynamicVars.Block.UpgradeValueBy(2m);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class GraveWarden_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is GraveWarden && RevertAnthony.IsVersion("grave-warden", "v0.99.1"))
            __result = new LocString("cards", "GRAVE_WARDEN_V0991.description");
    }
}
