using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// CrescentSpear v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: CalculationBase 6
// v0.107.1: CalculationBase 8

[HarmonyPatch(typeof(CrescentSpear), "CanonicalVars", MethodType.Getter)]
static class CrescentSpear_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("crescent-spear", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new CalculationBaseVar(6m),
                new ExtraDamageVar(2m),
                new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature _) => card.Owner.PlayerCombatState.AllCards.Count((CardModel c) => c.CanonicalStarCost >= 0 || c.HasStarCostX)),
            };
            return false;
        }
        return true;
    }
}
