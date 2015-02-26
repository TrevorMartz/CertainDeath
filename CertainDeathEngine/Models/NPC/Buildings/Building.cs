﻿using CertainDeathEngine.Models.NPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class Building : Killable, Temporal
    {
		[JsonProperty]
		public string Typename { get { return Type.ToString(); } }

        public BuildingType Type { get; protected set; }

        public Tile Tile { get; protected set; }

        public abstract void Update(long millis);

        public void RemoveBuilding()
        {
            Tile.RemoveObject(this);
        }
	}
}
