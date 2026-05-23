using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

static class Compat
{
    static bool? _isV104OrNewer;

    static PropertyInfo _combatStateProp;
    static PropertyInfo _creatureCombatStateProp;

    static MethodInfo _happenedThisTurn;
    static MethodInfo _soulCreateMethod;
    static MethodInfo _addGeneratedCardsToCombatMethod;
    static MethodInfo _powerCmdApplyV104Enumerable;
    static MethodInfo _powerCmdApplyV104Creature;

    public static bool IsV104OrNewer()
    {
        if (_isV104OrNewer == null)
        {
            _isV104OrNewer = false;
            var version = ReleaseInfoManager.Instance?.ReleaseInfo?.Version;
            if (version != null)
            {
                var parts = version.TrimStart('v').Split('.');
                if (parts.Length >= 2 && int.TryParse(parts[0], out var major) && int.TryParse(parts[1], out var minor))
                    _isV104OrNewer = major > 0 || minor >= 104;
            }
            Log.Info($"RevertAnthony: Version detected as v{version}, isV104OrNewer={_isV104OrNewer}");
        }
        return _isV104OrNewer.Value;
    }

    // ---- CardModel.CombatState (CombatState? -> ICombatState?) ----

    public static object GetCombatState(CardModel card)
    {
        if (_combatStateProp == null)
        {
            _combatStateProp = typeof(CardModel).GetProperty("CombatState",
                BindingFlags.Public | BindingFlags.Instance);
            Log.Info($"RevertAnthony: Compat Resolved CardModel.CombatState, return type={_combatStateProp?.PropertyType?.Name ?? "null"}");
        }
        return _combatStateProp?.GetValue(card);
    }

    public static object GetCombatState(Creature creature)
    {
        if (_creatureCombatStateProp == null)
        {
            _creatureCombatStateProp = typeof(Creature).GetProperty("CombatState",
                BindingFlags.Public | BindingFlags.Instance);
            Log.Info($"RevertAnthony: Compat Resolved Creature.CombatState, return type={_creatureCombatStateProp?.PropertyType?.Name ?? "null"}");
        }
        return _creatureCombatStateProp?.GetValue(creature);
    }

    // ---- HappenedThisTurn (CombatState? -> ICombatState?) ----

    public static bool HappenedThisTurn(this CombatHistoryEntry entry, object state)
    {
        if (_happenedThisTurn == null)
        {
            _happenedThisTurn = typeof(CombatHistoryEntry).GetMethod("HappenedThisTurn",
                BindingFlags.Public | BindingFlags.Instance);
            Log.Info($"RevertAnthony: Compat Resolved CombatHistoryEntry.HappenedThisTurn, param type={_happenedThisTurn?.GetParameters()[0]?.ParameterType?.Name ?? "null"}");
        }
        return (bool)_happenedThisTurn.Invoke(entry, new[] { state });
    }

    // ---- Soul.Create (CombatState -> ICombatState) ----

