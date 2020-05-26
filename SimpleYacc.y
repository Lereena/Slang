%{
public BlockNode root; //корневой узел синтаксического дерева
public FuncList funcs = new FuncList(); //таблица символов
public Parser(AbstractScanner<ValueType, LexLocation>
scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%union {
public double dVal;
public int iVal;
public string sVal;
public bool bVal;
public Node nVal;
public ExprNode eVal;
public StatementNode stVal;
public BlockNode blVal;
public ParametersNode pVal;
public FactParametersNode fpVal;
}

%using ProgramTree;
%namespace SimpleParser

%token BEGIN END CYCLE ASSIGN SEMICOLON WHILE DO REPEAT UNTIL FOR TO WRITE LB RB ARROW
%token IF THEN ELSE VAR COMMA ADD SUB MULT DIV EQUAL MORE LESS FUNC COLON
%token <iVal> INUM
%token <dVal> RNUM
%token <sVal> ID
%token <bVal> BOOL
%type <eVal> expr ident T F logic_op func
%type <stVal> assign statement cycle while repeat for write if
%type <blVal> stlist block
%type <pVal> parameters
%type <fpVal> fact_parameters

%%

progr   : block { root = $1; }
        | func progr
        ;

func    : FUNC ID LB parameters RB block { funcs.Add($2, new FuncDefNode($4,  $6)); }
        ;

parameters : ID { $$ = new ParametersNode($1); }
           | parameters COMMA ID { $$.Add($3); }
           ;

fact_parameters : expr { $$ = new FactParametersNode($1); }
                | fact_parameters SEMICOLON expr { $$.Add($3); }
                ;
      
stlist  : statement { $$ = new BlockNode($1); }
        | stlist SEMICOLON statement
        {
            $1.Add($3);
            $$ = $1;
        } 
        ;
      
statement   : assign { $$ = $1; }
            | block { $$ = $1; }
            | cycle { $$ = $1; }
            | while { $$ = $1; }
            | repeat { $$ = $1; }
            | for { $$ = $1; }
            | write { $$ = $1; }
            | if { $$ = $1; }
            ;
        
ident   : ID { $$ = new IdNode($1); }
        ;
      
assign  : ident ASSIGN expr { $$ = new AssignNode($1 as IdNode, $3); }
        | ident ASSIGN logic_op { $$ = new AssignNode($1 as IdNode, $3); }
        ;

logic_op    : BOOL { $$ = new BoolValNode($1);}
            | expr EQUAL expr { $$ = new LogicOpNode($1, $3, "=");}
            | expr MORE expr { $$ = new LogicOpNode($1, $3, ">");}
            | expr LESS expr { $$ = new LogicOpNode($1, $3, "<");}
            ;

expr    : T
        | expr ADD T {$$ = new NumOpNode($1, $3, '+');}
        | expr SUB T {$$ = new NumOpNode($1, $3, '-');}
        ;

T   : F
    | T MULT F {$$ = new NumOpNode($1, $3, '*');}
    | T DIV F {$$ = new NumOpNode($1, $3, '/');}
    ;

F   : ident { $$ = $1 as IdNode; }
    | INUM { $$ = new IntNumNode($1); }
    | RNUM { $$ = new RealNumNode($1); }
    | ID LB fact_parameters RB { $$ = new FunNode($1, funcs, $3); }
    | parameters ARROW block { $$ = new FuncDefNode($1, $3); } // lambda
    | LB expr RB { $$ = new NumOpNode($2, null, '('); }
    ;

block   : BEGIN stlist END { $$ = $2; }
        ;

cycle   : CYCLE expr statement { $$ = new CycleNode($2, $3); }
        ;

while   : WHILE logic_op DO statement { $$ = new WhileNode($2 as LogicOpNode, $4); }
        ;

repeat  : REPEAT statement UNTIL logic_op { $$ = new RepeatNode($4 as LogicOpNode, $2 as StatementNode); }
        ;

for     : FOR assign TO expr DO statement { $$ = new ForNode($2 as AssignNode, $4, $6); }
        ;

write   : WRITE LB expr RB { $$ = new WriteNode($3); }
        ;

if      : IF LB logic_op RB THEN statement { $$ = new IfNode($3 as LogicOpNode, $6); }
        | IF LB logic_op RB THEN statement ELSE statement { $$ = new IfNode($3 as LogicOpNode, $6, $8); }
        ;

%%
