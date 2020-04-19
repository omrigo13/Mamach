using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class WhileStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> Body { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            //throw new NotImplementedException();
            //First, we remove the "while" token
            Token tWhile = sTokens.Pop();//while
            if (!(tWhile is Statement) || ((Statement)tWhile).Name != "while")
                throw new SyntaxErrorException("Expected while received: " + tWhile, tWhile);
            //After the name there should be opening paranthesis for the expression
            Token tOpen = sTokens.Pop(); //(
            if (!(tOpen is Parentheses) || ((Parentheses)tOpen).Name != '(')
                throw new SyntaxErrorException("Expected ( received: " + tOpen, tOpen);
            Body = new List<StatetmentBase>();
            //Now we extract the expression from the stack until we see a closing parathesis
            Term = Expression.Create(sTokens);
            Term.Parse(sTokens);
            //Now we pop out the {. Note that you need to check that the stack contains the correct symbols here.
            Token tEnd = sTokens.Pop();//)
            if (!(tEnd is Parentheses) || ((Parentheses)tEnd).Name != ')')
                throw new SyntaxErrorException("Expected ) received: " + tEnd, tEnd);
            Token tWhileStart = sTokens.Pop();//{
            if (!(tWhileStart is Parentheses) || ((Parentheses)tWhileStart).Name != '{')
                throw new SyntaxErrorException("Expected { received: " + tWhileStart, tWhileStart);
            while (sTokens.Count > 0 && sTokens.Peek().ToString() != "}")
            {
                StatetmentBase stm = StatetmentBase.Create(sTokens.Peek());
                if(!(stm is StatetmentBase))
                    throw new SyntaxErrorException("Expected var received: " + sTokens.Peek(), sTokens.Peek());
                stm.Parse(sTokens);
                Body.Add(stm);
            }
            Token tWhileEnd = sTokens.Pop();//}
            if (!(tWhileEnd is Parentheses) || ((Parentheses)tWhileEnd).Name != '}')
                throw new SyntaxErrorException("Expected } received: " + tWhileEnd, tWhileEnd);
        }

        public override string ToString()
        {
            string sWhile = "while(" + Term + "){\n";
            foreach (StatetmentBase s in Body)
                sWhile += "\t\t\t" + s + "\n";
            sWhile += "\t\t}";
            return sWhile;
        }

    }
}
