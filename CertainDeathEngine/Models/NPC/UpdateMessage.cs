using log4net;
using Newtonsoft.Json;
using System.Text;
using CertainDeathEngine.Models.NPC.Buildings;

namespace CertainDeathEngine.Models.NPC
{
    [JsonObject]
    public abstract class UpdateMessage
    {
        public int ObjectId { get; set; }
        public string UType { get; set; }
    }

    public class BuildingStateChangeUpdateMessage : UpdateMessage
    {
        public string State { get; set; }
        public BuildingStateChangeUpdateMessage()
        {
            UType = "BuildingState";
        }
    }

    public class AddResourceToPlayerUpdateMessage : UpdateMessage
    {
        public string ResourceType { get; set; }
        public int Amount { get; set; }
        public AddResourceToPlayerUpdateMessage()
        {
            UType = "AddResourceToPlayer";
        }
    }

    public class GameOverUpdateMessage : UpdateMessage
    {
        public GameOverUpdateMessage()
        {
            UType = "GameOver";
        }
    }

    [JsonObject]
    public class HealthUpdateMessage : UpdateMessage
    {
        public float HealthPoints { get; set; }

        public HealthUpdateMessage()
        {
            UType = "Health";
        }
    }

    public class MonsterStateChangeUpdateMessage : UpdateMessage
    {
        public string State { get; set; }
        public MonsterStateChangeUpdateMessage()
        {
            UType = "MonsterState";
        }
    }

    [JsonObject]
    public class MoveUpdateMessage : UpdateMessage
    {
        public double MoveX { get; set; }
        public double MoveY { get; set; }

        public MoveUpdateMessage()
        {
            UType = "Move";
        }
    }

    [JsonObject]
    public class PlaceBuildingUpdateMessage : UpdateMessage
    {
        public double PosX { get; set; }
        public double PosY { get; set; }
        public string Type { get; set; }

        public PlaceBuildingUpdateMessage()
        {
            UType = "PlaceBuilding";
        }
    }

    [JsonObject]
    public class PlaceMonsterUpdateMessage : UpdateMessage
    {
        public double PosX { get; set; }
        public double PosY { get; set; }
        public string Type { get; set; }

        public PlaceMonsterUpdateMessage()
        {
            UType = "PlaceMonster";
        }
    }

    public class RemoveResourceFromSquareUpdateMessage : UpdateMessage
    {
        public string Square { get; set; }
        public int Amount { get; set; }
        public RemoveResourceFromSquareUpdateMessage()
        {
            UType = "RemoveResourceFromSquare";
        }
    }

    [JsonObject]
    public class RemoveUpdateMessage : UpdateMessage
    {
        public RemoveUpdateMessage()
        {
            UType = "Remove";
        }
    }

    public class UpdateBuildingCostUpdateMessage : UpdateMessage
    {
        public Cost NewCost { get; set; }
        public UpdateBuildingCostUpdateMessage()
        {
            UType = "UpdateCost";
        }
    }

    public class UpgradeBuildingUpdateMessage : UpdateMessage
    {
        public int NewLevel { get; set; }
        public UpgradeBuildingUpdateMessage()
        {
            UType = "Upgrade";
        }
    }

    public class WorldUpdateMessage : UpdateMessage
    {
        public WorldUpdateMessage()
        {
            UType = "World";
        }
    }

    
}
