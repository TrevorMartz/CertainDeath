using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
    public class Building : Killable, Temporal
    {
        public float HarvestRate { get; set; }

		public void Update(long millis)
		{
			throw new NotImplementedException();
		}
	}
}
