namespace DungeonGeneration
{
    //public enum TILE_NAMES
    //{
    //    NORTH_WALL, SOUTH_WALL, EAST_WALL, WEST_WALL,
    //    NORTHWEST_CORNER, NORTHEAST_CORNER, SOUTHWEST_CORNER, SOUTHEAST_CORNER,
    //    FLOOR,
    //    NORTH_DOOR, SOUTH_DOOR, EAST_DOOR, WEST_DOOR,
    //    SMALL_CHEST
    //}
    public interface ITile<T>
    {
        public int ID { get; }
        public T Sprite { get; }
        public float X { get; }
        public float Y { get; }

        public bool Dense { get; }
    }
}
