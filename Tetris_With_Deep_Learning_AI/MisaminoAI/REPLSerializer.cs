using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Tetris;

namespace MisaminoAI
{
    public struct Data
    {
        public Tetromino[] bag;
        public Tetromino current;
        public Tetromino hold;
        public int height;
        public IEnumerable<TetrisLine> field;
        public int combo;
        public bool b2b;
        public int incomingGarbage;
    }
    public static class REPLSerializer
    {
        public static string Serialize(Data dataToSerialize)
        {
            return JsonConvert.SerializeObject(dataToSerialize);
        }

        public static Data Deserialize(string SerializedData)
        {
            return JsonConvert.DeserializeObject<Data>(SerializedData);
        }
    }
}