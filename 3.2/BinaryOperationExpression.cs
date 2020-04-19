using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class BinaryOperationExpression : Expression
    {
        public string Operator { get;  set; }
        public Expression Operand1 { get;  set; }
        public Expression Operand2 { get;  set; }

        public override string ToString()
        {
            return "(" + Operand1 + " " + Operator + " " + Operand2 + ")";
        }

        public override void Parse(TokensStack sTokens)
        {
            //throw new NotImplementedException();
            //First, we remove the "(" token
            Token tStart = sTokens.Pop();//(
            if (!(tStart is Parentheses) || ((Parentheses)tStart).Name != '(')
                throw new SyntaxErrorException("Expected ( received: " + tStart, tStart);
            Operand1 = Expression.Create(sTokens);
            Operand1.Parse(sTokens);
            Token tOP = sTokens.Pop();
            if (!(tOP is Operator))
                throw new SyntaxErrorException("Expected operator received: " + tOP, tOP);
            Operator = tOP.ToString();
            Operand2 = Expression.Create(sTokens);
            Operand2.Parse(sTokens);
            Token tEnd = sTokens.Pop();//)
            if (!(tEnd is Parentheses) || ((Parentheses)tEnd).Name != ')')
                throw new SyntaxErrorException("Expected ) received: " + tEnd, tEnd);
        }
    }
}
