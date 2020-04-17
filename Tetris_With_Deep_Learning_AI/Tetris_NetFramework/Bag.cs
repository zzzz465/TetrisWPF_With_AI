using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public class TetrominoBag
    {
        Queue<Tetromino> minos = new Queue<Tetromino>();
        Tetromino[] template = new Tetromino[7] { Tetromino.I, Tetromino.J, Tetromino.L, Tetromino.O, Tetromino.S, Tetromino.T, Tetromino.Z };
        Random random;
        public TetrominoBag(Random random)
        {
            this.random = random;
        }
        public TetrominoBag()
        {
            random = new Random();
        }
        public TetrominoBag(int seed)
        {
            random = new Random(seed);
        }

        public Tetromino Peek(int index)
        {
            while(minos.Count <= index)
                fillBag();

            return minos.ElementAt(index);
        }

        public Tetromino GetNext()
        {
            if(minos.Count == 0)
                fillBag();
            
            return minos.Dequeue();
        }

        void fillBag()
        {
            Shuffle();
            foreach(var mino in template)
                minos.Enqueue(mino);
        }

        void Shuffle()
        {
            int n = template.Count();
            while(n > 1)
            {
                n--;
                int k = random.Next(n+1);
                var value = template[k];
                template[k] = template[n];
                template[n] = value;
            }
        }
    }
}