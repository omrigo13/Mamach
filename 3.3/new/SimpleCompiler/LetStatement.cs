using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class LetStatement : StatetmentBase
    {
        public string Variable { get; set; }
        public Expression Value { get; set; }

        public override string ToString()
        {
            return "let " + Variable + " = " + Value + ";";
        }

        public override void Parse(TokensStack sTokens)
        {
            //throw new NotImplementedException();
            //First, we remove the "let" token
            Token tLet = sTokens.Pop();//let
            if (!(tLet is Statement) || ((Statement)tLet).Name != "let")
                throw new SyntaxErrorException("Expected let received: " + tLet, tLet);
            //Next is the variable name
            Token tName = sTokens.Pop();
            if (!(tName is Identifier) || (char.IsDigit(tName.ToString()[0])))
                throw new SyntaxErrorException("Expected variable Name received: " + tName, tName);
            Variable = ((Identifier)tName).Name;
            //Next, we remove the "=" token
            sTokens.Pop();//=
            if (!(sTokens.LastPop is Operator) || ((Operator)sTokens.LastPop).Name != '=')
                throw new SyntaxErrorException("Expected = received: " + sTokens.LastPop, sTokens.LastPop);
            Value = Expression.Create(sTokens);
            Value.Parse(sTokens);
            Token tEnd = sTokens.Pop();//;
            if (!(tEnd is Separator) || ((Separator)tEnd).Name != ';')
                throw new SyntaxErrorException("Expected ; received: " + tEnd, tEnd);

        }

    }
}
