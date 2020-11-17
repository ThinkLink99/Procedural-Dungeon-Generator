using System.Collections.Generic;
namespace DungeonGeneration
{
    public interface IRoom
    {
        public int ID { get; set; }

        public int NorthID { get; set; }
        public int SouthID { get; set; }
        public int EastID { get; set; }
        public int WestID { get; set; }

        public float OriginX { get; }
        public float OriginY { get; }

        public int Width { get; }
        public int Height { get; }
        //public List<ITile<T>> Tiles { get; set; }
        //public List<ITile<T>> WorldObjects { get; set; }
    }
}
