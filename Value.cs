namespace SimpleLang
{
    public enum VarType
    {
        INT,
        DOUBLE,
        BOOL,
        CHAR,
        STRING
    }
    
    public class Value
    {
        public VarType type;
        public int intVal { get; }
        public double doubleVal;
        public bool boolVal;
        public char charVal;
        public string stringVal;

        public Value(int value)
        {
            type = VarType.INT;
            intVal = value;
        }

        public Value(double value)
        {
            type = VarType.DOUBLE;
            doubleVal = value;
        }

        public Value(bool value)
        {
            type = VarType.BOOL;
            boolVal = value;
        }

        public Value(char value)
        {
            type = VarType.CHAR;
            charVal = value;
        }

        public Value(string value)
        {
            type = VarType.STRING;
            stringVal = value;
        }
    }
}