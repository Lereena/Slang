%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

BOOL true|false
Alpha [a-zA-Z_]
Digit [0-9]
AlphaDigit {Alpha}|{Digit}
INTNUM {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}*

%%

{REALNUM} {
    IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
    yylval.dVal = double.Parse(yytext, formatter);
    return (int)Tokens.RNUM;
}

{INTNUM} {
    yylval.iVal = int.Parse(yytext);
    return (int)Tokens.INUM;
}

{BOOL} {
    yylval.bVal = bool.Parse(yytext);
    return (int)Tokens.BOOL;
}

{ID} {
    int res = ScannerHelper.GetIDToken(yytext);
    if (res == (int)Tokens.ID)
    yylval.sVal = yytext;
    return res;
}

":=" { return (int)Tokens.ASSIGN; }
":" { return (int)Tokens.COLON; }
";" { return (int)Tokens.SEMICOLON; }
"(" { return (int)Tokens.LB; }
")" { return (int)Tokens.RB; }
"," { return (int)Tokens.COMMA; }
"+" { return (int)Tokens.ADD; }
"-" { return (int)Tokens.SUB; }
"*" { return (int)Tokens.MULT; }
"/" { return (int)Tokens.DIV; }
"=" { return (int)Tokens.EQUAL; }
">" { return (int)Tokens.MORE; }
"<" { return (int)Tokens.LESS; }
"=>" { return (int)Tokens.ARROW; }

[^ \r\n] {
    LexError();
}

%{
    yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol);
%}

%%

public override void yyerror(string format, params object[]args)
{
    var ww = args.Skip(1).Cast<string>().ToArray();
    string errorMsg = string.Format("({0},{1}): Met {2}, but expected {3}", yyline, yycol, args[0], string.Join(" or ", ww));
    throw new SyntaxException(errorMsg);
}

public void LexError()
{
    string errorMsg = string.Format("({0},{1}): Unknown symbol {2}", yyline, yycol, yytext);
    throw new LexException(errorMsg);
}

class ScannerHelper
{
    private static Dictionary<string,int> keywords;

    static ScannerHelper()
    {
        keywords = new Dictionary<string,int>();
        keywords.Add("begin",(int)Tokens.BEGIN);
        keywords.Add("end",(int)Tokens.END);
        keywords.Add("cycle",(int)Tokens.CYCLE);
        keywords.Add("while",(int)Tokens.WHILE);
        keywords.Add("do",(int)Tokens.DO);
        keywords.Add("repeat",(int)Tokens.REPEAT);
        keywords.Add("until",(int)Tokens.UNTIL);
        keywords.Add("for",(int)Tokens.FOR);
        keywords.Add("to",(int)Tokens.TO);
        keywords.Add("write",(int)Tokens.WRITE);
        keywords.Add("if",(int)Tokens.IF);
        keywords.Add("then",(int)Tokens.THEN);
        keywords.Add("else",(int)Tokens.ELSE);
        keywords.Add("func", (int)Tokens.FUNC);
    }

    public static int GetIDToken(string s)
    {
        if (keywords.ContainsKey(s.ToLower()))
            return keywords[s];
        else
            return (int)Tokens.ID;
    }
}