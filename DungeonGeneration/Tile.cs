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
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Tile<T>
    {
        private T _root;

        [JsonProperty]
        private int _id;
        [JsonProperty]
        private Vector2 _worldPoint;

        public int Id { get => _id; set => _id = value; }
        public Vector2 WorldPoint { get => _worldPoint; set => _worldPoint = value; }
        public T Root { get => _root; set => _root = value; }
    }
}
