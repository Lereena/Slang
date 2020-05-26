using System;
using System.Collections.Generic;
using SimpleParser;

namespace SimpleLang
{
    public class Semitable
    {
        // Может быть написать сюда что-нибудь
    }
    
    public class VariablesTable
    {
        private readonly Dictionary<string, Value> VarTable =
            new Dictionary<string, Value>();
        
        public void Place(string name, Value val) => VarTable[name] = val;

        //public void Unplace(string name) => VarTable.Remove(name);

        public bool Contains(string name) => VarTable.ContainsKey(name);

        public Value GetVal(string name)
        {
            if (VarTable.ContainsKey(name))
                return VarTable[name];

            throw new SemanticException("Переменная " + name + " не объявлена");
        }
    }
}