using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSBASI3
{
    public class ScopedMemory
    {
        private readonly Dictionary<string, TypedValue> memory;
        public ScopedMemory EnclosingScope { get; set; }
        public string Name { get; private set; }
        public int Level { get; private set; }

        public TypedValue this[string name]
        {
            get
            {
                TypedValue value;
                if (!memory.TryGetValue(name, out value) && EnclosingScope != null)
                {
                    value = EnclosingScope[name];
                }
                return value;
            }

            set { memory[name] = value; }
        }

        public ScopedMemory(string name, int level, ScopedMemory enclosingScope)
        {
            Name = name;
            Level = level;
            EnclosingScope = enclosingScope;
            memory = new Dictionary<string, TypedValue>();
        }

        public static ScopedMemory ProgramMemory(string programName)
        {
            return new ScopedMemory(programName, 0, null);
        }

        public void AddAtLevel(string name, TypedValue value, int scopeLevel)
        {
            if (scopeLevel > Level)
            {
                throw new AccessViolationException(
                    $"Cannot write in memory scope deeper than the current level. Requested {scopeLevel} current is {Level}");
            }

            if (Level == scopeLevel)
            {
                memory[name] = value;
            }
            else
            {
                EnclosingScope.AddAtLevel(name, value, scopeLevel);
            }
        }
    }
}