using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MinionLib.RightClick.Easy;

public interface IEasyRightClickableCard
{
    bool CanHandleRightClickLocal(RightClickContext context)
    {
        return true;
    }

    Task OnRightClick(PlayerChoiceContext choiceContext, RightClickContext clickContext);
}
