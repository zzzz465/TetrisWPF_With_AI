using System;
using System.Linq;
using System.Collections.Generic;
using log4net;
using log4net.Core;

namespace Tetris
{
    public static class ILogExtension
    {
        static Level DebugAILevel = new Level(25000, "Debug_AI", "DEBUG_AI");
        static ILogExtension()
        {
            LogManager.GetRepository().LevelMap.Add(DebugAILevel);
        }
        public static void DebugAI(this ILog log, object message, Exception exception)
        {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, DebugAILevel, message, exception);
        }

        public static void DebugAI(this ILog log, object message)
        {
            DebugAI(log, message, null);
        }
    }
}