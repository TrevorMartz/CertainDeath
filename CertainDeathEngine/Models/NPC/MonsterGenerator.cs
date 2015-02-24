using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC
{
	class MonsterGenerator
	{
		// Milliseconds between each montster generated
		public int Frequency { get; set; }

		// Millisecond delay before first monster is generated
		public int Delay { get; set; }

		// Number of Monsters to generate
		public int NumMonsters { get; set; }
	}
}
