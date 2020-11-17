﻿/* DUNGEON GENERATION
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
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace DungeonGeneration
{
    public class DungeonGeneration<T>
    {
        #region MODEL_CONSTANTS
        private T[] tiles_available = new T[14];
        #region TILES
        private T NORTH_WALL { get { return tiles_available[(int)TILE_IDS.NORTH_WALL]; } set { tiles_available[(int)TILE_IDS.NORTH_WALL] = value; } }
        private T SOUTH_WALL { get { return tiles_available[(int)TILE_IDS.SOUTH_WALL]; } set { tiles_available[(int)TILE_IDS.SOUTH_WALL] = value; } }
        private T EAST_WALL { get { return tiles_available[(int)TILE_IDS.EAST_WALL]; } set { tiles_available[(int)TILE_IDS.EAST_WALL] = value; } }
        private T WEST_WALL { get { return tiles_available[(int)TILE_IDS.WEST_WALL]; } set { tiles_available[(int)TILE_IDS.WEST_WALL] = value; } }

        private T NORTHWEST_CORNER { get { return tiles_available[(int)TILE_IDS.NORTHWEST_CORNER]; } set { tiles_available[(int)TILE_IDS.NORTHWEST_CORNER] = value; } }
        private T NORTHEAST_CORNER { get { return tiles_available[(int)TILE_IDS.NORTHEAST_CORNER]; } set { tiles_available[(int)TILE_IDS.NORTHEAST_CORNER] = value; } }
        private T SOUTHWEST_CORNER { get { return tiles_available[(int)TILE_IDS.SOUTHWEST_CORNER]; } set { tiles_available[(int)TILE_IDS.SOUTHWEST_CORNER] = value; } }
        private T SOUTHEAST_CORNER { get { return tiles_available[(int)TILE_IDS.SOUTHEAST_CORNER]; } set { tiles_available[(int)TILE_IDS.SOUTHEAST_CORNER] = value; } }

        private T FLOOR { get { return tiles_available[(int)TILE_IDS.FLOOR]; } set { tiles_available[(int)TILE_IDS.FLOOR] = value; } }
        #endregion
        #region WORLD_OBJECTS
        private T NORTH_DOOR { get { return tiles_available[(int)TILE_IDS.NORTH_DOOR]; } set { tiles_available[(int)TILE_IDS.NORTH_DOOR] = value; } }
        private T SOUTH_DOOR { get { return tiles_available[(int)TILE_IDS.SOUTH_DOOR]; } set { tiles_available[(int)TILE_IDS.SOUTH_DOOR] = value; } }
        private T EAST_DOOR { get { return tiles_available[(int)TILE_IDS.EAST_DOOR]; } set { tiles_available[(int)TILE_IDS.EAST_DOOR] = value; } }
        private T WEST_DOOR { get { return tiles_available[(int)TILE_IDS.WEST_DOOR]; } set { tiles_available[(int)TILE_IDS.WEST_DOOR] = value; } }

        private T SMALL_CHEST { get { return tiles_available[(int)TILE_IDS.SMALL_CHEST]; } set { tiles_available[(int)TILE_IDS.SMALL_CHEST] = value; } }
        #endregion
        #endregion

        List<IRoom> AvailableRooms;
        int maxRooms = 0;
        Random rand;

        // the int is the id of the tile, the string is the url of the sprite
        Dictionary<int, object> tileSprites;
        public Dictionary<int, object> TileSprites { get => tileSprites; }
        //int chestMin = 0, chestMax = 0;

        public List<IRoom> Dungeon;

        /// <summary>
        /// Initializes all model constant values for Walls/Corners/Floors/Etc.
        /// </summary>
        /// <param name="northWall"></param>
        /// <param name="southWall"></param>
        /// <param name="eastWall"></param>
        /// <param name="westWall"></param>
        /// <param name="northwestCorner"></param>
        /// <param name="northeastCorner"></param>
        /// <param name="southwestCorner"></param>
        /// <param name="southeastCorner"></param>
        /// <param name="floor"></param>
        /// <param name="smallChest"></param>
        /// <param name="seed"></param>
        public DungeonGeneration(T northWall, T southWall, T eastWall, T westWall,
                                  T northwestCorner, T northeastCorner, T southwestCorner, T southeastCorner,
                                  T northDoor, T southDoor, T eastDoor, T westDoor,
                                  T floor, T smallChest, int seed = 0)
        {
            NORTH_WALL = northWall;
            SOUTH_WALL = southWall;
            EAST_WALL = eastWall;
            WEST_WALL = westWall;

            NORTHWEST_CORNER = northwestCorner;
            NORTHEAST_CORNER = northeastCorner;
            SOUTHWEST_CORNER = southwestCorner;
            SOUTHEAST_CORNER = southeastCorner;

            FLOOR = floor;

            NORTH_DOOR = northDoor;
            SOUTH_DOOR = southDoor;
            EAST_DOOR = eastDoor;
            WEST_DOOR = westDoor;

            SMALL_CHEST = smallChest;

            tileSprites = new Dictionary<int, object>();

            rand = new Random(seed);
        }        
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
        public int GenerateFreeformLayout (int side = -1, int lastRoom = -1)
        {
            // base case, current room <= max room number
            // max number of rooms to place
            // current room number, set room id equal to this
            // pick a random room to place
            // set the id of the appropriate side to the id of the last room, lastID

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
            IRoom room = AvailableRooms[rand.Next(0, AvailableRooms.Count)];
            room.NorthID = -1;
            room.EastID = -1;
            room.SouthID = -1;
            room.WestID = -1;

            //room.ID = currentRoom;

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

            Dungeon.Add(room);

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
            
            return room.ID;
        }

        /// <summary>
        /// Build Dungeon Levels in Hades style
        /// </summary>
        public int GenerateOnRailsLayout (ref List<IRoom> returnList, int lastRoom = -1)
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

        private void AssignSpriteToTile (IRoom room)
        {
            //for (int i in room.tiles)
        }

        //public List<Room<T>> BuildDungeon(Vector2 origin, Vector2 width, Vector2 height, Vector2 rooms, Vector2 chests)
        //{
        //    List<T> temp = new List<T>();

        //    chestMin = chests.X;
        //    chestMax = chests.Y;

        //    if (rooms.X < 1) rooms.X = 1;
        //    Room<T> room = new Room<T>(origin, 0, 0);
        //    int maxRooms = rand.Next(rooms.X, rooms.Y);

        //    RoomList = new List<Room<T>>();
        //    BuildRoom(1, maxRooms, origin.X, origin.Y, width.X, width.Y, height.X, height.Y, ref room, CardinalDirections.NORTH);


        //    return RoomList;
        //}
        //public List<Room<T>> BuildDungeon (Vector2 rooms)
        //{
        //    List<Room<T>> tempAvailable = new List<Room<T>>(AvailableRooms);

        //    if (rooms.X < 1) rooms.X = 1;
        //    List<Room<T>> temp = new List<Room<T>>();

        //    int roomCount = rand.Next(rooms.X, rooms.Y);

        //    for (int i = 0; i < roomCount; i++)
        //    {
        //        int roomIndex = rand.Next(0, AvailableRooms.Count - 1);


        //    }

        //    return null;
        //}

        //public void SaveDungeon(string directory)
        //{
        //    // we haven't generated a dungeon so don't try to save it
        //    //if (rootRoom == null) { return; }
        //    string jsonData = JsonConvert.SerializeObject(RoomList);

        //    if (!System.IO.Directory.Exists($"{directory}")) { System.IO.Directory.CreateDirectory($"{directory}"); }
        //    if (!System.IO.File.Exists($"{directory}/map.json")) { System.IO.File.Create($"{directory}/map.json"); }

        //    using (System.IO.StreamWriter writer = new System.IO.StreamWriter($"{directory}/map.json"))
        //    {
        //        writer.WriteLine(jsonData);
        //        writer.Close();
        //    }
        //}
        //public List<Room<T>> LoadDungeon(string directory)
        //{
        //    string jsonData = "";
        //    using (System.IO.StreamReader reader = new System.IO.StreamReader($"{directory}/map.json"))
        //    {
        //        jsonData = reader.ReadLine();
        //        reader.Close();
        //    }
        //    RoomList = JsonConvert.DeserializeObject<List<Room<T>>>(jsonData);
        //    //foreach (var room in RoomList) { GetTileRoots(room); }

        //    return RoomList;
        //}

        // Recursive Function 
        // Base case: roomNum > maxRooms
        //private int BuildRoom(int roomNum, int maxRooms, int startX, int startY, int minWidth, int maxWidth, int minHeight, int maxHeight, ref Room<T> lastRoom, CardinalDirections roomDir)
        //{
        //    List<ITile<T>> temp = new List<ITile<T>>();

        //    // base case
        //    if (roomNum > maxRooms)
        //        return roomNum;

        //    int roomWidth = rand.Next(minWidth, maxWidth);
        //    int roomHeight = rand.Next(minHeight, maxHeight);
        //    int doorX = 0, doorY = 0;
        //    if (roomNum != 1)
        //        switch (roomDir)
        //        {
        //            case CardinalDirections.NORTH:
        //                startX = lastRoom.Origin.X;
        //                startY = lastRoom.Origin.Y - roomHeight - 1;

        //                doorY = startY + roomHeight - 1;
        //                doorX = ((startX + roomWidth) / 2);
        //                break;
        //            case CardinalDirections.SOUTH:
        //                startX = lastRoom.Origin.X;
        //                startY = lastRoom.Origin.Y + lastRoom.Height;

        //                doorY = startY;
        //                doorX = ((startX + roomWidth) / 2);
        //                break;
        //            case CardinalDirections.EAST:
        //                startX = lastRoom.Origin.X + lastRoom.Width;
        //                startY = lastRoom.Origin.Y;

        //                doorY = ((startY + roomHeight) / 2);
        //                doorX = startX;
        //                break;
        //            case CardinalDirections.WEST:
        //                startX = lastRoom.Origin.X - roomWidth;
        //                startY = lastRoom.Origin.Y;

        //                doorY = ((startY + roomHeight) / 2);
        //                doorX = startX + roomWidth - 1;
        //                break;
        //        }

        //    Room<T> room = new Room<T>(new Vector2(startX, startY), roomWidth, roomHeight);

        //    for (int y = startY; y < roomHeight + startY; y++)
        //    {
        //        for (int x = startX; x < roomWidth + startX; x++)
        //        {
        //            temp.Add(placeCorners(startX, startY, x, y, roomWidth + startX, roomHeight + startY));
        //            // clean up default values

        //            temp.Add(placeWalls(startX, startY, x, y, roomWidth + startX, roomHeight + startY));
        //            // clean up default values

        //            temp.Add(placeFloors(startX, startY, x, y, roomWidth + startX, roomHeight + startY));
        //            // clean up default values

        //        }
        //    }
        //    temp.RemoveAll(t => t == null);

        //    SetAdjacency(roomNum, roomDir, ref room, ref lastRoom);
        //    room.Tiles = temp;
        //    room.ID = roomNum;
        //    PrepareWorldObjects(room.Tiles.Count, room.WorldObjects);

        //    //Generate door for this room and respective door in last room
        //    if (roomNum != 1)
        //        placeDoors(startX, startY, roomWidth, roomHeight, doorX, doorY, lastRoom, roomDir, room.WorldObjects);

        //    // generate loot chests and place down
        //    placeChests(startX, startY, roomWidth, roomHeight, room.WorldObjects);

        //    var availability = FindAvailability(room);

        //    int roomsToAdd = rand.Next(0, availability.Count);
        //    if (roomsToAdd > maxRooms)
        //        roomsToAdd = maxRooms;
        //    int maxRoomNum = roomNum;
        //    CardinalDirections[] nextRooms = new CardinalDirections[roomsToAdd];

        //    for (int i = 0; i < roomsToAdd; i++)
        //    {
        //        var j = rand.Next(0, 3);
        //        nextRooms[i] = (CardinalDirections)Enum.Parse(typeof(CardinalDirections), (j).ToString());
        //    }

        //    for (int i = 0; i < roomsToAdd; i++)
        //    {
        //        // the startx and starty should be the position of the last door, offset by a certain amount in the direction ofthe axis not wall dominant.
        //        maxRoomNum = BuildRoom(i + maxRoomNum, maxRooms, startX, startY, minWidth, maxWidth, minHeight, maxHeight, ref room, nextRooms[i]);
        //    }
        //    RoomList.Add(room);
        //    return maxRoomNum;
        //}
        //private Room<T> GetRoom (int index)
        //{
        //    return null;
        //}

        //private void PrepareWorldObjects(int count, List<ITile<T>> worldObjects)
        //{
        //    for (int i = 0; i < count; i++)
        //        worldObjects.Add(new ITile<T>());
        //}
        //private void SetAdjacency(int roomNum, CardinalDirections roomDir, ref Room<T> room, ref Room<T> lastRoom)
        //{
        //    if (roomNum != 1)
        //    {
        //        switch (roomDir)
        //        {
        //            case CardinalDirections.NORTH:
        //                room.SouthID = lastRoom.ID;
        //                lastRoom.NorthID = room.ID;
        //                break;
        //            case CardinalDirections.SOUTH:
        //                room.NorthID = lastRoom.ID;
        //                lastRoom.SouthID = room.ID;
        //                break;
        //            case CardinalDirections.EAST:
        //                room.WestID = lastRoom.ID;
        //                lastRoom.EastID = room.ID;
        //                break;
        //            case CardinalDirections.WEST:
        //                room.EastID = lastRoom.ID;
        //                lastRoom.WestID = room.ID;
        //                break;
        //        }
        //    }
        //    else
        //        lastRoom = room;
        //}
        //private List<CardinalDirections> FindAvailability(Room<T> room)
        //{
        //    // generate a random number of rooms to create
        //    // based on available walls
        //    List<CardinalDirections> available = new List<CardinalDirections>();
        //    for (int i = 0; i < 4; i++)
        //    {
        //        switch (i)
        //        {
        //            case (int)CardinalDirections.NORTH:
        //                if (room.NorthID == -1)
        //                    available.Add(CardinalDirections.NORTH);
        //                break;
        //            case (int)CardinalDirections.SOUTH:
        //                if (room.SouthID == -1)
        //                    available.Add(CardinalDirections.SOUTH);
        //                break;
        //            case (int)CardinalDirections.EAST:
        //                if (room.EastID == -1)
        //                    available.Add(CardinalDirections.EAST);
        //                break;
        //            case (int)CardinalDirections.WEST:
        //                if (room.WestID == -1)
        //                    available.Add(CardinalDirections.WEST);
        //                break;
        //        }
        //    }
        //    return available;
        //}

        //private ITile<T> placeCorners(int startX, int startY, int x, int y, int roomWidth, int roomHeight)
        //{
        //    ITile<T> tile = new ITile<T>();
        //    // If the x and y value pair does not equate to a corner, then return the default
        //    // value for type T and stop execution for this function
        //    if ((x != startX && x != roomWidth - 1) && (y != startY && y != roomHeight - 1))
        //        return default;

        //    // if x and y equates to TopLeft Corner
        //    if (x == startX && y == startY)
        //    {
        //        tile.Root = NORTHWEST_CORNER;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.NORTHWEST_CORNER;
        //        //place north west corner
        //        return tile;
        //    }

        //    // if x and y equates to TopRight corner
        //    else if (x == roomWidth - 1 && y == startY)
        //    {
        //        //place north east corner
        //        tile.Root = NORTHEAST_CORNER;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.NORTHEAST_CORNER;
        //        return tile;

        //    }

        //    // if x and y equates to BottomLeft corner
        //    else if (y == roomHeight - 1 && x == startX)
        //    {
        //        //place south west corner
        //        tile.Root = SOUTHWEST_CORNER;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.SOUTHWEST_CORNER;
        //        return tile;

        //    }

        //    // if x and y equates to BottomRight corner
        //    else if (y == roomHeight - 1 && x == roomWidth - 1)
        //    {
        //        //place south east corner
        //        tile.Root = SOUTHEAST_CORNER;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.SOUTHEAST_CORNER;
        //        return tile;

        //    }
        //    else return default;
        //}
        //private ITile<T> placeWalls(int startX, int startY, int x, int y, int roomWidth, int roomHeight)
        //{
        //    // If the x and y value pair does not equate to a wall, then return the default
        //    // value for type T and stop execution for this function
        //    if ((x == startX || x == roomWidth - 1) && (y == startY || y == roomHeight - 1))
        //        return default;
        //    ITile<T> tile; // = new ITile<T>();

        //    // If x is on the Left Side of the room
        //    if (x == startX)
        //    {
        //        // place west wall
        //        tile.Root = WEST_WALL;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.WEST_WALL;
        //        return tile;
        //    }
        //    // If x is on the Right Side of the room
        //    else if (x == roomWidth - 1)
        //    {
        //        // place east wall
        //        tile.Root = EAST_WALL;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.EAST_WALL;
        //        return tile;
        //    }
        //    // If y is on the Top Side of the room
        //    else if (y == startY)
        //    {
        //        // place north wall
        //        tile.Root = NORTH_WALL;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.NORTH_WALL;
        //        return tile;
        //    }
        //    // If Y is on the Bottom Side of the room
        //    else if (y == roomHeight - 1)
        //    {
        //        // place south wall
        //        tile.Root = SOUTH_WALL;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.SOUTH_WALL;
        //        return tile;
        //    }
        //    else return default;
        //}
        //private ITile<T> placeFloors(int startX, int startY, int x, int y, int roomWidth, int roomHeight)
        //{
        //    ITile<T> tile = new ITile<T>();
        //    if ((x >= startX + 1 && x <= roomWidth - 2) && (y >= startY + 1 && y <= roomHeight - 2))
        //    {
        //        tile.Root = FLOOR;
        //        tile.WorldPoint = new Vector2(x + startX, y + startY);
        //        tile.ID = TILE_IDS.FLOOR;
        //        return tile;
        //    }
        //    else return default;
        //}

        //public enum CardinalDirections { NORTH, SOUTH, EAST, WEST }
        //private void placeDoors(int startX, int startY, int roomWidth, int roomHeight, int doorX, int doorY, Room<T> lastRoom, CardinalDirections side, List<ITile<T>> worldObjects)
        //{
        //    switch (side)
        //    {
        //        case CardinalDirections.NORTH:
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].Root = NORTH_DOOR;
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].ID = TILE_IDS.NORTH_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].Root = SOUTH_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].ID = TILE_IDS.SOUTH_DOOR;
        //            break;
        //        case CardinalDirections.SOUTH:
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].Root = SOUTH_DOOR;
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].ID = TILE_IDS.SOUTH_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].Root = NORTH_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].ID = TILE_IDS.NORTH_DOOR;
        //            break;
        //        case CardinalDirections.EAST:
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].Root = EAST_DOOR;
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].ID = TILE_IDS.EAST_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].Root = WEST_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].ID = TILE_IDS.WEST_DOOR;
        //            break;
        //        case CardinalDirections.WEST:
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].Root = WEST_DOOR;
        //            lastRoom.WorldObjects[(doorX - lastRoom.Origin.X) + (lastRoom.Height - 1) * roomWidth].ID = TILE_IDS.WEST_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].Root = EAST_DOOR;
        //            worldObjects[(doorX - startX) + (doorY - startY) * roomWidth].ID = TILE_IDS.EAST_DOOR;
        //            break;
        //    }
        //}
        //private void placeChests(int startX, int startY, int roomWidth, int roomHeight, List<ITile<T>> worldObjects)
        //{
        //    try
        //    {
        //        int chestX = 0, chestY = 0, chestCount = 0;
        //        chestCount = rand.Next(chestMin, chestMax);
        //        for (int i = 0; i < chestCount; i++)
        //        {
        //            chestX = rand.Next(startX + 1, (roomWidth + startX) - 1);
        //            chestY = rand.Next(startY + 1, (roomHeight + startY) - 1);

        //            worldObjects[(chestX - startX) + (chestY - startY) * roomWidth].Root = SMALL_CHEST;
        //            worldObjects[(chestX - startX) + (chestY - startY) * roomWidth].ID = TILE_IDS.SMALL_CHEST;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private void GetTileRoots(Room<T> room)
        //{
        //    for (int i = 0; i < room.Tiles.Count; i++)
        //    {
        //        room.Tiles[i].Root = tiles_available[room.Tiles[i].ID];
        //        if (room.WorldObjects[i].ID > 0) { room.WorldObjects[i].Root = tiles_available[room.WorldObjects[i].ID]; }
        //    }
        //}
    }
}
