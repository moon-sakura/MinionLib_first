using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MinionLib.RightClick.Easy;

public interface IEasyRightClickableModel
{
    bool CanHandleRightClickLocal(RightClickContext context)
    {
        return true;
    }

    Task OnRightClick(PlayerChoiceContext choiceContext, RightClickContext clickContext);
}

public interface IEasyRightClickableCard : IEasyRightClickableModel;

public interface IEasyRightClickableRelic : IEasyRightClickableModel;

public interface IEasyRightClickablePower : IEasyRightClickableModel;

public interface IEasyRightClickableMonster : IEasyRightClickableModel;
