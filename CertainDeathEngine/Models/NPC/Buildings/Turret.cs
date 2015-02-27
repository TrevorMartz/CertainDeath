﻿using CertainDeathEngine.Models.NPC.Buildings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Turret : Building
    {
        
        // The Radius of the turrets's attack circle
        public double Range { get; set; }

        // Turret's damage per second
        public float Damage { get; set; }

        // building's current state {WAITING, ATTAKING}
        private TurretState State { get; set; }

        // The monster the turret is attacking
        private Monster Attacking { get; set; }

        public float AttackSpeed { get; set; }
        
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
			if (State == TurretState.ATTACKING)
			{
				if (Attacking.HealthPoints <= 0)
				{
					// someone else killed it, do nothing this step
					// If we can sell buildings there could be a problem here
                    State = TurretState.WAITING;
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
				}
			}
        }

        private Monster FindClosestAttackableMonster(long millis)
        {
            Monster monsterToReturn = null;
            double distanceFromTurret = Range;
            lock (Tile.Monsters)
            {
                foreach (Monster m in Tile.Monsters)
                {
                    double d = GetFastDistance(m);
                    if (d < distanceFromTurret)
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
            if (Attacking.HealthPoints <= 0)
            {
                Tile.RemoveObject(Attacking);
                State = TurretState.WAITING;
                Attacking = null;
            }
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
                MaxHealthPoints = Level * 3;
                HealthPoints = MaxHealthPoints;
                Range = 3 * Level;
                AttackSpeed = Level * .03f;//idk, just pikced a number.
            }
        }
    }

    public enum TurretState
    {
        WAITING, ATTACKING
    }
}