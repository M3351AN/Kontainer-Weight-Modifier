// Copyright (c) 2025 渟雲. All rights reserved.
//
// Licensed under the Unlicense License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  https://unlicense.org
//
// -----------------------------------------------------------------------------
// File: CS2x64.h
// Author: 渟雲(quq[at]outlook.it)
// Date: 2025-09-29
//
// Description:
//   This file isSPT 4.0 MOD that modifies the weight
//   of all container items to -100
//
// -----------------------------------------------------------------------------
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace ContainerWeightModi;
public record ModMetadata : AbstractModMetadata
{
    public static ModMetadata Instance { get; private set; }

    public ModMetadata()
    {
        Instance = this;
    }
    public override string ModGuid { get; init; } = "icu.tkm.containerweightmodi";
    public override string Name { get; init; } = "Kontainer Weight Modifier";
    public override string Author { get; init; } = "Ukia";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "The Unlicense";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class ContainerWeightModifierHook(
    DatabaseServer databaseServer,
    ISptLogger<ContainerWeightModifierHook> logger) : IOnLoad
{
    public Task OnLoad()
    {
        var containersParentId = new MongoId("5448bf274bdc2dfc2f8b456a");

        int modifiedCount = 0;

        foreach (var item in databaseServer.GetTables().Templates.Items.Values)
        {
            if (item.Parent == containersParentId)
            {
                if (item.Properties == null)
                {
                    item.Properties = new SPTarkov.Server.Core.Models.Eft.Common.Tables.TemplateItemProperties();
                }

                var oldWeight = item.Properties.Weight;

                item.Properties.Weight = -100f;

                modifiedCount++;

                logger.LogWithColor(
                    $"[{ModMetadata.Instance?.Name}]Modified {item.Name} (ID: {item.Id}): Weight {oldWeight} -> -100",
                    LogTextColor.Blue);
            }
        }

        logger.LogWithColor(
            $"[{ModMetadata.Instance?.Name}]Weight modification completed! Modified {modifiedCount} items",
            LogTextColor.White,
            LogBackgroundColor.Blue);

        return Task.CompletedTask;
    }
}