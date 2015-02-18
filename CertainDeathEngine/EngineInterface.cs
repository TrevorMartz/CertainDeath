using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;

namespace CertainDeathEngine
{
	interface EngineInterface
	{
		
		/* Changed the current tile to be the tile directly above the current current tile.
		 * If there is no above tile, does nothing.
		 */
		void MoveUp();

		/* Changed the current tile to be the tile directly below the current current tile.
		 * If there is no below tile, does nothing.
		 */
		void MoveDown();

		/* Changed the current tile to be the tile directly to the left of the current current tile.
		 * If there is no left tile, does nothing.
		 */
		void MoveLeft();

		/* Changed the current tile to be the tile directly to the right of the current current tile.
		 * If there is no right tile, does nothing.
		 */
		void MoveRight();

		/* Notify the game engine that a user has clicked on a sqaure. If there is a resource on that 
		 * square the resource will be added to the user's inventory. Otherwise nothing will happen.
		 * 
		 * Returns a JSON string representing changes. 
		 * JSON described in IncrementTimeAndReturnDelta method
		 */
		string SquareClicked(int x, int y);

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
		string IncrementTimeAndReturnDeta(int millis);

		/* returns a JSON string representing the game state including: Squares, monsters, resources, and buildings.
		 * 
		 * Example:
		 *			{
			 *			squares:[
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url],
			 *				[url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url, url]
			 *			], 
			 *			directions: [{canMoveUp : true}, {canMoveDown : true}, {canMoveLeft : false}, {canMoveUp : true}],
			 *			resources: [{type: corn, square_x: 5, square_y: 10, quantity: 25}, {type: wood, square_x: 12, square_y: 19, quantity: 6}],
			 *			monsters: [{id: 0, health: 100, damage: 15, top_left_pixel_x: 115, top_left_pixel_y: 200}],
			 *			buildings: [{type: turret_1, damage: 500, fire_rate: .5, size: 2, top_left_x: 7 top_left_y: 3}]
		 *			}
		 */
		string ToJSON();
	}
}
