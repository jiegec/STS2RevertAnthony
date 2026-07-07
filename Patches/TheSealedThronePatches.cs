using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// TheSealedThrone v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: upgrade: Innate
// v0.107.1: upgrade: -1 cost

[HarmonyPatch(typeof(TheSealedThrone), "OnUpgrade")]
static class TheSealedThrone_OnUpgrade_Patch
{
    static bool Prefix(TheSealedThrone __instance)
    {
        if (RevertAnthony.IsVersion("the-sealed-throne", "v0.99.1", "v0.103.2"))
        {
            __instance.AddKeyword(CardKeyword.Innate);
            return false;
        }
        return true;
    }
}
