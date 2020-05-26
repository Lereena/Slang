using System.Collections.Generic;
using SimpleParser;

namespace ProgramTree
{
    public class FuncList
    {
        private Dictionary<string, FuncDefNode> functions;

        public FuncList() => functions = new Dictionary<string, FuncDefNode>();

        public void Add(string name, FuncDefNode func) => functions[name] = func;

        public FuncDefNode Find(string name) => functions[name];
    }
}