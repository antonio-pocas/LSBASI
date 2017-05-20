using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class StackFrame
    {
        private readonly Dictionary<string, TypedValue> memory;
        public StackFrame PreviousFrame { get; set; }
        public string Name { get; private set; }
        public int Depth { get; private set; }

        public TypedValue this[string name]
        {
            get
            {
                TypedValue value;
                if (!memory.TryGetValue(name, out value) && PreviousFrame != null)
                {
                    value = PreviousFrame[name];
                }
                return value;
            }

            set { memory[name] = value; }
        }

        public StackFrame(string name, int depth, StackFrame previousFrame)
        {
            Name = name;
            Depth = depth;
            PreviousFrame = previousFrame;
            memory = new Dictionary<string, TypedValue>();
        }

        public static StackFrame ProgramMemory(string programName)
        {
            return new StackFrame(programName, 0, null);
        }

        public void AddAtDepth(string name, TypedValue value, int scopeLevel)
        {
            if (scopeLevel > Depth)
            {
                throw new AccessViolationException(
                    $"Cannot write in memory scope deeper than the current level. Requested {scopeLevel} current is {Depth}");
            }

            if (Depth == scopeLevel)
            {
                memory[name] = value;
            }
            else
            {
                PreviousFrame.AddAtDepth(name, value, scopeLevel);
            }
        }
    }
}