using System;
using System.Collections.Generic;

namespace SimpleCompiler
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; private set; }
        public List<Expression> Args { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            //throw new NotImplementedException();
            //First, we remove the function ID Name token
            Token tFuncName = sTokens.Pop();//Function Name
            FunctionName = ((Identifier) tFuncName).Name;
            Token tStart = sTokens.Pop();//(
            if (!(tStart is Parentheses) || ((Parentheses)tStart).Name != '(')
                throw new SyntaxErrorException("Expected ( received: " + tStart, tStart);
            Args = new List<Expression>();
            while (sTokens.Count > 0 && (sTokens.Peek().ToString() != ";") && (sTokens.Peek().ToString() != ")"))
            { 
                Expression exp = Expression.Create(sTokens);
                exp.Parse(sTokens);
                Args.Add(exp);
                Token tEndLine = sTokens.Pop();
            }
        }
    
        public override string ToString()
        {
            string sFunction = FunctionName + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i] + ",";
            if (Args.Count > 0)
                sFunction += Args[Args.Count - 1];
            sFunction += ")";
            return sFunction;
        }
    }
}