using System;
using System.Threading.Tasks;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace RevertAnthony;

// GuidingStar v0.99.1 vs current
// v0.99.1: OnPlay: deal damage + apply DrawCardsNextTurnPower with VFX
// Current:  OnPlay: deal damage + draw cards immediately

[HarmonyPatch(typeof(GuidingStar), "OnPlay")]
static class GuidingStar_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, GuidingStar __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("guiding-star", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: draw next turn via power with VFX (current: draw immediately)
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, GuidingStar instance)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        var nCreature = NCombatRoom.Instance?.GetCreatureNode(cardPlay.Target);
        if (nCreature != null)
        {
            SfxCmd.Play("event:/sfx/characters/regent/regent_guiding_star");
            var vfx = NSmallMagicMissileVfx.Create(nCreature.GetBottomOfHitbox(), new Color("50b598"));
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
            await Cmd.Wait(vfx.WaitTime);
        }
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).WithNoAttackerAnim().FromCard(instance)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(instance.Owner.Creature, instance.DynamicVars.Cards.BaseValue, instance.Owner.Creature, instance);
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class GuidingStar_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is GuidingStar && RevertAnthony.IsVersion("guiding-star", "v0.99.1"))
            __result = new LocString("cards", "GUIDING_STAR_V0991.description");
    }
}
