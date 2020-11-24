using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    [Serializable]
    public class Transition<T> : ITile<T>, ITransition
    {
        IRoom targetRoom;

        int id = 0;
        float x = 0,
              y = 0;
        bool dense = false;

        public virtual IRoom TargetRoom { get => targetRoom; set => targetRoom = value; }
        public virtual int ID => id;
        public virtual T Sprite { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public virtual float X { get => x; set => x = value; }
        public virtual float Y { get => y; set => y = value; }
        public virtual bool Dense { get => dense; set => dense = value; }
    }
}
