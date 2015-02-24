using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC
{
	interface Temporal
	{
		// Represents the passing of time
		void Update(long millis);
	}
}
