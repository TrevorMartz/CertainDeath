using System.Collections.Generic;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC.Buildings;

namespace CertainDeathEngine
{
	public interface EngineInterface
	{
		
		/* Changed the current tile to be the tile directly above the current current tile.
		 * If there is no above tile, does nothing.
		 * 
		 * Returns ToJSON
		 */
		string MoveUp();

		/* Changed the current tile to be the tile directly below the current current tile.
		 * If there is no below tile, does nothing.
		 * Returns ToJSON
		 */
		string MoveDown();

		/* Changed the current tile to be the tile directly to the left of the current current tile.
		 * If there is no left tile, does nothing.
		 * Returns ToJSON
		 */
		string MoveLeft();

		/* Changed the current tile to be the tile directly to the right of the current current tile.
		 * If there is no right tile, does nothing.
		 * Returns ToJSON
		 */
		string MoveRight();

		/* Notify the game engine that a user has clicked on a sqaure. If there is a resource on that 
		 * square the resource will be added to the user's inventory. Otherwise nothing will happen.
		 * 
		 * Returns a JSON string representing changes. 
		 * JSON described in IncrementTimeAndReturnDelta method
		 */
        void SquareClicked(RowColumnPair click);

		/* returns a JSON string representing the game state including: Squares, monsters, resources, and buildings.
		 * 
		 * Example:
		 * 
		 * { "HasAbove" : false,
			  "HasBelow" : false,
			  "HasLeft" : false,
			  "HasRight" : false,
			  "Objects" : [ (I don't know how this will look yet) ],
			  "Squares" : [ [ 
		            { "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },
					{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },
					{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },
					{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },
					{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" }
				  ],
				  [ { "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },
					{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },
					{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },{ "TypeName" : "GRASS" },
					...
				  ]
		          ...
			   ]
			}
		 */
		string ToJSON();

        IEnumerable<BuildingType> GetBuildableBuildingsList();

        Building BuildBuildingAtSquare(int row, int column, BuildingType buildingType);

        void SaveWorld();

        void GameOver();

        void SaveScore();
	}
}
