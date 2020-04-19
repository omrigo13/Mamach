using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class UnaryOperatorExpression : Expression
    {
        public string Operator { get; set; }
        public Expression Operand { get; set; }

        public override string ToString()
        {
            return Operator + Operand;
        }

        public override void Parse(TokensStack sTokens)
        {
            //throw new NotImplementedException();
            Token tOp = sTokens.Pop();//-/!
            if (!(tOp is Operator) || ((Operator)tOp).Name != '-'  && ((Operator)tOp).Name != '!')
                throw new SyntaxErrorException("Expected operator - or ! received: " + tOp, tOp);
            Operator = tOp.ToString();
            Token tStart = sTokens.Peek();//(
            if (!(sTokens.Peek() is Operator) && Operand == null)
            {
                Operand = Expression.Create(sTokens);
                Operand.Parse(sTokens);
            }
        }
    }
}
