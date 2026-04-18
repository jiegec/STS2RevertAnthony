using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Base CardModel patches for properties not overridden in derived classes

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class CardModel_Rarity_Patch
{
    // Acrobatics v0.99.1: Common (current: Uncommon)
    // Colossus v0.99.1: Rare (current: Uncommon)
    // FollowThrough v0.99.1: Uncommon (current: Common)
    // RipAndTear v0.99.1: Uncommon (current: Event)
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is Acrobatics && RevertAnthony.IsVersion("acrobatics", "v0.99.1"))
        {
            __result = CardRarity.Common;
        }
        else if (__instance is Colossus && RevertAnthony.IsVersion("colossus", "v0.99.1"))
        {
            __result = CardRarity.Rare;
        }
        else if (__instance is FollowThrough && RevertAnthony.IsVersion("follow-through", "v0.99.1"))
        {
            __result = CardRarity.Uncommon;
        }
        else if (__instance is RipAndTear && RevertAnthony.IsVersion("rip-and-tear", "v0.99.1"))
        {
            __result = CardRarity.Uncommon;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class CardModel_CanonicalEnergyCost_Patch
{
    // BorrowedTime v0.99.1: Cost 0 (current: Cost 1)
    // BansheesCry v0.99.1: Cost 6 (current: Cost 9)
    // Break v0.99.1: Cost 2 (current: Cost 1)
    // BundleOfJoy v0.99.1: Cost 2 (current: Cost 1)
    // MinionDiveBomb v0.99.1: Cost 1 (current: Cost 0)
    // Seance v0.99.1: Cost 0 (current: Cost 1)
    // Voltaic v0.99.1: Cost 3 (current: Cost 2)
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is BorrowedTime && RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            __result = 0;
        }
        else if (__instance is BansheesCry && RevertAnthony.IsVersion("banshees-cry", "v0.99.1"))
        {
            __result = 6;
        }
        else if (__instance is Break && RevertAnthony.IsVersion("break", "v0.99.1"))
        {
            __result = 2;
        }
        else if (__instance is BundleOfJoy && RevertAnthony.IsVersion("bundle-of-joy", "v0.99.1"))
        {
            __result = 2;
        }
        else if (__instance is MinionDiveBomb && RevertAnthony.IsVersion("minion-dive-bomb", "v0.99.1"))
        {
            __result = 1;
        }
        else if (__instance is Seance && RevertAnthony.IsVersion("seance", "v0.99.1"))
        {
            __result = 0;
        }
        else if (__instance is Voltaic && RevertAnthony.IsVersion("voltaic", "v0.99.1"))
        {
            __result = 2;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
static class CardModel_TargetType_Patch
{
    // Begone v0.99.1: AnyEnemy (current: Self)
    // FollowThrough v0.99.1: AllEnemies (current: AnyEnemy)
    static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
        {
            __result = TargetType.AnyEnemy;
        }
        else if (__instance is FollowThrough && RevertAnthony.IsVersion("follow-through", "v0.99.1"))
        {
            __result = TargetType.AllEnemies;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "CardType", MethodType.Getter)]
static class CardModel_CardType_Patch
{
    // Begone v0.99.1: Attack (current: Skill)
    static void Postfix(CardModel __instance, ref CardType __result)
    {
        if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
        {
            __result = CardType.Attack;
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class CardModel_Description_Patch
{
    // Only cards where description TEXT differs between versions
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Arsenal && RevertAnthony.IsVersion("arsenal", "v0.99.1"))
            __result = new LocString("cards", "ARSENAL_V0991.description");
        else if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
            __result = new LocString("cards", "BEGONE_V0991.description");
        else if (__instance is BladeOfInk && RevertAnthony.IsVersion("blade-of-ink", "v0.99.1"))
            __result = new LocString("cards", "BLADE_OF_INK_V0991.description");
        else if (__instance is BorrowedTime && RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
            __result = new LocString("cards", "BORROWED_TIME_V0991.description");
        else if (__instance is Charge && RevertAnthony.IsVersion("charge", "v0.99.1"))
            __result = new LocString("cards", "CHARGE_V0991.description");
        else if (__instance is Cinder && RevertAnthony.IsVersion("cinder", "v0.99.1"))
            __result = new LocString("cards", "CINDER_V0991.description");
        else if (__instance is Dominate && RevertAnthony.IsVersion("dominate", "v0.99.1"))
            __result = new LocString("cards", "DOMINATE_V0991.description");
        else if (__instance is ExpectAFight && RevertAnthony.IsVersion("expect-a-fight", "v0.99.1"))
            __result = new LocString("cards", "EXPECT_A_FIGHT_V0991.description");
        else if (__instance is FollowThrough && RevertAnthony.IsVersion("follow-through", "v0.99.1"))
            __result = new LocString("cards", "FOLLOW_THROUGH_V0991.description");
        else if (__instance is Glow && RevertAnthony.IsVersion("glow", "v0.99.1"))
            __result = new LocString("cards", "GLOW_V0991.description");
        else if (__instance is GraveWarden && RevertAnthony.IsVersion("grave-warden", "v0.99.1"))
            __result = new LocString("cards", "GRAVE_WARDEN_V0991.description");
        else if (__instance is GuidingStar && RevertAnthony.IsVersion("guiding-star", "v0.99.1"))
            __result = new LocString("cards", "GUIDING_STAR_V0991.description");
        else if (__instance is HuddleUp && RevertAnthony.IsVersion("huddle-up", "v0.99.1"))
            __result = new LocString("cards", "HUDDLE_UP_V0991.description");
        else if (__instance is LeadingStrike && RevertAnthony.IsVersion("leading-strike", "v0.99.1"))
            __result = new LocString("cards", "LEADING_STRIKE_V0991.description");
        else if (__instance is Spite && RevertAnthony.IsVersion("spite", "v0.99.1"))
            __result = new LocString("cards", "SPITE_V0991.description");
        else if (__instance is SpoilsOfBattle && RevertAnthony.IsVersion("spoils-of-battle", "v0.99.1"))
            __result = new LocString("cards", "SPOILS_OF_BATTLE_V0991.description");
        else if (__instance is Stoke && RevertAnthony.IsVersion("stoke", "v0.99.1"))
            __result = new LocString("cards", "STOKE_V0991.description");
    }
}
