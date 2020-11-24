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

using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGeneration
{
    public class DungeonGeneration<T>
    {
        List<IRoom> AvailableRooms;
        int maxRooms = 0;
        Random rand;

        // the int is the id of the tile, the string is the url of the sprite
        Dictionary<int, object> tileSprites;
        public Dictionary<int, object> TileSprites { get => tileSprites; }

        public List<IRoom> Dungeon;

        public DungeonGeneration(int seed = 0, params IRoom[] rooms)
        {
            rand = new Random(seed);
            tileSprites = new Dictionary<int, object>();

            AvailableRooms = new List<IRoom>(rooms);
        }
        public DungeonGeneration(IEnumerable<IRoom> rooms, int maxRooms = 0, int seed = 0)
        {
            rand = new Random(seed);

            this.maxRooms = maxRooms;
            tileSprites = new Dictionary<int, object>();

            AvailableRooms = new List<IRoom>(rooms);
        }
        public DungeonGeneration(IEnumerable<IRoom> rooms, IEnumerable<object> sprites, int maxRooms = 0, int seed = 0)
        {
            try
            {
                rand = new Random(seed);
                this.maxRooms = maxRooms;

                AvailableRooms = new List<IRoom>(rooms);
                tileSprites = new Dictionary<int, object>();

                Dungeon = new List<IRoom>();

                int i = 0;
                foreach (object value in sprites)
                {
                    tileSprites.Add(i, value);
                    i++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Message: {ex.Message}\nStack: {ex.StackTrace}");
            }
        }
        /// <summary>
        /// Build Dungeon Levels in Binding of Isaac Style
        /// </summary>
        public int GenerateFreeformLayout(int side = -1, int lastRoom = -1, List<IRoom> availableRooms = null)
        {
            if (availableRooms == null) { availableRooms = new List<IRoom>(AvailableRooms); }

            // base case, current room <= max room number
            // max number of rooms to place
            // current room number, set room id equal to this
            // pick a random room to place
            // set the id of the appropriate side to the id of the last room, lastID

            // OPTION 1
            // Pick next room based on number of doors in the room
            
            // OPTION 2
            // get count of the available sides
            // generate a number between 0 and available side count
            // if > 0, pick a random side as number to place on, call Generate(current room number)

            // set side chosen at random to the returned value of generate

            // max number of rooms to place
            // current room number, set room id equal to this
            int currentRoom = lastRoom + 1;

            // base case, current room <= max room number
            if ((currentRoom + 1) > maxRooms) { return -1; }

            // pick a random room to place
            IRoom room = availableRooms[rand.Next(0, availableRooms.Count)];
            availableRooms.Remove(room);

            room.NorthID = -1;
            room.EastID = -1;
            room.SouthID = -1;
            room.WestID = -1;

            // set the id of the north side to the id of the last room, lastID
            switch (side)
            {
                // north
                case 0:
                    room.SouthID = Dungeon[lastRoom].ID;
                    break;
                // east
                case 1:
                    room.WestID = Dungeon[lastRoom].ID;
                    break;
                // south
                case 2:
                    room.NorthID = Dungeon[lastRoom].ID;
                    break;
                // west
                case 3:
                    room.EastID = Dungeon[lastRoom].ID;
                    break;
            }

            // OPTION 1
            FindDoors(room);

            // OPTION 2
            FindRooms(currentRoom, room);

            Dungeon.Add(room);
           
            //for (int i = 0; i < Dungeon.Count; i++) { SetOrigins(Dungeon[i]); }
            for (int i = 0; i < Dungeon.Count; i++) { FindAllRoomAdjacencies(); }
            return room.ID;
        }
        private void FindDoors (IRoom room)
        {
            //if (room.Tiles.Any(d => d.ID == TILE_IDS.NORTH_DOOR || d.ID == TILE_IDS.SOUTH_DOOR || d.ID == TILE_IDS.EAST_DOOR || d.ID == TILE_IDS.WEST_DOOR))
            //{

            //}
        }
        private void FindRooms (int currentRoom, IRoom room)
        {
            List<int> availableSide = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    // north
                    case 0:
                        if (room.NorthID == -1)
                        {
                            availableSide.Add(i);
                        }
                        break;
                    // east
                    case 1:
                        if (room.EastID == -1)
                        {
                            availableSide.Add(i);
                        }
                        break;
                    // south
                    case 2:
                        if (room.SouthID == -1)
                        {
                            availableSide.Add(i);
                        }
                        break;
                    // west
                    case 3:
                        if (room.WestID == -1)
                        {
                            availableSide.Add(i);
                        }
                        break;
                }
            }
            int numRooms = rand.Next(0, 3);
            if (numRooms + (currentRoom + 1) > maxRooms)
            {
                numRooms = maxRooms - (currentRoom + 1);
            }
            while (numRooms > 0)
            {
                int randSide = rand.Next(0, availableSide.Count);
                int nextRoom = -1;
                if (currentRoom + 2 <= maxRooms)
                {
                    nextRoom = GenerateFreeformLayout(randSide, currentRoom);
                    numRooms -= 1;
                    availableSide.Remove(randSide);
                }

                // set south side = returned value of Generate
                if (nextRoom > -1)
                {
                    switch (randSide)
                    {
                        // north
                        case 0:
                            Dungeon[currentRoom].NorthID = nextRoom;
                            break;
                        // east
                        case 1:
                            Dungeon[currentRoom].EastID = nextRoom;
                            break;
                        // south
                        case 2:
                            Dungeon[currentRoom].SouthID = nextRoom;
                            break;
                        // west
                        case 3:
                            Dungeon[currentRoom].WestID = nextRoom;
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// Build Dungeon Levels in Hades style
        /// </summary>
        public int GenerateOnRailsLayout(ref List<IRoom> returnList, int lastRoom = -1)
        {
            // max number of rooms to place
            // current room number, set room id equal to this
            int currentRoom = lastRoom + 1;

            // base case, current room <= max room number
            if (currentRoom > maxRooms) { return -1; }

            // pick a random room to place
            IRoom room = AvailableRooms[rand.Next(0, AvailableRooms.Count - 1)];
            room.NorthID = -1;
            room.SouthID = -1;
            room.ID = currentRoom;

            // set the id of the north side to the id of the last room, lastID
            room.NorthID = lastRoom;

            // call Generate(current room number)
            int southRoom = -1;
            if (currentRoom + 1 <= maxRooms)
            {
                southRoom = GenerateOnRailsLayout(ref returnList, currentRoom);
            }

            // set south side = returned value of Generate
            if (southRoom > -1) { room.SouthID = southRoom; }

            return currentRoom;
        }

        private void FindAllRoomAdjacencies ()
        {
            // loop through list of rooms and determine if any are near eachother and havent been associated
        }

        private void SetOrigins (IRoom room)
        {
            // find rooms that are attached to this current room
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        if (room.NorthID != -1)
                        {
                            var temp = Dungeon.Where(d => d.ID == room.NorthID).FirstOrDefault();
                            room.OriginX = (temp.ID == -1 ? 0 : temp.OriginX);
                            room.OriginY = (temp.ID == -1 ? 0 : temp.OriginY - room.Height);
                        }
                        break;
                    case 1:
                        if (room.EastID != -1)
                        {
                            var temp = Dungeon.Where(d => d.ID == room.EastID).FirstOrDefault();
                            room.OriginX = (temp.ID == -1 ? 0 : temp.OriginX - room.Width);
                            room.OriginY = (temp.ID == -1 ? 0 : temp.OriginY);
                        }
                        break;
                    case 2:
                        if (room.SouthID != -1)
                        {
                            var temp = Dungeon.Where(d => d.ID == room.SouthID).FirstOrDefault();
                            room.OriginX = (temp.ID == -1 ? 0 : temp.OriginX);
                            room.OriginY = (temp.ID == -1 ? 0 : temp.OriginY + temp.Height);
                        }
                        break;
                    case 3:
                        if (room.WestID != -1)
                        {
                            var temp = Dungeon.Where(d => d.ID == room.WestID).FirstOrDefault();
                            room.OriginX = (temp.ID == -1 ? 0 : temp.OriginX + temp.Width);
                            room.OriginY = (temp.ID == -1 ? 0 : temp.OriginY);
                        }
                        break;
                }
            }
        }
    }
}
