using System.Collections.Generic;
using System;

namespace ProgramTree
{
    public class Node
    {
    }

    public class ExprNode : Node
    {
        public virtual dynamic Eval(Scope scope) => 0;
    }

    public class IdNode : ExprNode
    {
        public string Name { get; }

        public IdNode(string name) => Name = name;

        public override dynamic Eval(Scope scope)
        {
            var localScope = scope;
            while (localScope != null)
            {
                if (localScope.table.ContainsKey(Name))
                    return localScope.table[Name];
                localScope = localScope.parent;
            }

            throw new ExecEvalException("Переменной " + Name + " не существует в текущем контексте");
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; }

        public IntNumNode(int num) => Num = num;

        public override dynamic Eval(Scope scope) => Num;
    }

    public class RealNumNode : ExprNode
    {
        public double Num { get; }

        public RealNumNode(double num) => Num = num;

        public override dynamic Eval(Scope scope) => Num;
    }

    public class BoolValNode : ExprNode
    {
        public bool Val { get; }

        public BoolValNode(bool val) => Val = val;

        public override dynamic Eval(Scope scope) => Val;
    }

    public class NumOpNode : ExprNode
    {
        public ExprNode Left { get; }
        public ExprNode Right { get; }
        public char Operation { get; }

        public NumOpNode(ExprNode left, ExprNode right, char operation)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public override dynamic Eval(Scope scope)
            => Operation switch
            {
                '+' => Left.Eval(scope) + Right.Eval(scope),
                '-' => Left.Eval(scope) - Right.Eval(scope),
                '*' => Left.Eval(scope) * Right.Eval(scope),
                '/' => Left.Eval(scope) / Right.Eval(scope),
                '(' => Left.Eval(scope),
                _ => 0
            };
    }

    public class LogicOpNode : ExprNode
    {
        public ExprNode Left { get; }
        public ExprNode Right { get; }
        public string Operation { get; }

        public LogicOpNode(ExprNode left, ExprNode right, string operation)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }

