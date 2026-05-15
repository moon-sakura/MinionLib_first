using BaseLib.Abstracts;
using BaseLib.Hooks;
using MinionLib.Action;

namespace MinionLib.BaseLibAdapters;

public abstract class CustomActionModel :
    ActionModel,
    ICustomPower,
    ILocalizationProvider,
    IHealthBarForecastSource
{
    public virtual string? CustomPackedIconPath => null;

    public virtual string? CustomBigIconPath => null;

    public virtual string? CustomBigBetaIconPath => null;

    /// <summary>
    ///     Override when this power wants to contribute extra health bar forecast segments.
    ///     Default is empty (no forecast).
    /// </summary>
    public virtual IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(
        HealthBarForecastContext context)
    {
        return [];
    }

    /// <summary>
    ///     Override this to define localization directly in your class.
    ///     You are recommended to return a PowerLoc<seealso cref="T:BaseLib.Abstracts.PowerLoc" />.
    /// </summary>
    public virtual List<(string, string)>? Localization => null;
}
