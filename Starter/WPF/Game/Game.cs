namespace LangtonsAnt
{
    public class Game : IGame
    {
        protected const int defaultSize = 128;
        public int GenerationN { get; set; } = 0;
        public byte[,] Field { get; set; }
        public IAnt[] Ants { get; set; }
        public int Size
        {
            get => Field?.GetLength(0) ?? defaultSize;
        }

        public Game(int size, IAnt[]? initialAnts)
        {
            Field = new byte[size, size];
            Ants = initialAnts ?? new IAnt[] { };
        }

        public Game() : this(defaultSize, null) { }

        private byte[,] CalcNextGeneration()
        {
            var newField = (byte[,])Field.Clone();

      for (int index = Ants.Length - 1; index >= 0; index--)
      {
        var ant = Ants[index];

        // Check if the ant is still within the field
        if (ant.I < 0 || ant.J < 0 || ant.J >= Size || ant.I >= Size)
        {
          // TODO later we will act on ants going out of the field, 
          //      now we just exclude them from processing
          continue;
        }

        byte v = newField[ant.I, ant.J];
        int i = ant.I;
        int j = ant.J;
        byte newVal = ant.Act(v);
        newField[i, j] = newVal;
      }

      return newField;
    }

    public void NextGeneration()
    {
      Field = CalcNextGeneration();
      GenerationN++;
    }
  }
}