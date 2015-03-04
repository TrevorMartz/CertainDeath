using log4net;
using Newtonsoft.Json;
using System.Text;

namespace CertainDeathEngine.Models.NPC
{
    [JsonObject]
    public abstract class UpdateMessage
    {
        public int ObjectId { get; set; }
        public string UType { get; set; }
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
    public class HealthUpdateMessage : UpdateMessage
    {
        public float HealthPoints { get; set; }

        public HealthUpdateMessage()
        {
            UType = "Health";
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

    public class WorldUpdateMessage : UpdateMessage
    {
        public WorldUpdateMessage()
        {
            UType = "World";
        }
    }

    public class BuildingStateChangeUpdateMessage : UpdateMessage
    {
        public string State { get; set; }
        public BuildingStateChangeUpdateMessage()
        {
            UType = "BuildingState";
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

    
}
