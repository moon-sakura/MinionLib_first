using Godot;
using MegaCrit.Sts2.Core.Models;
using MinionLib.Component.Core;

namespace MinionLib.Component.Interfaces;

public interface IComponentsCardModel
{
    CardModel AsCardModel => (CardModel)this;

    IReadOnlyList<ICardComponent> Components { get; }

    Color? GlowColor => null;

    ICardComponent? AddComponent<T>(T incoming, bool allowMerge = true, bool isUpgrade = false)
        where T : class, ICardComponent
    {
        return ApplyComponent(incoming, new ApplyComponentOptions(
            allowMerge,
            false,
            isUpgrade
        ));
    }

    ICardComponent? SubtractComponent<T>(T incoming, bool isUpgrade = false)
        where T : class, ICardComponent
    {
        return ApplyComponent(incoming, new ApplyComponentOptions(
            true,
            true,
            isUpgrade
        ));
    }

    ICardComponent? ApplyComponent<T>(T incoming, ApplyComponentOptions options)
        where T : class, ICardComponent;

    ICardComponent? RemoveComponent<T>() where T : class, ICardComponent;

    IReadOnlyList<ICardComponent> RemoveComponents<T>() where T : class, ICardComponent;

    bool RefRemoveComponent(ICardComponent component);

    T? GetComponent<T>() where T : class, ICardComponent;

    IReadOnlyList<T> GetComponents<T>() where T : class, ICardComponent;

    # region Deprecated

    [Obsolete(
        "This method is deprecated and should not be called or overridden. Use interface constraints or delegate registry instead.",
        false)]
    Task ComponentCallBack(string name, params object?[] args);

    [Obsolete(
        "This method is deprecated and should not be called or overridden. Use interface constraints or delegate registry instead.",
        false)]
    bool ComponentPredicate(string name, params object?[] args);

    [Obsolete(
        "This method is deprecated and should not be called or overridden. Use interface constraints or delegate registry instead.",
        false)]
    object? ComponentQuery(string name, params object?[] args);

    [Obsolete(
        "This method is deprecated and should not be called or overridden. Use interface constraints or delegate registry instead.",
        false)]
    Task<object?> ComponentQueryAsync(string name, params object?[] args);

    #endregion
}
