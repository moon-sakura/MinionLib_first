using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MinionLib.Minion;

namespace MinionLib.BaseLibAdapters;

public abstract class CustomMinionModel : MinionModel, ICustomModel, ISceneConversions
{
    /// <summary>
    ///     Override this or place your scene at res://scenes/creature_visuals/modname-class_name.tscn
    /// </summary>
    public virtual string? CustomVisualPath => null;

    public virtual string? CustomAttackSfx => null;

    public virtual string? CustomCastSfx => null;

    public virtual string? CustomDeathSfx => null;

    public void RegisterSceneConversions()
    {
        CustomVisualPath?.RegisterSceneForConversion<NCreatureVisuals>();
    }

    /// <summary>
    ///     Use if you want to generate creature visuals entirely yourself.
    ///     Otherwise, just override CustomVisualPath.
    /// </summary>
    /// <returns></returns>
    public virtual NCreatureVisuals? CreateCustomVisuals()
    {
        return null;
    }

    /// <summary>
    ///     Override and return a CreatureAnimator if you need to set up states that differ from the default for the monster.
    ///     Using
    ///     <seealso
    ///         cref="M:BaseLib.Abstracts.CustomMonsterModel.SetupAnimationState(MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite,System.String,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean,System.String,System.Boolean)" />
    ///     is suggested.
    /// </summary>
    /// <returns></returns>
    public virtual CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        return null;
    }

    /// <summary>
    ///     If you have a spine animation without all the required animations,
    ///     use this method to set up a controller that will use animations of your choice for each animation.
    ///     Any omitted animation parameters will default to the idle animation.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="idleName"></param>
    /// <param name="deadName"></param>
    /// <param name="deadLoop"></param>
    /// <param name="hitName"></param>
    /// <param name="hitLoop"></param>
    /// <param name="attackName"></param>
    /// <param name="attackLoop"></param>
    /// <param name="castName"></param>
    /// <param name="castLoop"></param>
    /// <returns></returns>
    public static CreatureAnimator SetupAnimationState(
        MegaSprite controller,
        string idleName,
        string? deadName = null,
        bool deadLoop = false,
        string? hitName = null,
        bool hitLoop = false,
        string? attackName = null,
        bool attackLoop = false,
        string? castName = null,
        bool castLoop = false)
    {
        var animState1 = new AnimState(idleName, true);
        var state1 = deadName == null ? animState1 : new AnimState(deadName, deadLoop);
        AnimState animState2;
        if (hitName != null)
            animState2 = new AnimState(hitName, hitLoop)
            {
                NextState = animState1
            };
        else
            animState2 = animState1;
        var state2 = animState2;
        AnimState animState3;
        if (attackName != null)
            animState3 = new AnimState(attackName, attackLoop)
            {
                NextState = animState1
            };
        else
            animState3 = animState1;
        var state3 = animState3;
        AnimState animState4;
        if (castName != null)
            animState4 = new AnimState(castName, castLoop)
            {
                NextState = animState1
            };
        else
            animState4 = animState1;
        var state4 = animState4;
        var creatureAnimator = new CreatureAnimator(animState1, controller);
        creatureAnimator.AddAnyState("Idle", animState1);
        creatureAnimator.AddAnyState("Dead", state1);
        creatureAnimator.AddAnyState("Hit", state2);
        creatureAnimator.AddAnyState("Attack", state3);
        creatureAnimator.AddAnyState("Cast", state4);
        return creatureAnimator;
    }
}
