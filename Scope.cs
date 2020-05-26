using System.Collections.Generic;

namespace ProgramTree
{
    public class Scope
    {
        public readonly Dictionary<string, dynamic> table;
        public readonly Scope parent;

        public Scope(Scope parent = null)
        {
            table = new Dictionary<string, dynamic>();
            this.parent = parent;
        }

        public void Add(dynamic name)
        {
            if (table.ContainsKey(name))
                throw new ExecEvalException("Переменная уже определена");
            table.Add(name, "def");
        }

        public void MoveToParent()
        {
            if (parent == null)
                return;

            foreach (var (key, value) in table)
                if (parent.table.ContainsKey(key))
                    parent.table[key] = value;
        }
    }
}