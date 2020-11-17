namespace DungeonGeneration
{
    public enum TILE_IDS
    {
        NORTH_WALL, SOUTH_WALL, EAST_WALL, WEST_WALL,
        NORTHWEST_CORNER, NORTHEAST_CORNER, SOUTHWEST_CORNER, SOUTHEAST_CORNER,
        FLOOR,
        NORTH_DOOR, SOUTH_DOOR, EAST_DOOR, WEST_DOOR,
        SMALL_CHEST
    }
    public interface ITile
    {
        public TILE_IDS ID { get; }
        public object Sprite { get; }
        public float X { get; }
        public float Y { get; }

        public bool Dense { get; }
    }
}
