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

        public float OriginX { get; set; }
        public float OriginY { get; set; }

        public int Width { get; }
        public int Height { get; }
        //public List<ITile> Tiles { get; set; }
    }
}
