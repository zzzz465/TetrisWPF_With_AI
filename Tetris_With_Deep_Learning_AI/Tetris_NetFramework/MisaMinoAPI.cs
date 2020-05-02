using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisaMinoNET
{
    public static class MisaMinoAPI
    {
        static object FindMoveLocker = new object();
        public static async Task<Solution> FindMove()
        { // not thread-safe, returned task is thread-safe
            throw new NotImplementedException();
            lock(FindMoveLocker)
            {

            }
        }
    }
}