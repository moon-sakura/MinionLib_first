using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MinionLib.Minion;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;
using STS2RitsuLib.Scaffolding.Visuals.StateMachine;

namespace MinionLib.RitsuAdapters;

#pragma warning disable CS0618
/// <summary>
///     Base <see cref="T:MegaCrit.Sts2.Core.Models.MonsterModel" /> for mods:
///     <see cref="T:STS2RitsuLib.Scaffolding.Content.Patches.IModMonsterAssetOverrides" /> supplies the visuals scene
///     path; override <see cref="M:STS2RitsuLib.Scaffolding.Content.ModMonsterTemplate.TryCreateCreatureVisuals" /> to
///     build <see cref="T:MegaCrit.Sts2.Core.Nodes.Combat.NCreatureVisuals" /> in code instead.
///     Use <see cref="T:STS2RitsuLib.Scaffolding.Godot.RitsuGodotNodeFactories" /> for explicit <c>CreateFromResource</c>
///     / <c>CreateFromScenePath</c>
///     construction. Register with <c>ModContentRegistry.RegisterMonster&lt;T&gt;()</c> or <c>Monster&lt;T&gt;()</c> on
///     the pack builder.
///     Mod 怪物的基础 <see cref="T:MegaCrit.Sts2.Core.Models.MonsterModel" />：
///     <see cref="T:STS2RitsuLib.Scaffolding.Content.Patches.IModMonsterAssetOverrides" /> 提供视觉场景
///     路径；也可以重写 <see cref="M:STS2RitsuLib.Scaffolding.Content.ModMonsterTemplate.TryCreateCreatureVisuals" /> 直接用代码构建
///     <see cref="T:MegaCrit.Sts2.Core.Nodes.Combat.NCreatureVisuals" />。
///     如需显式 <see cref="T:STS2RitsuLib.Scaffolding.Godot.RitsuGodotNodeFactories" /> 构造 <c>CreateFromResource</c> /
///     <c>CreateFromScenePath</c>，
///     请在包构建器上通过 <c>ModContentRegistry.RegisterMonster&lt;T&gt;()</c> 或 <c>Monster&lt;T&gt;()</c>
///     注册。
/// </summary>
/// <remarks>
///     When the monster's visuals should use a
///     <see cref="T:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachine" /> for combat triggers, override
///     <see
///         cref="M:STS2RitsuLib.Scaffolding.Content.ModMonsterTemplate.SetupCustomCombatAnimationStateMachine(Godot.Node,MegaCrit.Sts2.Core.Models.MonsterModel)" />
///     to drive the creature with a
///     <see cref="T:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachine" /> (the same state-machine pipeline
///     used by
///     <see cref="T:STS2RitsuLib.Scaffolding.Characters.ModCharacterTemplate`3" />).
///     <see cref="T:STS2RitsuLib.Scaffolding.Characters.ModCharacterTemplate`3" />
///     当怪物视觉应使用 <see cref="T:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachine" /> 处理战斗 trigger 时，请重写
///     <see
///         cref="M:STS2RitsuLib.Scaffolding.Content.ModMonsterTemplate.SetupCustomCombatAnimationStateMachine(Godot.Node,MegaCrit.Sts2.Core.Models.MonsterModel)" />
///     ，用 <see cref="T:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachine" /> 驱动该生物（与
///     使用的状态机管线相同）。
/// </remarks>
public abstract class ModMinionTemplate :
    MinionModel,
    IModMonsterAssetOverrides,
    IModCreatureVisualsFactory,
    IModMonsterCreatureVisualsFactory,
    IModCreatureAnimatorFactory,
    IModCreatureCombatAnimationStateMachineFactory,
    IModNonSpineAnimationStateMachineFactory
{
    CreatureAnimator? IModCreatureAnimatorFactory.TryCreateCreatureAnimator(MegaSprite controller)
    {
        return SetupCustomCreatureAnimator(controller);
    }

    ModAnimStateMachine? IModCreatureCombatAnimationStateMachineFactory.TryCreateCombatAnimationStateMachine(
        Node visualsRoot)
    {
        return ResolveCombatAnimationStateMachine(visualsRoot);
    }

    NCreatureVisuals? IModCreatureVisualsFactory.TryCreateCreatureVisuals()
    {
        return TryCreateCreatureVisuals();
    }

    /// <inheritdoc />
    public virtual MonsterAssetProfile AssetProfile => MonsterAssetProfile.Empty;

    /// <inheritdoc />
    public virtual string? CustomVisualsPath => AssetProfile.VisualsScenePath;

    NCreatureVisuals? IModMonsterCreatureVisualsFactory.TryCreateCreatureVisuals()
    {
        return TryCreateCreatureVisuals();
    }

    ModAnimStateMachine? IModNonSpineAnimationStateMachineFactory.TryCreateNonSpineAnimationStateMachine(
        Node visualsRoot)
    {
        return ResolveCombatAnimationStateMachine(visualsRoot);
    }

    private ModAnimStateMachine? ResolveCombatAnimationStateMachine(Node visualsRoot)
    {
        return SetupCustomCombatAnimationStateMachine(visualsRoot, this) ??
               SetupCustomNonSpineAnimationStateMachine(visualsRoot, this);
    }

    /// <summary>
    ///     Non-null value becomes combat visuals; otherwise paths (
    ///     <see cref="P:STS2RitsuLib.Scaffolding.Content.ModMonsterTemplate.CustomVisualsPath" /> / vanilla) apply.
    ///     返回非 null 值时作为战斗视觉；否则使用路径（<see cref="P:STS2RitsuLib.Scaffolding.Content.ModMonsterTemplate.CustomVisualsPath" /> /
    ///     原版）。
    /// </summary>
    protected virtual NCreatureVisuals? TryCreateCreatureVisuals()
    {
        return null;
    }

    /// <summary>
    ///     Optional override producing a fully wired Spine <see cref="T:MegaCrit.Sts2.Core.Animation.CreatureAnimator" />
    ///     (state graph for idle /
    ///     hit / attack / cast / die / relaxed). Return <see langword="null" /> to defer to vanilla
    ///     <see
    ///         cref="M:MegaCrit.Sts2.Core.Models.MonsterModel.GenerateAnimator(MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite)" />
    ///     . Prefer
    ///     <see
    ///         cref="M:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachines.Standard(MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite,System.String,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean)" />
    ///     to
    ///     match baselib semantics.
    ///     可选重写，用于生成已完整接线的 Spine <see cref="T:MegaCrit.Sts2.Core.Animation.CreatureAnimator" />（idle / hit / attack / cast /
    ///     die /
    ///     relaxed 状态图）。返回 <see langword="null" /> 则交给原版
    ///     <see
    ///         cref="M:MegaCrit.Sts2.Core.Models.MonsterModel.GenerateAnimator(MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite)" />
    ///     。
    ///     优先使用
    ///     <see
    ///         cref="M:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachines.Standard(MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite,System.String,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean)" />
    ///     以匹配 baselib 语义。
    /// </summary>
    /// <param name="controller">
    ///     Spine controller attached to the monster's combat visuals.
    ///     附加在怪物战斗视觉上的 Spine controller。
    /// </param>
    protected virtual CreatureAnimator? SetupCustomCreatureAnimator(MegaSprite controller)
    {
        return null;
    }

    /// <summary>
    ///     Optional override producing a <see cref="T:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachine" />
    ///     for the monster's combat visuals (any
    ///     animation backend, including Spine via
    ///     <see
    ///         cref="M:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachineBuilder.BuildSpine(MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite)" />
    ///     ). Return
    ///     <see langword="null" /> to defer to vanilla Spine <see cref="T:MegaCrit.Sts2.Core.Animation.CreatureAnimator" />
    ///     triggers or the vanilla
    ///     single-shot playback path when there is no Spine animator.
    ///     可选重写，用于为怪物战斗视觉生成 <see cref="T:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachine" />（可使用任意动画后端，包括通过
    ///     <see
    ///         cref="M:STS2RitsuLib.Scaffolding.Visuals.StateMachine.ModAnimStateMachineBuilder.BuildSpine(MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite)" />
    ///     使用 Spine）。返回 <see langword="null" /> 时，
    ///     会回退到原版 Spine <see cref="T:MegaCrit.Sts2.Core.Animation.CreatureAnimator" /> 触发，或在没有 Spine animator 时回退到原版单次播放路径。
    /// </summary>
    /// <param name="visualsRoot">
    ///     Combat visuals root node.
    ///     战斗视觉根节点。
    /// </param>
    /// <param name="monster">
    ///     Monster model (always <see langword="this" />, exposed for convenience).
    ///     怪物模型（始终为 <see langword="this" />，仅为方便而暴露）。
    /// </param>
    protected virtual ModAnimStateMachine? SetupCustomCombatAnimationStateMachine(
        Node visualsRoot,
        MonsterModel monster)
    {
        return null;
    }

    /// <inheritdoc
    ///     cref="M:STS2RitsuLib.Scaffolding.Content.ModMonsterTemplate.SetupCustomCombatAnimationStateMachine(Godot.Node,MegaCrit.Sts2.Core.Models.MonsterModel)" />
    [Obsolete("Override SetupCustomCombatAnimationStateMachine instead.")]
    protected virtual ModAnimStateMachine? SetupCustomNonSpineAnimationStateMachine(
        Node visualsRoot,
        MonsterModel monster)
    {
        return SetupCustomCombatAnimationStateMachine(visualsRoot, monster);
    }
}
