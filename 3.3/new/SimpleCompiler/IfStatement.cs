using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class IfStatement : StatetmentBase
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> DoIfTrue { get; private set; }
        public List<StatetmentBase> DoIfFalse { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            //throw new NotImplementedException();
            //First, we remove the "if" token
            Token tIf = sTokens.Pop();//if
            //After the name there should be opening paranthesis for the expression
            Token tOpen = sTokens.Pop(); //(
            if (!(tOpen is Parentheses) || ((Parentheses)tOpen).Name != '(')
                throw new SyntaxErrorException("Expected ( received: " + tOpen, tOpen);
            DoIfTrue = new List<StatetmentBase>();
            DoIfFalse = new List<StatetmentBase>();
            //Now we extract the expression from the stack until we see a closing parathesis
            Term = Expression.Create(sTokens);
            Term.Parse(sTokens);
            //Now we pop out the {. Note that you need to check that the stack contains the correct symbols here.
            Token tEnd = sTokens.Pop();//)
            Token tIfstart = sTokens.Peek();//{
            if (tIfstart.ToString() != "{")
            {
                StatetmentBase stm = StatetmentBase.Create(sTokens.Pop());
                stm.Parse(sTokens);
                DoIfTrue.Add(stm);
            }
            else
            {
                tIfstart = sTokens.Pop();//{
                while (sTokens.Count > 0 && sTokens.Peek().ToString() != "}")
                {
                    //Token stm = sTokens.Peek();
                    StatetmentBase stm = StatetmentBase.Create(sTokens.Peek());
                    stm.Parse(sTokens);
                    DoIfTrue.Add(stm);
                }
                Token tIfEnd = sTokens.Pop();//}
                if (!(tIfEnd is Parentheses) || ((Parentheses)tIfEnd).Name != '}')
                    throw new SyntaxErrorException("Expected } received: " + tIfEnd, tIfEnd);
            }
            Token tElseStart = sTokens.Peek();//else
            if (tElseStart.ToString() == "else")
            {
                tElseStart = sTokens.Pop();//else
                Token tElsebegin = sTokens.Pop();//{ else begin
                if (!(tElsebegin is Parentheses) || ((Parentheses)tElsebegin).Name != '{')
                    throw new SyntaxErrorException("Expected { received: " + tElsebegin, tElsebegin);
                while (sTokens.Count > 0 && sTokens.Peek().ToString() != "}")
                {
                    StatetmentBase stm = StatetmentBase.Create(sTokens.Peek());
                    stm.Parse(sTokens);
                    DoIfFalse.Add(stm);
                }
                Token tElseEnd = sTokens.Pop();//}
                if (!(tElseEnd is Parentheses) || ((Parentheses)tElseEnd).Name != '}')
                    throw new SyntaxErrorException("Expected } received: " + tElseEnd, tElseEnd);
            }
        }

        public override string ToString()
        {
            string sIf = "if(" + Term + "){\n";
            foreach (StatetmentBase s in DoIfTrue)
                sIf += "\t\t\t" + s + "\n";
            sIf += "\t\t}";
            if (DoIfFalse.Count > 0)
            {
                sIf += "else{";
                foreach (StatetmentBase s in DoIfFalse)
                    sIf += "\t\t\t" + s + "\n";
                sIf += "\t\t}";
            }
            return sIf;
        }

    }
}
