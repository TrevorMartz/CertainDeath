using CertainDeathEngine.Models.Resources;
using log4net;
using Newtonsoft.Json;
using System;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Turret : Building
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // The Radius of the turrets's attack circle
        public double Range { get; set; }

        // Turret's damage per second
        public float Damage { get; set; }

        // building's current state {WAITING, ATTAKING}
        private TurretState State { get; set; }

        // The monster the turret is attacking
        private Monster Attacking { get; set; }
        
		public Turret(Tile tile, Point pos) : base(tile, pos)
		{
            Type = BuildingType.TURRET;
            State = TurretState.WAITING;
            MaxLevel = 5;
            Level = 0;
            Upgrade();   
		}

		public override void Update(long millis)
		{
            if (HealthPoints <= 0)
            {
                Remove();
            }
			if (State == TurretState.ATTACKING)
			{
				if (Attacking.HealthPoints <= 0)
				{
					// someone else killed it, do nothing this step
					// If we can sell buildings there could be a problem here
                    State = TurretState.WAITING;
                    this.Tile.World.AddUpdateMessage(new BuildingStateChangeUpdateMessage(this.Id)
                    {
                        State = TurretState.WAITING.ToString()
                    });
					Attacking = null;
				}
				else
				{
					Attack(millis);
				}
			}
			else if (State == TurretState.WAITING)
			{
				Monster monsterToAttack = FindClosestAttackableMonster(millis);
                if (monsterToAttack != null)
				{
					Attacking = monsterToAttack;
                    State = TurretState.ATTACKING;
                    this.Tile.World.AddUpdateMessage(new BuildingStateChangeUpdateMessage(this.Id)
                    {
                        State = TurretState.ATTACKING.ToString()
                    });
				}
			}
        }

        private Monster FindClosestAttackableMonster(long millis)
        {
            Monster monsterToReturn = null;
            lock (Tile.World)
            {
                foreach (Monster m in Tile.Monsters)
                {
                    double d = GetFastDistance(m);
                    if (d < Range)
                    {
                        monsterToReturn = m;
                    }
                }
            }
            return monsterToReturn;
        }

        private void Attack(long millis)
        {
            float damage = Damage * (millis / 1000.0f);
            Attacking.HealthPoints -= damage;
            this.Tile.World.AddUpdateMessage(new HealthUpdateMessage(Attacking.Id)
            {
                HealthPoints = Attacking.HealthPoints,
                MaxHealthPoints = Attacking.MaxHealthPoints
            });

            if (Attacking.HealthPoints <= 0)
            {
                Attacking.Remove();
                State = TurretState.WAITING;
                Attacking = null;
            }
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                //TODO: Adjust Health, Damage Levels and Cost
                Level++;
                MaxHealthPoints = Level * 300;
                HealthPoints = MaxHealthPoints;
                Range = Square.PIXEL_SIZE * 1 + Level;
				Damage = Level * 10;
                UpdateCost();
                if (Tile != null)
                {
                    this.Tile.World.AddUpdateMessage(new UpgradeBuildingUpdateMessage(this.Id)
                    {
                        NewLevel = Level
                    });
                }
            }
        }

        public override void UpdateCost()
        {
            Cost = new Cost();
            Cost.SetCost(ResourceType.COAL, 10 * Level);
            Cost.SetCost(ResourceType.CORN, 10 * Level);
            Cost.SetCost(ResourceType.IRON, 10 * Level);
            Cost.SetCost(ResourceType.STONE, 10 * Level);
            Cost.SetCost(ResourceType.WOOD, 10 * Level);
            if (Tile != null)
            {
                this.Tile.World.AddUpdateMessage(new UpdateBuildingCostUpdateMessage(this.Id)
                {
                    NewCost = Cost
                });
            }
        }
    }

    public enum TurretState
    {
        WAITING, ATTACKING
    }
}
