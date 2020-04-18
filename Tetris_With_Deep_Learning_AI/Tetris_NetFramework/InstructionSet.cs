using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Collections;

namespace Tetris
{
    public class InstructionSet : IEnumerator<Instruction>
    {
        public List<Instruction> instructions;
        public readonly IEnumerable<Point> expectedPoints;
        Instruction currentInstruction = Instruction.None;
        public int index { get; private set; } = -1;
        public int Length { get { return instructions.Count; } }

        public Instruction Current => currentInstruction;

        object IEnumerator.Current => currentInstruction;

        public InstructionSet(IEnumerable<Instruction> insts, IEnumerable<Point> expectedPoints)
        {
            this.instructions = new List<Instruction>(insts);
            this.expectedPoints = expectedPoints;
        }

        public void Dispose()
        {
            instructions = null;
        }

        public bool MoveNext()
        {
            if(++index < instructions.Count)
            {
                currentInstruction = instructions[index];
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}