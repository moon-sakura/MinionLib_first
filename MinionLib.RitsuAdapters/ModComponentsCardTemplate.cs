using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MinionLib.Component;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace MinionLib.RitsuAdapters;

/// <summary>
///     Base <see cref="T:MegaCrit.Sts2.Core.Models.CardModel" /> for mods: hooks extra hover tips (keywords) and optional
///     asset overrides via
///     <see cref="T:STS2RitsuLib.Scaffolding.Content.Patches.IModCardAssetOverrides" />. For gold/red hand highlights
///     (Evil Eye / Osty-style), override
///     <c>ShouldGlowGoldInternal</c> / <c>ShouldGlowRedInternal</c> or use
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.ModCardHandGlowRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandGlow&lt;TCard&gt;()</c> with
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.CardModelHandGlowExtensions" />.
///     For arbitrary hand-highlight colors use
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandOutline&lt;TCard&gt;()</c>.
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     Mod 卡牌的基础 <see cref="T:MegaCrit.Sts2.Core.Models.CardModel" />：接入额外悬停提示（关键词），并通过
///     <see cref="T:STS2RitsuLib.Scaffolding.Content.Patches.IModCardAssetOverrides" />
///     提供可选资源覆盖。对于金色/红色手牌高亮（Evil Eye / Osty 风格），请重写 <c>ShouldGlowGoldInternal</c> / <c>ShouldGlowRedInternal</c>，或结合
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.ModCardHandGlowRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandGlow&lt;TCard&gt;()</c> 使用
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.CardModelHandGlowExtensions" />。对于任意手牌高亮颜色，请使用
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandOutline&lt;TCard&gt;()</c>。
/// </summary>
/// <summary>
///     Base <see cref="T:MegaCrit.Sts2.Core.Models.CardModel" /> for mods: hooks extra hover tips (keywords) and optional
///     asset overrides via
///     <see cref="T:STS2RitsuLib.Scaffolding.Content.Patches.IModCardAssetOverrides" />. For gold/red hand highlights
///     (Evil Eye / Osty-style), override
///     <c>ShouldGlowGoldInternal</c> / <c>ShouldGlowRedInternal</c> or use
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.ModCardHandGlowRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandGlow&lt;TCard&gt;()</c> with
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.CardModelHandGlowExtensions" />.
///     For arbitrary hand-highlight colors use
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandOutline&lt;TCard&gt;()</c>.
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     Mod 卡牌的基础 <see cref="T:MegaCrit.Sts2.Core.Models.CardModel" />：接入额外悬停提示（关键词），并通过
///     <see cref="T:STS2RitsuLib.Scaffolding.Content.Patches.IModCardAssetOverrides" />
///     提供可选资源覆盖。对于金色/红色手牌高亮（Evil Eye / Osty 风格），请重写 <c>ShouldGlowGoldInternal</c> / <c>ShouldGlowRedInternal</c>，或结合
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.ModCardHandGlowRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandGlow&lt;TCard&gt;()</c> 使用
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandGlow.CardModelHandGlowExtensions" />。对于任意手牌高亮颜色，请使用
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     <see cref="T:STS2RitsuLib.Scaffolding.Cards.HandOutline.ModCardHandOutlineRegistry" /> /
///     <c>ModContentRegistry.RegisterCardHandOutline&lt;TCard&gt;()</c>。
/// </summary>
public abstract class ModComponentsCardTemplate(
    int baseCost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true) :
    ComponentsCardModel(baseCost, type, rarity, target, showInCardLibrary),
    IModCardAssetOverrides,
    IModCardFrameMaterialOverride,
    IModCardBannerMaterialOverride
{
    /// <summary>
    ///     Keyword declarations seeded onto every instance of this card on first
    ///     <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.Keywords" />
    ///     access. Intentionally kept as a separate channel from vanilla
    ///     <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.CanonicalKeywords" /> so derived mods can still override
    ///     <c>CanonicalKeywords</c> for vanilla keywords without accidentally dropping their mod keyword
    ///     declarations. Each string resolves as a registered mod keyword id first, then as a vanilla
    ///     <see cref="T:MegaCrit.Sts2.Core.Entities.Cards.CardKeyword" /> enum name, and is unioned into
    ///     <c>CardModel.Keywords</c> right after the
    ///     vanilla canonical seed runs, so runtime additions/removals and <c>DeepCloneFields</c> preserve them.
    ///     首次访问 <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.Keywords" /> 时种入每个卡牌实例的关键词声明。它刻意与原版
    ///     <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.CanonicalKeywords" /> 分离，这样派生 mod 仍可重写
    ///     <c>CanonicalKeywords</c> 来声明原版关键词，而不会意外丢失自己的 mod 关键词
    ///     声明。每个字符串会先按已注册的 mod 关键词 id 解析，再按原版
    ///     <see cref="T:MegaCrit.Sts2.Core.Entities.Cards.CardKeyword" /> 枚举名解析，并在原版规范种入执行后合并进 <c>CardModel.Keywords</c>
    ///     ，因此运行时增删和 <c>DeepCloneFields</c> 都会保留它们。
    /// </summary>
    protected virtual IEnumerable<string> RegisteredKeywordIds => Array.Empty<string>();

    /// <summary>
    ///     Card tag declarations seeded onto every instance when <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.Tags" /> is
    ///     first
    ///     materialized. Each string resolves as a registered mod card-tag id first, then as a vanilla
    ///     <see cref="T:MegaCrit.Sts2.Core.Entities.Cards.CardTag" /> enum name, and is unioned into the same backing set as
    ///     <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.CanonicalTags" />.
    ///     首次实体化 <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.Tags" /> 时种入每个卡牌实例的卡牌标签声明。每个字符串会先按已注册的
    ///     mod 卡牌标签 id 解析，再按原版 <see cref="T:MegaCrit.Sts2.Core.Entities.Cards.CardTag" /> 枚举名解析，并合并到与
    ///     <see cref="P:MegaCrit.Sts2.Core.Models.CardModel.CanonicalTags" /> 相同的后备集合。
    /// </summary>
    protected virtual IEnumerable<string> RegisteredCardTagIds => Array.Empty<string>();

    /// <summary>
    ///     Extra hover tips appended after keyword-derived tips.
    ///     在关键词派生提示之后追加的额外悬浮提示。
    /// </summary>
    protected virtual IEnumerable<IHoverTip> AdditionalHoverTips => Array.Empty<IHoverTip>();

    /// <inheritdoc />
    protected sealed override IEnumerable<IHoverTip> ExtraHoverTipsC => AdditionalHoverTips.ToArray();

    /// <inheritdoc />
    public virtual CardAssetProfile AssetProfile => CardAssetProfile.Empty;

    /// <inheritdoc />
    public virtual string? CustomPortraitPath => AssetProfile.PortraitPath;

    /// <inheritdoc />
    public virtual string? CustomBetaPortraitPath => AssetProfile.BetaPortraitPath;

    /// <inheritdoc />
    public virtual string? CustomFramePath => AssetProfile.FramePath;

    /// <inheritdoc />
    public virtual string? CustomPortraitBorderPath => AssetProfile.PortraitBorderPath;

    /// <inheritdoc />
    public virtual string? CustomEnergyIconPath => AssetProfile.EnergyIconPath;

    /// <inheritdoc />
    public virtual string? CustomFrameMaterialPath => AssetProfile.FrameMaterialPath;

    /// <inheritdoc />
    public virtual string? CustomOverlayScenePath => AssetProfile.OverlayScenePath;

    /// <inheritdoc />
    public virtual string? CustomBannerTexturePath => AssetProfile.BannerTexturePath;

    /// <inheritdoc />
    public virtual string? CustomBannerMaterialPath => AssetProfile.BannerMaterialPath;

    /// <inheritdoc />
    public virtual Material? CustomBannerMaterial => AssetProfile.BannerMaterial;

    /// <inheritdoc />
    public virtual Material? CustomFrameMaterial => AssetProfile.FrameMaterial;

    /// <summary>
    ///     Internal accessor for the mod-keyword seeding patch.
    ///     供 mod 关键词种入补丁使用的内部访问器。
    /// </summary>
    internal IEnumerable<string> EnumerateRegisteredKeywordIds()
    {
        return RegisteredKeywordIds;
    }

    /// <summary>
    ///     Internal accessor for the mod card-tag seeding patch.
    ///     供 mod 卡牌标签种入补丁使用的内部访问器。
    /// </summary>
    internal IEnumerable<string> EnumerateRegisteredCardTagIds()
    {
        return RegisteredCardTagIds;
    }
}
