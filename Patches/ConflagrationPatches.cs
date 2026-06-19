using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Conflagration v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: CalculationBase 8, ExtraDamage 2, CalculatedDamage (scales with attacks played)
//                    Upgrade: +1 CalculationBase, +1 ExtraDamage
// v0.107.1: Damage 2, Repeat 4 (fixed repeat instead of calculated)
//           Upgrade: +1 Repeat

[HarmonyPatch(typeof(Conflagration), "CanonicalVars", MethodType.Getter)]
static class Conflagration_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("conflagration", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new CalculationBaseVar(8m),
                new ExtraDamageVar(2m),
                new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature _) =>
                    CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) =>
                    {
                        if (e.HappenedThisTurn(Compat.GetCombatState(card))) {
                            return false;
                        }
                        if (e.CardPlay.Card.Type != CardType.Attack) {
                            return false;
                        }
                        return (e.CardPlay.Card.Owner == card.Owner) ? true : false;
                    })),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Conflagration), "OnPlay")]
static class Conflagration_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Conflagration __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("conflagration", "v0.99.1", "v0.103.2"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Conflagration instance)
    {
        IReadOnlyList<Creature> hittableEnemies = instance.CombatState.HittableEnemies;
        foreach (Creature item in hittableEnemies)
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(item));
        }
        await DamageCmd.Attack(instance.DynamicVars.CalculatedDamage).FromCard(instance).TargetingAllOpponents(instance.CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }
}

[HarmonyPatch(typeof(Conflagration), "OnUpgrade")]
static class Conflagration_OnUpgrade_Patch
{
    static bool Prefix(Conflagration __instance)
    {
        if (RevertAnthony.IsVersion("conflagration", "v0.99.1", "v0.103.2"))
        {
            // v0.99.1/v0.103.2: +1 CalculationBase, +1 ExtraDamage (current: +1 Repeat)
            __instance.DynamicVars.CalculationBase.UpgradeValueBy(1m);
            __instance.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
