using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// DeathMarch v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: CalculationBase 8, ExtraDamage 3, upgrade: +1 CalcBase, +1 ExtraDmg
// v0.107.1: CalculationBase 8, ExtraDamage 4, upgrade: +1 CalcBase, +2 ExtraDmg

[HarmonyPatch(typeof(DeathMarch), "CanonicalVars", MethodType.Getter)]
static class DeathMarch_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("death-march", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new CalculationBaseVar(8m),
                new ExtraDamageVar(3m),
                new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature _) => CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>().Count((CardDrawnEntry e) => e.HappenedThisTurn(card.CombatState) && e.Actor == card.Owner.Creature && !e.FromHandDraw)),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(DeathMarch), "OnUpgrade")]
static class DeathMarch_OnUpgrade_Patch
{
    static bool Prefix(DeathMarch __instance)
    {
        if (RevertAnthony.IsVersion("death-march", "v0.99.1", "v0.103.2"))
        {
            __instance.DynamicVars.CalculationBase.UpgradeValueBy(1m);
            __instance.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
