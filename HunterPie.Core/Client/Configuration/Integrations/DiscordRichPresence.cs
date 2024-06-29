﻿using HunterPie.Core.Architecture;
using HunterPie.Core.Domain.Enums;
using HunterPie.Core.Settings;
using HunterPie.Core.Settings.Annotations;
using HunterPie.Core.Settings.Common;

namespace HunterPie.Core.Client.Configuration.Integrations;

[Configuration("DISCORD_RPC_STRING", "ICON_RPC", availableGames: GameProcess.MonsterHunterWorld | GameProcess.MonsterHunterRise | GameProcess.MonsterHunterRiseSunbreakDemo)]
public class DiscordRichPresence : ISettings
{
    [ConfigurationProperty("DRPC_ENABLE_RICH_PRESENCE", group: CommonConfigurationGroups.GENERAL)]
    [ConfigurationCondition]
    public Observable<bool> EnableRichPresence { get; set; } = true;

    [ConfigurationProperty("DRPC_ENABLE_SHOW_CHARACTER_INFO", group: CommonConfigurationGroups.CUSTOMIZATIONS)]
    public Observable<bool> ShowCharacterInfo { get; set; } = true;

    [ConfigurationProperty("DRPC_ENABLE_SHOW_MONSTER_HEALTH", group: CommonConfigurationGroups.CUSTOMIZATIONS)]
    public Observable<bool> ShowMonsterHealth { get; set; } = true;

    [ConfigurationProperty("ENABLE_LOBBY_ID_SHARING", group: CommonConfigurationGroups.GENERAL, availableGames: GameProcess.MonsterHunterRise)]
    [ConfigurationCondition]
    public Observable<bool> EnableLobbyIdSharing { get; set; } = true;

    [ConfigurationProperty("DISCORD_WEBHOOK_URL", group: CommonConfigurationGroups.CUSTOMIZATIONS, availableGames: GameProcess.MonsterHunterRise)]
    public Observable<string> LobbyIdWebHook { get; set; } = "";
}
