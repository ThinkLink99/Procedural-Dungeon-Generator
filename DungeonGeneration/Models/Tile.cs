using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Tile <T> : ITile <T>
    {
        int   id = 0;
        float x = 0,
              y = 0;
        bool  dense = false;

        [JsonProperty]
        public virtual int ID { get => id; set => id = value; }

        public virtual T Sprite { get; set; }

        [JsonProperty]
        public virtual float X { get => x; set => x = value; }

        [JsonProperty]
        public virtual float Y { get => y; set => y = value; }

        [JsonProperty]
        public virtual bool Dense { get => dense; set => dense = value; }
    }
}
