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

		[JsonProperty]	
		public string StateName { get { return Enum.GetName(typeof(TurretState), State);} }

        // building's current state {WAITING, ATTAKING}
        private TurretState State { get; set; }

        // The monster the turret is attacking
        private Monster Attacking { get; set; }

        // Rotation of the turret
		[JsonProperty]
		public double Rotation { get; private set; }
        
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
                        State = TurretState.WAITING.ToString(),
                        Rotation = Rotation
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
				Monster monsterToAttack = FindClosestAttackableMonster();
                if (monsterToAttack != null)
				{
					Attacking = monsterToAttack;
                    State = TurretState.ATTACKING;
                    this.Tile.World.AddUpdateMessage(new BuildingStateChangeUpdateMessage(this.Id)
                    {
                        State = TurretState.ATTACKING.ToString(),
                        Rotation = this.Rotation
                    });
				}
			}
        }

        private Monster FindClosestAttackableMonster()
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
            if(monsterToReturn != null)
                Rotation = FindRotation(Position, monsterToReturn.Position);
            return monsterToReturn;
        }

        private double FindRotation(Point me, Point b)
        {
            var angle = Math.Atan2(b.Y - me.Y, b.X - me.X) % (2*Math.PI);
            angle = angle * (180 / Math.PI);
            return angle;
        }
        public long TimeSinceDamage { get; set; }
        private void Attack(long millis)
        {
            TimeSinceDamage += millis;
            if (TimeSinceDamage >= 1000)
            {
                long timeToDamage = (TimeSinceDamage / 1000);
                float damage = Damage * timeToDamage;
                Attacking.HealthPoints -= damage;
                TimeSinceDamage -= timeToDamage * 1000;
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
                if (Tile != null)
                {
                    this.Tile.World.AddUpdateMessage(new UpgradeBuildingUpdateMessage(this.Id)
                    {
                        NewLevel = Level
                    });
                }
            }
        }
    }

    public enum TurretState
    {
        WAITING, ATTACKING
    }
}
