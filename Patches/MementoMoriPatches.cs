using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// MementoMori v0.99.1 vs current
// v0.99.1: CalculationBase 8, ExtraDamage 4
// Current:  CalculationBase 9, ExtraDamage 4

[HarmonyPatch(typeof(MementoMori), "get_CanonicalVars")]
static class MementoMori_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("memento-mori", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new CalculationBaseVar(8m),
                new ExtraDamageVar(4m),
                new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature _) => CombatManager.Instance.History.Entries.OfType<CardDiscardedEntry>().Count((CardDiscardedEntry e) => e.HappenedThisTurn(card.CombatState) && e.Card.Owner == card.Owner)),
            };
            return false;
        }
        return true;
    }
}
