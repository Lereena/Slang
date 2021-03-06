﻿using System;

namespace SimpleParser
{
    public class LexException : Exception
    {
        public LexException(string msg) : base(msg) { }
    }
    public class SyntaxException : Exception
    {
        public SyntaxException(string msg) : base(msg) { }
    }
    
    public class SemanticException : Exception
    {
        public SemanticException(string msg) : base(msg) {}
    }

    public class WrongTypeException : Exception
    {
        public WrongTypeException(string msg) : base(msg) {}
    }
    
    // Класс глобальных описаний и статических методов
    // для использования различными подсистемами парсера и сканера
    public static class ParserHelper
    {
    }
}