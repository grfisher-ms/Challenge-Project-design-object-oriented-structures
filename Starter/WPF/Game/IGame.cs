using System.Collections.Generic;

namespace LangtonsAnt
{
    public interface IGame
    {
        public int Size { get; }
        public IAnt[] Ants { get; set; }
        public int GenerationN { get; }
        public byte[,] Field { get; }
        public void NextGeneration();
    }

}
