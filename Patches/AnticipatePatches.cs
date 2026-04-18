using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// ============================================
// Anticipate - v0.99.1 vs v0.103.2
// Old: Gain 3(5) Dexterity this turn
// New: Gain 2(3) Dexterity this turn
// ============================================

[HarmonyPatch(typeof(Anticipate), "get_CanonicalVars")]
static class Anticipate_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("anticipate", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new PowerVar<DexterityPower>(3m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Anticipate), "OnUpgrade")]
static class Anticipate_OnUpgrade_Patch
{
    static bool Prefix(Anticipate __instance)
    {
        if (RevertAnthony.IsVersion("anticipate", "v0.99.1"))
        {
            __instance.DynamicVars.Dexterity.UpgradeValueBy(2m);
            return false;
        }
        return true;
    }
}