    public static IEnumerable<Soul> SoulCreate(Player owner, int amount, object combatState)
    {
        if (_soulCreateMethod == null)
        {
            _soulCreateMethod = typeof(Soul).GetMethod("Create", BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(Player), typeof(int), typeof(CombatState) }, null);
            if (_soulCreateMethod != null)
                Log.Info("RevertAnthony: Compat Resolved Soul.Create with CombatState param");
            else
            {
                _soulCreateMethod = typeof(Soul).GetMethod("Create", BindingFlags.Public | BindingFlags.Static, null,
                    new[] { typeof(Player), typeof(int), typeof(Soul).Assembly.GetType("MegaCrit.Sts2.Core.Combat.ICombatState") }, null);
                if (_soulCreateMethod != null)
                    Log.Info("RevertAnthony: Compat Resolved Soul.Create with ICombatState param");
            }
        }
        return (IEnumerable<Soul>)_soulCreateMethod.Invoke(null, new[] { owner, amount, combatState });
    }

    // ---- CardPileCmd.AddGeneratedCardsToCombat (bool addedByPlayer -> Player? creator) ----

    public static Task<IReadOnlyList<CardPileAddResult>> AddGeneratedCardsToCombatCompat(
        IEnumerable<CardModel> cards, PileType pileType, Player owner, CardPilePosition position)
    {
        if (_addGeneratedCardsToCombatMethod == null)
        {
            _addGeneratedCardsToCombatMethod = typeof(CardPileCmd).GetMethod("AddGeneratedCardsToCombat",
                BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(IEnumerable<CardModel>), typeof(PileType), typeof(bool), typeof(CardPilePosition) }, null);
            if (_addGeneratedCardsToCombatMethod != null)
                Log.Info("RevertAnthony: Compat Resolved CardPileCmd.AddGeneratedCardsToCombat with bool param");
            else
            {
                _addGeneratedCardsToCombatMethod = typeof(CardPileCmd).GetMethod("AddGeneratedCardsToCombat",
                    BindingFlags.Public | BindingFlags.Static, null,
                    new[] { typeof(IEnumerable<CardModel>), typeof(PileType), typeof(Player), typeof(CardPilePosition) }, null);
                if (_addGeneratedCardsToCombatMethod != null)
                    Log.Info("RevertAnthony: Compat Resolved CardPileCmd.AddGeneratedCardsToCombat with Player param");
            }
        }
        var args = _addGeneratedCardsToCombatMethod.GetParameters();
        var invokeArgs = args[2].ParameterType == typeof(bool)
            ? new object[] { cards, pileType, true, position }
            : new object[] { cards, pileType, owner, position };
        return (Task<IReadOnlyList<CardPileAddResult>>)_addGeneratedCardsToCombatMethod.Invoke(null, invokeArgs);
    }

    // ---- PowerCmd.Apply (v0.104.0 added PlayerChoiceContext as first param) ----

    static MethodInfo FindV104ApplyMethod(Type firstTargetParamType)
    {
        if (_powerCmdApplyV104Enumerable == null)
        {
            _powerCmdApplyV104Enumerable = typeof(PowerCmd).GetMethod("Apply", 1, BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool) }, null);
            if (_powerCmdApplyV104Enumerable != null)
                Log.Info("RevertAnthony: Compat Resolved PowerCmd.Apply<T>(PlayerChoiceContext, IEnumerable<Creature>, ...)");
        }
        if (_powerCmdApplyV104Creature == null)
        {
            _powerCmdApplyV104Creature = typeof(PowerCmd).GetMethod("Apply", 1, BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), typeof(Creature), typeof(CardModel), typeof(bool) }, null);
            if (_powerCmdApplyV104Creature != null)
                Log.Info("RevertAnthony: Compat Resolved PowerCmd.Apply<T>(PlayerChoiceContext, Creature, ...)");
        }
        return firstTargetParamType == typeof(IEnumerable<Creature>)
            ? _powerCmdApplyV104Enumerable : _powerCmdApplyV104Creature;
    }

    static Task ApplyPowerV1032Enumerable<T>(IEnumerable<Creature> targets, decimal amount,
        Creature applier, CardModel cardSource, bool silent) where T : PowerModel
        => PowerCmd.Apply<T>(targets, amount, applier, cardSource, silent);

    static Task ApplyPowerV1032Creature<T>(Creature target, decimal amount,
        Creature applier, CardModel cardSource, bool silent) where T : PowerModel
        => PowerCmd.Apply<T>(target, amount, applier, cardSource, silent);

    static Task ApplyPowerV104Enumerable<T>(PlayerChoiceContext ctx, IEnumerable<Creature> targets,
        decimal amount, Creature applier, CardModel cardSource, bool silent) where T : PowerModel
        => (Task)FindV104ApplyMethod(typeof(IEnumerable<Creature>)).MakeGenericMethod(typeof(T))
            .Invoke(null, new object[] { ctx, targets, amount, applier, cardSource, silent });

    static Task ApplyPowerV104Creature<T>(PlayerChoiceContext ctx, Creature target,
        decimal amount, Creature applier, CardModel cardSource, bool silent) where T : PowerModel
        => (Task)FindV104ApplyMethod(typeof(Creature)).MakeGenericMethod(typeof(T))
            .Invoke(null, new object[] { ctx, target, amount, applier, cardSource, silent });

    public static Task ApplyPower<T>(PlayerChoiceContext ctx, IEnumerable<Creature> targets,
        decimal amount, Creature applier, CardModel cardSource, bool silent = false) where T : PowerModel
    {
        if (IsV104OrNewer())
            return ApplyPowerV104Enumerable<T>(ctx, targets, amount, applier, cardSource, silent);
        return ApplyPowerV1032Enumerable<T>(targets, amount, applier, cardSource, silent);
    }

    public static Task ApplyPower<T>(PlayerChoiceContext ctx, Creature target,
        decimal amount, Creature applier, CardModel cardSource, bool silent = false) where T : PowerModel
    {
        if (IsV104OrNewer())
            return ApplyPowerV104Creature<T>(ctx, target, amount, applier, cardSource, silent);
        return ApplyPowerV1032Creature<T>(target, amount, applier, cardSource, silent);
    }
}
