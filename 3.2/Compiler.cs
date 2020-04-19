using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class Compiler
    {

        private Dictionary<string, int> m_dSymbolTable;
        private int m_cLocals;

        public Compiler()
        {
            m_dSymbolTable = new Dictionary<string, int>();
            m_cLocals = 0;

        }

        public List<string> Compile(string sInputFile)
        {
            List<string> lCodeLines = ReadFile(sInputFile);
            List<Token> lTokens = Tokenize(lCodeLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            JackProgram program = Parse(sTokens);
            return null;
        }

        private JackProgram Parse(TokensStack sTokens)
        {
            JackProgram program = new JackProgram();
            program.Parse(sTokens);
            return program;
        }

        public List<string> Compile(List<string> lLines)
        {

            List<string> lCompiledCode = new List<string>();
            foreach (string sExpression in lLines)
            {
                List<string> lAssembly = Compile(sExpression);
                lCompiledCode.Add("// " + sExpression);
                lCompiledCode.AddRange(lAssembly);
            }
            return lCompiledCode;
        }



        public List<string> ReadFile(string sFileName)
        {
            StreamReader sr = new StreamReader(sFileName);
            List<string> lCodeLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lCodeLines.Add(sr.ReadLine());
            }
            sr.Close();
            return lCodeLines;
        }

        //Computes the next token in the string s, from the begining of s until a delimiter has been reached. 
        //Returns the string without the token.
        private string Next(string s, char[] aDelimiters, out string sToken, out int cChars)
        {
            cChars = 1;
            sToken = s[0] + "";
            if (aDelimiters.Contains(s[0]))
                return s.Substring(1);
            int i = 0;
            for (i = 1; i < s.Length; i++)
            {
                if (aDelimiters.Contains(s[i]))
                    return s.Substring(i);
                else
                    sToken += s[i];
                cChars++;
            }
            return null;
        }

        public List<Token> Tokenize(List<string> lCodeLines)
        {
            //throw new NotImplementedException();
             List<Token> lTokens = new List<Token>();
            //your code here
            List<char> puncs = new List<char>();
            puncs.AddRange(Token.Operators);
            puncs.AddRange(Token.Parentheses);
            puncs.AddRange(Token.Separators);
            puncs.Add(' ');
            char[] deli = puncs.ToArray();
            string meaning;
            int indexPos = 0;
            Token t;
            int num;
            for(int i = 0; i < lCodeLines.Count; i++)
            {
                if(lCodeLines[i].Contains("//") || lCodeLines[i] == "\t")
                    continue;
                string line = lCodeLines[i];
                meaning = line;
                int finalPos = 0;
                while (line != "")
                {
                    if (line != "")
                        line = Next(line, deli, out meaning, out indexPos);
                    //finalPos = lCodeLines[i].IndexOf(meaning) + lCodeLines[i].Count(ch => ch == '\t');
                    if (meaning != " ")
                    {
                        if (meaning.Contains("\t"))
                        {
                            meaning = meaning.Replace("\t", "");
                            indexPos = lCodeLines[i].Count(ch => ch == '\t');
                            finalPos = indexPos;
                            indexPos = meaning.Length;
                        }
                        
                        if (Token.Statements.Contains(meaning))
                        { 
                            t = new Statement(meaning, i, finalPos);
                            lTokens.Add(t);
                        }
                        else if (Token.VarTypes.Contains(meaning))
                        {
                            t = new VarType(meaning, i, finalPos);
                            lTokens.Add(t);
                        }
                        else if (Token.Constants.Contains(meaning))
                        {
                            t = new Constant(meaning, i, finalPos);
                            lTokens.Add(t);
                        }
                        else if (meaning.Length == 1 && Token.Operators.Contains(char.Parse(meaning)))
                        {
                            t = new Operator(char.Parse(meaning), i, finalPos);
                            lTokens.Add(t);
                        }
                        else if (meaning.Length == 1 && Token.Parentheses.Contains(char.Parse(meaning)))
                        {
                            t = new Parentheses(char.Parse(meaning), i, finalPos);
                            lTokens.Add(t);
                        }
                        else if (meaning.Length == 1 && Token.Separators.Contains(char.Parse(meaning)))
                        {
                            t = new Separator(char.Parse(meaning), i, finalPos);
                            lTokens.Add(t); 
                        }
                        else if (Int32.TryParse(meaning, out num))
                        {
                            t = new Number(meaning, i, finalPos);
                            lTokens.Add(t); }
                        else if(meaning != "")//should improve to alphanumeric if its not working
                        { 
                            t = new Identifier(meaning, i, finalPos);
                            lTokens.Add(t);
                        } 
                    }

                    finalPos += indexPos;
                }
            }
            return lTokens;
        }

    }
}
