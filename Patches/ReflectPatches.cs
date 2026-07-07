using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Reflect v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Block 17, upgrade: +4 Block
// v0.107.1: Block 15, upgrade: +5 Block

[HarmonyPatch(typeof(Reflect), "CanonicalVars", MethodType.Getter)]
static class Reflect_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("reflect", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new BlockVar(17m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Reflect), "OnUpgrade")]
static class Reflect_OnUpgrade_Patch
{
    static bool Prefix(Reflect __instance)
    {
        if (RevertAnthony.IsVersion("reflect", "v0.99.1", "v0.103.2"))
        {
            __instance.DynamicVars.Block.UpgradeValueBy(4m);
            return false;
        }
        return true;
    }
}
