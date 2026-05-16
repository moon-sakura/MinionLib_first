using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace MinionLib.RightClick.Easy;

public class EasyRightClickableModelHandler : IRightClickHandler
{
    public bool Handle(RightClickContext context)
    {
        if (context.Model is not IEasyRightClickableModel rightClickableModel) return false;
        if (!IsValidType(context.Model, context.Player)) return false;
        if (!rightClickableModel.CanHandleRightClickLocal(context)) return false;

        var queuedAction = new EasyRightClickCardAction(context, CombatManager.Instance.IsInProgress);
        RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(queuedAction);
        return true;
    }

    private static bool IsValidType(AbstractModel model, Player player)
    {
        return model switch
        {
            CardModel card => card.Owner == player,
            RelicModel relic => relic.Owner == player,
            PowerModel power => power.Owner.Player == player || power.Owner.PetOwner == player || power.Owner.IsEnemy,
            PotionModel potion => potion.Owner == player,
            _ => false
        };
    }
}
