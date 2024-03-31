﻿using HunterPie.Core.Address.Map;
using HunterPie.Core.Architecture.Events;
using HunterPie.Core.Domain;
using HunterPie.Core.Domain.Interfaces;
using HunterPie.Core.Domain.Process;
using HunterPie.Core.Extensions;
using HunterPie.Core.Game.Entity.Game.Quest;
using HunterPie.Core.Game.Events;
using HunterPie.Integrations.Datasources.MonsterHunterWorld.Definitions;
using HunterPie.Integrations.Datasources.MonsterHunterWorld.Utils;

namespace HunterPie.Integrations.Datasources.MonsterHunterWorld.Entity.Game.Quest;

public class MHWQuest : Scannable, IQuest, IDisposable, IEventDispatcher
{
    /// <inheritdoc />
    public int Id { get; }

    /// <inheritdoc />
    public string Name => string.Empty;

    /// <inheritdoc />
    public QuestType Type { get; }

    private QuestStatus _status;

    /// <inheritdoc />
    public QuestStatus Status
    {
        get => _status;
        private set
        {
            if (value == _status)
                return;

            QuestStatus temp = _status;
            _status = value;
            this.Dispatch(_onQuestStatusChange, new SimpleValueChangeEventArgs<QuestStatus>(temp, value));
        }
    }

    private int _deaths;

    /// <inheritdoc />
    public int Deaths
    {
        get => _deaths;
        private set
        {
            if (value == _deaths)
                return;

            _deaths = value;
            this.Dispatch(_onDeathCounterChange, new CounterChangeEventArgs(value, MaxDeaths));
        }
    }

    /// <inheritdoc />
    public int MaxDeaths { get; private set; }

    /// <inheritdoc />
    public QuestLevel Level { get; }

    /// <inheritdoc />
    public int Stars { get; }

    private readonly SmartEvent<SimpleValueChangeEventArgs<QuestStatus>> _onQuestStatusChange = new();

    /// <inheritdoc />
    public event EventHandler<SimpleValueChangeEventArgs<QuestStatus>> OnQuestStatusChange
    {
        add => _onQuestStatusChange.Hook(value);
        remove => _onQuestStatusChange.Unhook(value);
    }

    private readonly SmartEvent<CounterChangeEventArgs> _onDeathCounterChange = new();

    /// <inheritdoc />
    public event EventHandler<CounterChangeEventArgs> OnDeathCounterChange
    {
        add => _onDeathCounterChange.Hook(value);
        remove => _onDeathCounterChange.Unhook(value);
    }

    public MHWQuest(
        IProcessManager process,
        int id,
        int stars,
        QuestType questType
    ) : base(process)
    {
        Id = id;
        Stars = stars % 10;
        Level = GetLevelByStars(stars);
        Type = questType;
    }

    [ScannableMethod]
    private void GetQuestData()
    {
        MHWQuestStructure quest = Memory.Deref<MHWQuestStructure>(
            AddressMap.GetAbsolute("QUEST_DATA_ADDRESS"),
            AddressMap.GetOffsets("QUEST_DATA_OFFSETS")
        );
        MHWQuestDataStructure questData = Memory.Deref<MHWQuestDataStructure>(
            AddressMap.GetAbsolute("QUEST_DATA_ADDRESS"),
            AddressMap.GetOffsets("QUEST_EXTRA_DATA_OFFSETS")
        );

        Status = quest.State.ToStatus();
        MaxDeaths = questData.MaxDeaths;
        Deaths = questData.Deaths;
    }

    public void Dispose()
    {
        IDisposableExtensions.DisposeAll(
            _onQuestStatusChange,
            _onDeathCounterChange
        );
    }

    private static QuestLevel GetLevelByStars(int stars)
    {
        return stars switch
        {
            >= 1 and < 6 => QuestLevel.LowRank,
            >= 5 and < 10 => QuestLevel.HighRank,
            >= 10 => QuestLevel.MasterRank,
            _ => throw new ArgumentOutOfRangeException(nameof(stars), stars, null)
        };
    }
}