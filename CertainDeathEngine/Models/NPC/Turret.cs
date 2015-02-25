using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Turret : Building
    {

        // The Radius of the turrets's attack circle
        public int Radius { get; set; }

        // Turret's damage per second
        public float Damage { get; set; }

        public override Point Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
            }
        }
        public override int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
            }
        }
        public override int Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
            }
        }

        // building's current state {WAITING, ATTAKING}
        private TurretState State { get; set; }

        // The monster the building is attacking
        private Monster Attacking { get; set; }
        public Tile Tile { get; private set; }

        // 
        public float AttackSpeed { get; set; }
        

		public Turret(Tile t, Point pos, int attackSpeed)
		{
			Tile = t;
			Position = pos;
            AttackSpeed = attackSpeed;
            State = TurretState.WAITING;
			_Height = Square.PIXEL_SIZE;
			_Width = Square.PIXEL_SIZE;
		}
		public void Update(long millis)
		{
            //Trace.WriteLine("Turret " + Id + " is updating");
			if (State == TurretState.ATTACKING)
			{
				if (Attacking.HealthPoints <= 0)
				{
					// someone else killed it, do nothing this step, then start walking
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
				Monster monsterToAttack = FindTheFirstMonsterThatIsCloseEnoughToAttackAndReturnThatMonsterSoWeCanStoreTheThingWeWantToShootAt(millis);
                if (monsterToAttack != null)
				{
					Attacking = monsterToAttack;
					State = TurretState.ATTACKING;
				}
			}
        }

        private Monster FindTheFirstMonsterThatIsCloseEnoughToAttackAndReturnThatMonsterSoWeCanStoreTheThingWeWantToShootAt(long millis)
        {
            Monster monsterToReturn = null;
            double distanceFromTurret = double.MaxValue;
            foreach (Monster m in Tile.Monsters)
            {
                double d = GetFastDistance(m);
                if (d < distanceFromTurret && d > Radius)
                {
                    monsterToReturn = m;
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
            }
        }
    }
}
