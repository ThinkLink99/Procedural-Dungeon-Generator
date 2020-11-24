using System.Collections.Generic;

namespace DungeonGeneration
{
    public interface IManager<T>
    {
        public IRoom ActiveRoom { get; }
        public List<ITransition> Transitions { get; }

        public DungeonGeneration<T> Dungeon { get; }
        public int Seed { get; }

        public object[] TileSprites { get; }
        public int MaximumRoomCount { get; }
    }
}
