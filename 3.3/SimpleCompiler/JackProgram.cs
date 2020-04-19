using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class JackProgram : JackProgramElement
    {
        public List<VarDeclaration> Globals { get; private set; }
        public List<Function> Functions { get; private set; }
        public Function Main { get; private set; }


        public override void Parse(TokensStack sTokens)
        {
            Globals = new List<VarDeclaration>();
            while ((sTokens.Peek() is Statement) && ((Statement)sTokens.Peek()).Name == "var")
            {
                VarDeclaration global = new VarDeclaration();
                Token tVar = sTokens.Peek();
                Token tType = sTokens.Peek(1);
                global.Parse(sTokens);
                if (!(tVar is Statement) || (((Statement)tVar).Name != "var"))
                    throw new SyntaxErrorException("Expected var, received " + tVar, tVar);
                Globals.Add(global);
            }
            
            Main = new Function();
            Main.Parse(sTokens);
            Functions = new List<Function>();
            if(sTokens.Count == 0)
                throw new SyntaxErrorException("Expected }", sTokens.Peek());
            while (sTokens.Count > 0)
            {
                
                if (!(sTokens.Peek() is Statement) || ((Statement)sTokens.Peek()).Name != "function")
                    throw new SyntaxErrorException("Expected function", sTokens.Peek());
                Function f = new Function();
                f.Parse(sTokens);
                Functions.Add(f);
            }
        }

        public override string ToString()
        {
            string sProgram = "";
            foreach (VarDeclaration v in Globals)
                sProgram += "\t" + v + "\n";
            sProgram += "\t" + Main + "\n";
            foreach (Function f in Functions)
                sProgram += "\t" + f + "\n";
            return sProgram;
        }
    }
}
