using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public interface IManager
    {
        public IRoom ActiveRoom { get; }
        public List<ITransition> Transitions { get; }
    }
}
