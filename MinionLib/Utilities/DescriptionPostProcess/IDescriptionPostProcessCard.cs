using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MinionLib.Utilities.BetterExtraArgs;

namespace MinionLib.Utilities.DescriptionPostProcess;

public interface IDescriptionPostProcessCard
{
    string PostProcessDescription(
        string description,
        PileType pileType,
        DescriptionPreviewType previewType,
        Creature? target = null);
}
