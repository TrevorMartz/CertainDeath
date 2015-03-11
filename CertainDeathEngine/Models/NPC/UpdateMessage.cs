using log4net;
using Newtonsoft.Json;
using System.Text;
using CertainDeathEngine.Models.NPC.Buildings;

namespace CertainDeathEngine.Models.NPC
{
    [JsonObject]
    public abstract class UpdateMessage
    {
        public int ObjectId { get; private set; }
        public string UType { get; set; }

        protected UpdateMessage(int id)
        {
            ObjectId = id;
        }
    }

    [JsonObject]
    public class BuildingStateChangeUpdateMessage : UpdateMessage
    {
        public string State { get; set; }
        public BuildingStateChangeUpdateMessage(int id) : base(id)
        {
            UType = "BuildingState";
        }
    }

    [JsonObject]
    public class AddResourceToPlayerUpdateMessage : UpdateMessage
    {
        public string ResourceType { get; set; }
        public int Amount { get; set; }
        public AddResourceToPlayerUpdateMessage(int id) : base(id)
        {
            UType = "AddResourceToPlayer";
        }
    }

    [JsonObject]
    public class GameOverUpdateMessage : UpdateMessage
    {
        public GameOverUpdateMessage(int id) : base(id)
        {
            UType = "GameOver";
        }
    }

    [JsonObject]
    public class HealthUpdateMessage : UpdateMessage
    {
        public float HealthPoints { get; set; }
        public float MaxHealthPoints { get; set; }

        public HealthUpdateMessage(int id) : base(id)
        {
            UType = "Health";
        }
    }

    [JsonObject]
    public class MonsterStateChangeUpdateMessage : UpdateMessage
    {
        public string State { get; set; }
        public MonsterStateChangeUpdateMessage(int id) : base(id)
        {
            UType = "MonsterState";
        }
    }

    [JsonObject]
    public class MoveUpdateMessage : UpdateMessage
    {
        public double MoveX { get; set; }
        public double MoveY { get; set; }

        public MoveUpdateMessage(int id) : base(id)
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

        public PlaceBuildingUpdateMessage(int id) : base(id)
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
		public MonsterDirection Direction { get; set; }
		public string State { get; set; }

        public PlaceMonsterUpdateMessage(int id) : base(id)
        {
            UType = "PlaceMonster";
        }
    }

    [JsonObject]
    public class RemoveResourceFromPlayerUpdateMessage : UpdateMessage
    {
        public string ResourceType { get; set; }
        public int Amount { get; set; }
        public RemoveResourceFromPlayerUpdateMessage(int id)
            : base(id)
        {
            UType = "RemoveResourceFromPlayer";
        }
    }

    [JsonObject]
    public class RemoveResourceFromSquareUpdateMessage : UpdateMessage
    {
        public string Square { get; set; }
        public int Amount { get; set; }
        public RemoveResourceFromSquareUpdateMessage(int id) : base(id)
        {
            UType = "RemoveResourceFromSquare";
        }
    }

    [JsonObject]
    public class RemoveUpdateMessage : UpdateMessage
    {
        public RemoveUpdateMessage(int id) : base(id)
        {
            UType = "Remove";
        }
    }

    [JsonObject]
    public class UpdateBuildingCostUpdateMessage : UpdateMessage
    {
        public Cost NewCost { get; set; }
        public UpdateBuildingCostUpdateMessage(int id) : base(id)
        {
            UType = "UpdateCost";
        }
    }

    [JsonObject]
    public class UpgradeBuildingUpdateMessage : UpdateMessage
    {
        public int NewLevel { get; set; }
        public UpgradeBuildingUpdateMessage(int id) : base(id)
        {
            UType = "Upgrade";
        }
    }

    [JsonObject]
    public class WorldUpdateMessage : UpdateMessage
    {
        public WorldUpdateMessage() : base(0)
        {
            UType = "World";
        }
    }

    
}
