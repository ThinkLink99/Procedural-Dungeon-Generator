/* DUNGEON GENERATION
 * Author: Trey Hall
 * 
 * This API can be used to procedurally generate a series of rooms to form a dungeon, 
 * similar to Binding of Isaac
 * 
 * TODO:
 * Add option to generate mazes inside of rooms
 * Add option to generate hallways between rooms
 */

using Newtonsoft.Json;

namespace DungeonGeneration
{
    /// <summary>
    /// Vector2 is a class containing an x and y value pair
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public struct Vector2
    {
        [JsonProperty]
        private int x;

        [JsonProperty]
        private int y;
        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2(float x, float y)
        {
            this.x = (int)x;
            this.y = (int)y;
        }

        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
    }
}
