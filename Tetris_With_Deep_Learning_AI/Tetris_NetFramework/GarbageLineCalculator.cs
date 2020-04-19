using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Tetris
{
    public interface iGarbageLineCalculator
    {
        int Calculate(int deletedLine, TSpinType tspinType, bool b2b, int combo);

        int CalculatePerfectClear(int deletedLine, TSpinType tspinType, bool b2b, int combo);
    }

    public class FallbackGarbageLineCalculator : iGarbageLineCalculator
    {
        ILog Log = LogManager.GetLogger("FallbackGarbageLineCalculator");
        public FallbackGarbageLineCalculator()
        {
            Log.Warn("You're using fallback garbage line calculator, it must used in debug mode");
        }
        

        public int Calculate(int deletedLine, TSpinType tspinType, bool b2b, int combo)
        {
            int damage = deletedLine;

            if(tspinType == TSpinType.Spin)
                damage += 1;
            if(b2b)
                damage += 1;

            return deletedLine;
        }
        
        public int CalculatePerfectClear(int deletTSpinine, TSpinType tspinType, bool b2b, int combo)
        {
            return 8;
        }
        
    }
}