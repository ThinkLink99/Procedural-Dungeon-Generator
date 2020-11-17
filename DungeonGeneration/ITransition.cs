using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public interface ITransition
    {
        public IRoom TargetRoom { get; }
    }
}
