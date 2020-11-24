using Newtonsoft.Json;
using System.Collections.Generic;

namespace DungeonGeneration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Room<T> : IRoom
    {
        int id = 0,
            northId = 0,
            southId = 0,
            eastId = 0,
            westId = 0,
            w = 0,
            h = 0;

        float x = 0,
              y = 0;

        [JsonProperty]
        public virtual int ID { get => id; set => id = value; }
        [JsonProperty]
        public virtual int NorthID { get => northId; set => northId = value; }
        [JsonProperty]
        public virtual int SouthID { get => southId; set => southId = value; }
        [JsonProperty]
        public virtual int EastID { get => eastId; set => eastId = value; }
        [JsonProperty]
        public virtual int WestID { get => westId; set => westId = value; }
        [JsonProperty]
        public virtual float OriginX { get => x; set => x = value; }
        [JsonProperty]
        public virtual float OriginY { get => y; set => y = value; }

        [JsonProperty]
        public virtual int Width => w;
        [JsonProperty]
        public virtual int Height => h;

        [JsonProperty]
        public virtual List<Tile<T>> Tiles { get; set; }
    }
}
