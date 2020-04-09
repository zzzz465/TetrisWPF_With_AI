using System;
using System.Collections.Generic;
using System.Linq;
using Tetris;
using ColdClear;

namespace ColdClear
{
    // wrapper class for cold clear API
    public class ColdClear
    {
        #region static methods and variables
        public static ColdClear CreateInstance(CCOptions options, CCWeights weights)
        {
            return new ColdClear(options, weights);
        }

        public static ColdClear CreateInstance()
        {
            CCOptions options;
            ColdClearAPI.cc_default_options(out options);
            CCWeights weights;
            ColdClearAPI.cc_default_weights(out weights);
            return new ColdClear(options, weights);
        }
        #endregion
        IntPtr CCBot;
        protected ColdClear(CCOptions options, CCWeights weights)
        {
            CCBot = ColdClearAPI.cc_launch_async(ref options, ref weights);
            if(ColdClearAPI.cc_is_dead_async(CCBot))
                throw new Exception("CCBot died immediately!");
        }

        public void AddMino(Tetromino mino)
        {
            ColdClearAPI.cc_add_next_piece_async(CCBot, mino.ToCCPiece());
        }

        void Reset()
        {

        }
    }
}