        public override dynamic Eval(Scope scope)
            => Operation switch
            {
                "=" => Left.Eval(scope) == Right.Eval(scope),
                ">" => Left.Eval(scope) > Right.Eval(scope),
                "<" => Left.Eval(scope) < Right.Eval(scope),
                _ => 0
            };
    }

    public class FuncDefNode : ExprNode
    {
        public readonly ParametersNode parametersNode;
        public readonly BlockNode funcBody;

        public FuncDefNode(ParametersNode parametersNode, BlockNode funcBody)
        {
            this.parametersNode = parametersNode;
            this.funcBody = funcBody;
        }

        public override dynamic Eval(Scope scope)
        {
            funcBody.Exec(scope);
            return scope.table["Ret"];
        }
    }

    public class FunNode : ExprNode
    {
        public string name;
        public FactParametersNode factParameters;
        public FuncList funcs;

        public FunNode(string name, FuncList funcs, FactParametersNode factParameters)
        {
            this.name = name;
            this.factParameters = factParameters;
            this.funcs = funcs;
        }

        public override dynamic Eval(Scope scope)
        {
            var tScope = scope;
            dynamic func = null;
            while (tScope != null)
            {
                func = tScope.table.GetValueOrDefault(name, null);
                if (func == null)
                    tScope = tScope.parent;
                else break;
            }

            if (func == null || !(func is FuncDefNode))
                func = funcs.Find(name);
            var localScope = new Scope(scope);
            localScope.Add("Ret");
            for (var i = 0; i < factParameters.parameters.Count; i++)
            {
                var funcParam = func.parametersNode.parameters[i];
                var factParam = factParameters.parameters[i];
                localScope.Add(funcParam);
                localScope.table[funcParam] = factParam is FuncDefNode
                    ? factParam : factParam.Eval(scope);
            }

            return func.Eval(localScope);
        }
    }

    public class ParametersNode
    {
        public List<string> parameters;

        public ParametersNode(string param) => parameters = new List<string> {param};

        public void Add(string name) => parameters.Add(name);
    }

    public class FactParametersNode
    {
        public List<ExprNode> parameters;

        public FactParametersNode(ExprNode param)
            => parameters = new List<ExprNode> {param};

        public void Add(ExprNode param) => parameters.Add(param);
    }

    
    public class StatementNode : Node
    {
        public virtual void Exec(Scope scope)
        {
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();

        public BlockNode(StatementNode stat) => Add(stat);

        public void Add(StatementNode stat) => StList.Add(stat);

        public override void Exec(Scope scope)
        {
            var localScope = new Scope(scope);
            foreach (var st in StList)
                st.Exec(localScope);

            localScope.MoveToParent();
        }
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; }
        public ExprNode Expr { get; }

        public AssignNode(IdNode id, ExprNode expr)
        {
            Id = id;
            Expr = expr;
        }

        public override void Exec(Scope scope)
        {
            scope.table[Id.Name] = Expr is FuncDefNode ? Expr : Expr.Eval(scope);
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; }
        public StatementNode Stat { get; }

        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }

        public override void Exec(Scope scope)
        {
            var localScope = new Scope(scope);
            for (var i = 0; i < (int) Expr.Eval(scope); i++)
                Stat.Exec(localScope);
            localScope.MoveToParent();
        }
    }

    public class WhileNode : StatementNode
    {
        public LogicOpNode Logic { get; }
        public StatementNode Stat { get; }

        public WhileNode(LogicOpNode logic, StatementNode stat)
        {
            Logic = logic;
            Stat = stat;
        }

        public override void Exec(Scope scope)
        {
            var localScope = new Scope(scope);
            while (Logic.Eval(localScope))
                Stat.Exec(localScope);
            localScope.MoveToParent();
        }
    }

    public class RepeatNode : StatementNode
    {
        public LogicOpNode Logic { get; }
        public StatementNode Stat { get; }

        public RepeatNode(LogicOpNode logic, StatementNode stat)
        {
            Logic = logic;
            Stat = new BlockNode(stat);
        }

        public override void Exec(Scope scope)
        {
            var localScope = new Scope(scope);
            do
                Stat.Exec(localScope);
            while (Logic.Eval(localScope));

            localScope.MoveToParent();
        }
    }

    public class ForNode : StatementNode
    {
        public AssignNode Assign { get; }
        public ExprNode Expr { get; }
        public StatementNode Stat { get; }

        public ForNode(AssignNode assign, ExprNode expr, StatementNode stat)
        {
            Assign = assign;
            Expr = expr;
            Stat = stat;
        }

        public override void Exec(Scope scope)
        {
            var localScope = new Scope(scope);
            localScope.Add(Assign.Id.Name);
            localScope.table[Assign.Id.Name] = Assign.Expr.Eval(scope);
            var to = Expr.Eval(scope);

            for (; localScope.table[Assign.Id.Name] <= to; localScope.table[Assign.Id.Name]++)
            {
                Stat.Exec(localScope);
            }

            localScope.MoveToParent();
        }
    }

    public class WriteNode : StatementNode
    {
        public ExprNode Expr { get; }

        public WriteNode(ExprNode expr) => Expr = expr;

        public override void Exec(Scope scope) => Console.WriteLine(Expr.Eval(scope));
    }

    public class IfNode : StatementNode
    {
        public LogicOpNode Logic { get; }
        public BlockNode StList { get; }

        public IfNode(LogicOpNode logic, StatementNode stat)
        {
            Logic = logic;
            StList = new BlockNode(stat);
        }

        public IfNode(LogicOpNode logic, StatementNode stat1, StatementNode stat2)
        {
            Logic = logic;
            StList = new BlockNode(stat1);
            StList.Add(stat2);
        }

        public override void Exec(Scope scope)
        {
            var localScope = new Scope(scope);
            if (Logic.Eval(localScope))
                StList.StList[0].Exec(localScope);
            else if (StList.StList.Count == 2)
                StList.StList[1].Exec(localScope);

            localScope.MoveToParent();
        }
    }
    
    class ExecEvalException : Exception
    {
        public ExecEvalException(string message)
            : base(message)
        {
        }
    }
}