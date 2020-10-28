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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace DungeonGeneration
{
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Room<T>
    {
        public T Root { get; set; }

        [JsonProperty]
        byte roomCount = 0;
        [JsonProperty]
        int id = 0;

        int north_id = -1;
        int south_id = -1;
        int east_id = -1;
        int west_id = -1;

        [JsonProperty]
        public int NorthID
        {
            get
            {
                return north_id;
            }
            set
            {
                north_id = value;
            }
        }
        [JsonProperty]
        public int SouthID
        {
            get
            {
                return south_id;
            }
            set
            {
                south_id = value;
            }
        }
        [JsonProperty]
        public int EastID
        {
            get
            {
                return east_id;
            }
            set
            {
                east_id = value;
            }
        }
        [JsonProperty]
        public int WestID
        {
            get
            {
                return west_id;
            }
            set
            {
                west_id = value;
            }
        }

        [JsonProperty]
        private Vector2 origin;
        [JsonProperty]
        private int width;
        [JsonProperty]
        private int height;
        [JsonProperty]
        private List<Tile<T>> tiles = new List<Tile<T>>();
        [JsonProperty]
        private List<Tile<T>> worldObjects = new List<Tile<T>>();

        public int ID { get { return id; } set { id = value; } }
        public List<Tile<T>> Tiles { get => tiles; set => tiles = value; }
        public List<Tile<T>> WorldObjects { get => worldObjects; set => worldObjects = value; }
        public byte AttachedRoomCount
        {
            get
            {
                byte count = 0;
                if (NorthID != -1) { count++; }
                if (SouthID != -1) { count++; }
                if (EastID != -1) { count++; }
                if (WestID != -1) { count++; }

                return count;
            }
        }

        public Vector2 Origin { get => origin; set => origin = value; }

        public int Width { get => width; }
        public int Height { get => height; }

        public Room(Vector2 origin, int width = 0, int height = 0)
        {
            this.width = width;
            this.height = height;
            this.origin = origin;
        }
    }
}
