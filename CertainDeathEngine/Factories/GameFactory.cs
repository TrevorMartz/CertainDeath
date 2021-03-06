﻿using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC.Buildings;
using CertainDeathEngine.Models.Resources;
using log4net;
using System;
using System.Windows;

namespace CertainDeathEngine.Factories
{
    public class GameFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int _nextObjectId = 1;
        private readonly object _idBaton = new object();

        public GameWorld World { get; protected set; }

        public GameFactory(GameWorld world)
        {
            Log.Debug("Constructing a GameFactory");
            World = world;
        }

        public Building BuildBuilding(BuildingType buildingType, Point position)
        {
            Log.Debug("Trying to build a building of type : " + buildingType + " at position: " + position);
            Building building = null;
            Cost cost = null;
            switch(buildingType)
            {
                case BuildingType.TURRET:
                    building = new Turret(World.CurrentTile, position,
                        GameWorld.BuildingCostsForTheWorld[BuildingType.TURRET]) { Id = GetNextId() };
                    break;
                case BuildingType.WALL:
                    building = new Wall(World.CurrentTile, position, 
                        GameWorld.BuildingCostsForTheWorld[BuildingType.WALL]) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_MINE:
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_MINE, World.Player, 
                        GameWorld.BuildingCostsForTheWorld[BuildingType.AUTO_HARVESTER_MINE]) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_QUARRY:
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_QUARRY, World.Player,
                        GameWorld.BuildingCostsForTheWorld[BuildingType.AUTO_HARVESTER_QUARRY]) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_FARM:
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_FARM, World.Player,
                        GameWorld.BuildingCostsForTheWorld[BuildingType.AUTO_HARVESTER_FARM]) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_LUMBER_MILL:
                    cost = new Cost();
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_LUMBER_MILL, World.Player,
                        GameWorld.BuildingCostsForTheWorld[BuildingType.AUTO_HARVESTER_LUMBER_MILL]) { Id = GetNextId() };
                    break;
                case BuildingType.FIRE_OF_LIFE: //fire of life does not have a case because it is only built once, in the constructor of the gameworld.
                    Log.Error("You can't build a second Fire of Life");
                    throw new Exception("You can't have another fire of life");
            }
            return building;
        }

		public int GetNextId()
		{
			int id;
			lock (_idBaton)
			{
				id = _nextObjectId++;
			}
			return id;
		}
    }
}
