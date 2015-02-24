using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using CertainDeathEngine.Models.NPC;

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
		string SquareClicked(float x, float y);

		/* Notify the game engine that a user has clicked on a monster. The monster wil take damage 
		 * equal to the user's attack damage.
		 * 
		 * Returns a JSON string representing changes. 
		 * JSON described in IncrementTimeAndReturnDelta method
		 */
		string MonsterClicked(int monsterid);

		/* Instructs the game engine to move time forward millis number of milliseconds.
		 * 
		 * Parameter: milliseconds to increment time
		 * 
		 * Return: JSON string representing the changes that have occured over the elapsed time.
		 * 
		 * JSON: This has not been created yet and will not be needed until there is more of a game
		 *		Such as movement, damage, picking up resources, etc.
		 */
		string IncrementTimeAndReturnDelta(int millis);

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

        IEnumerable<string> GetBuildableBuildingsList();

        Building BuildBuildingAtSquare(int row, int column, string buildingType);
	}
}
