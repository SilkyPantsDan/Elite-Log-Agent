﻿using DW.ELA.Interfaces;
using DW.ELA.LogModel;
using DW.ELA.Interfaces.Events;
using DW.ELA.Plugin.Inara.Model;
using DW.ELA.Utility.Json;
using Interfaces;
using MoreLinq;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DW.ELA.Plugin.Inara
{
    public class InaraEventConverter : IEventConverter<ApiEvent>
    {
        private readonly IPlayerStateHistoryRecorder playerStateRecorder;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public InaraEventConverter(IPlayerStateHistoryRecorder playerStateRecorder)
        {
            this.playerStateRecorder = playerStateRecorder ?? throw new ArgumentNullException(nameof(playerStateRecorder));
        }

        public IEnumerable<ApiEvent> Convert(LogEvent @event)
        {
            try
            {
                var eventName = @event.Event;
                switch (@event)
                {
                    // Generic
                    case LoadGame e: return ConvertEvent(e);
                    case Statistics e: return ConvertEvent(e);

                    // Inventory
                    case Materials e: return ConvertEvent(e);
                    case MaterialCollected e: return ConvertEvent(e);
                    case StoredModules e: return ConvertEvent(e);

                    // Travel
                    case FsdJump e: return ConvertEvent(e);
                    case Docked e: return ConvertEvent(e);

                    // Engineers
                    case EngineerProgress e: return ConvertEvent(e);

                    // Combat
                    case Interdicted e: return ConvertEvent(e);
                    case Interdiction e: return ConvertEvent(e);
                    case EscapeInterdiction e: return ConvertEvent(e);
                    case PvpKill e: return ConvertEvent(e);

                    // Loadout
                    case Loadout e: return ConvertEvent(e);

                    // Missions
                    case MissionAccepted e: return ConvertEvent(e);
                    case MissionCompleted e: return ConvertEvent(e);
                    case MissionAbandoned e: return ConvertEvent(e);
                    case MissionFailed e: return ConvertEvent(e);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error in OnNext");
            }
            return Enumerable.Empty<ApiEvent>();
        }

        private IEnumerable<ApiEvent> ConvertEvent(StoredModules e)
        {
            var @event = new ApiEvent("setCommanderStorageModules")
            {
                Timestamp = e.Timestamp,
                EventData = e.Items.Select(ConvertStoredItem).ToArray()
            };
            yield return @event;
        }

        private Dictionary<string,object> ConvertStoredItem(Item item)
        {
            var result = new Dictionary<string, object>()
            {
                { "itemName", item.Name},
                { "itemValue", item.BuyPrice},
                { "isHot", item.Hot},
                { "starsystemName", item.StarSystem},
                { "marketID", item.MarketId},
            };
            if (!string.IsNullOrEmpty(item.EngineerModifications))
                result.Add("engineering", new Dictionary<string, object>()
                {
                    {"blueprintName", item.EngineerModifications },
                    {"blueprintLevel", item.Level },
                    {"blueprintQuality", item.Quality },

                });
            return result;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MissionFailed e)
        {
            var @event = new ApiEvent("setCommanderMissionFailed")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    {"missionGameID", e.MissionId },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MissionAbandoned e)
        {
            var @event = new ApiEvent("setCommanderMissionAbandoned")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    {"missionGameID", e.MissionId },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MissionCompleted e)
        {
            var @event = new ApiEvent("setCommanderMissionCompleted")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    {"missionGameID", e.MissionId },
                    {"missionName", e.Name },
                    {"donationCredits", e.Donation },
                    {"rewardCredits", e.Reward },
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MissionAccepted e)
        {
            var @event = new ApiEvent("addCommanderMission")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    {"missionName", e.Name },
                    {"missionGameID", e.MissionId },
                    {"starsystemNameOrigin", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    {"minorfactionNameOrigin", e.Faction },
                    {"minorfactionNameTarget", e.TargetFaction },
                    {"influenceGain", e.Influence },
                    {"reputationGain", e.Reputation },
                    {"starsystemNameTarget", e.DestinationSystem },
                    {"stationNameTarget", e.DestinationStation },
                    {"missionExpiry", e.Expiry }
                }
            };
            yield return @event;
        }

        private IEnumerable<ApiEvent> ConvertEvent(MaterialCollected e)
        {
            var @event = new ApiEvent("addCommanderInventoryMaterialsItem")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    {"itemName", e.Name},
                    {"itemCount", e.Count }
                }
            };
            yield return @event;
        }


        private IEnumerable<ApiEvent> ConvertEvent(Loadout e)
        {
            var shipEvent = new ApiEvent("setCommanderShip")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>()
                {
                    { "shipType" , e.Ship},
                    { "shipGameID" , e.ShipId},
                    { "shipName" , e.ShipName},
                    { "shipIdent" , e.ShipIdent},
                    { "isCurrentShip" , true},
                    { "isMainShip" , false},
                    { "isHot" , false}, // TODO
                    { "shipHullValue" , e.HullValue},
                    { "shipModulesValue" , e.ModulesValue},
                    { "shipRebuyCost" , e.Rebuy},
                    { "starsystemName" , playerStateRecorder.GetPlayerSystem(e.Timestamp)}
                }
            };

            yield return shipEvent;

            var loadoutEvent = new ApiEvent("setCommanderShipLoadout")
            {
                Timestamp = e.Timestamp,
                EventData = new Dictionary<string, object>
                {
                    { "shipType", e.Ship },
                    { "shipGameID", e.ShipId },
                    { "shipLoadout", e.Modules.Select(ConvertModule).ToArray() }
                }
            };

            yield return loadoutEvent;
        }

        JObject ConvertModule(Module module)
        {
            var item = new JObject
            {
                ["slotName"] = module.Slot,
                ["itemName"] = module.Item,
                ["itemHealth"] = module.Health,
                ["isOn"] = module.On,
                ["isHot"] = false, // TODO!
                ["itemPriority"] = module.Priority,

            };
            item.AddIfNotNull("itemAmmoClip", module.AmmoInClip);
            item.AddIfNotNull("itemAmmoHopper", module.AmmoInHopper);
            item.AddIfNotNull("itemValue", module.Value);
            if (module.Engineering != null)
                item["engineering"] = ConvertEngineering(module.Engineering);
            return item;
        }

        private JObject ConvertEngineering(Engineering eng)
        {
            var item = new JObject
            {
                ["blueprintName"] = eng.BlueprintName,
                ["blueprintLevel"] = eng.Level,
                ["blueprintQuality"] = eng.Quality,
                ["modifiers"] = new JArray(eng.Modifiers.Select(ConvertModifier).ToArray())
            };
            item.AddIfNotNull("experimentalEffect", eng.ExperimentalEffect);
            return item;
        }

        private JObject ConvertModifier(Modifier mod)
        {
            var item = new JObject()
            {
                ["name"] = mod.Label,
                ["value"] = mod.Value,
                ["originalValue"] = mod.OriginalValue,
                ["lessIsGood"] = mod.LessIsGood > 0
            };
            return item;
        }

        private IEnumerable<ApiEvent> ConvertEvent(EscapeInterdiction e)
        {
            yield return new ApiEvent("addCommanderCombatInterdictionEscape")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdictor },
                    { "isPlayer", e.IsPlayer }
                },
                Timestamp = e.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(Interdiction e)
        {
            yield return new ApiEvent("addCommanderCombatInterdiction")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdicted },
                    { "isPlayer", e.IsPlayer }
                },
                Timestamp = e.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(Interdicted e)
        {
            yield return new ApiEvent("addCommanderCombatInterdicted")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(e.Timestamp) },
                    { "opponentName", e.Interdictor },
                    { "isPlayer", e.IsPlayer },
                    { "isSubmit", e.Submitted }
                },
                Timestamp = e.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(PvpKill @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiEvent("addCommanderCombatKill")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", playerStateRecorder.GetPlayerSystem(timestamp) },
                    { "opponentName", @event.Victim },
                },
                Timestamp = timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(Statistics @event)
        {
            yield return new ApiEvent("setCommanderGameStatistics")
            {
                EventData = @event.Raw,
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(Docked @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiEvent("addCommanderTravelDock")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event.StarSystem },
                    { "stationName", @event.StationName},
                    { "marketID", @event.MarketId },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(FsdJump @event)
        {
            var timestamp = @event.Timestamp;
            yield return new ApiEvent("addCommanderTravelFSDJump")
            {
                EventData = new Dictionary<string, object> {
                    { "starsystemName", @event.StarSystem },
                    { "jumpDistance", @event.JumpDist },
                    { "shipGameID", playerStateRecorder.GetPlayerShipId(timestamp) },
                    { "shipType", playerStateRecorder.GetPlayerShipType(timestamp) }
                },
                Timestamp = timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(Materials @event)
        {
            var materialCounts = @event.RawMats.Concat(@event.Manufactured).Concat(@event.Encoded)
                .ToDictionary(mat => mat.Name, mat => mat.Count);

            yield return new ApiEvent("setCommanderInventoryMaterials")
            {
                EventData = materialCounts
                    .Select(kvp => new { itemName = kvp.Key, itemCount = kvp.Value })
                    .OrderBy(x => x.itemName)
                    .ToArray(),
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(LoadGame @event)
        {
            yield return new ApiEvent("setCommanderCredits")
            {
                EventData = new Dictionary<string, object> {
                    { "commanderCredits", @event.Credits },
                    { "commanderLoan", @event.Loan }
                },
                Timestamp = @event.Timestamp
            };
        }

        private IEnumerable<ApiEvent> ConvertEvent(EngineerProgress @event)
        {
            yield return new ApiEvent("setCommanderRankEngineer")
            {
                EventData = new Dictionary<string, object> {
                    { "engineerName", @event.Engineer },
                    { "rankStage", @event.Progress },
                    { "rankValue", @event.Rank }
                },
                Timestamp = @event.Timestamp
            };
        }
    }
}