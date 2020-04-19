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


        public Compiler()
        {

        }


        public List<VarDeclaration> ParseVarDeclarations(List<string> lVarLines)
        {
            List<VarDeclaration> lVars = new List<VarDeclaration>();
            for(int i = 0; i < lVarLines.Count; i++)
            {
                List<Token> lTokens = Tokenize(lVarLines[i], i);
                TokensStack stack = new TokensStack(lTokens);
                VarDeclaration var = new VarDeclaration();
                var.Parse(stack);
                lVars.Add(var);
            }
            return lVars;
        }


        public List<LetStatement> ParseAssignments(List<string> lLines)
        {
            List<LetStatement> lParsed = new List<LetStatement>();
            List<Token> lTokens = Tokenize(lLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            while(sTokens.Count > 0)
            {
                LetStatement ls = new LetStatement();
                ls.Parse(sTokens);
                lParsed.Add(ls);

            }
            return lParsed;
        }

 

        public List<string> GenerateCode(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
            List<string> lAssembly = new List<string>();
            //add here code for computing a single let statement containing only a simple expression
            if (aSimple.Value is NumericExpression)
            {
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (!dSymbolTable.ContainsKey(aSimple.Variable))
                {
                    Token t = new Identifier(aSimple.Variable,0,0);
                    throw new SyntaxErrorException("key already in map: " + ((Identifier)t).Name, t);
                }
                lAssembly.Add("@" + dSymbolTable[aSimple.Variable]);
                lAssembly.Add("D=D+A");
                lAssembly.Add("@ADDRESS");
                lAssembly.Add("M=D");
                lAssembly.Add("@" + aSimple.Value);
                lAssembly.Add("D=A");
                lAssembly.Add("@ADDRESS");
                lAssembly.Add("A=M");
                lAssembly.Add("M=D");
            }
            
            else if (aSimple.Value is BinaryOperationExpression && ((BinaryOperationExpression) (aSimple.Value)).Operator == "+")
            {
                string operator1 = ((BinaryOperationExpression) (aSimple.Value)).Operand1.ToString();
                string operator2 = ((BinaryOperationExpression) (aSimple.Value)).Operand2.ToString();
                // Console.WriteLine(operator1);
                // Console.WriteLine(operator2);
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (!dSymbolTable.ContainsKey(aSimple.Variable))
                {
                    Token t = new Identifier(aSimple.Variable,0,0);
                    throw new SyntaxErrorException("key already in map: " + ((Identifier)t).Name, t);
                }
                lAssembly.Add("@" + dSymbolTable[aSimple.Variable]);
                lAssembly.Add("A=A+D");
                lAssembly.Add("D=A");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (dSymbolTable.ContainsKey(operator1))
                {
                    lAssembly.Add("@" + dSymbolTable[operator1]);
                    lAssembly.Add("A=A+D");
                    lAssembly.Add("D=M");
                }
                else
                {
                    lAssembly.Add("@" + operator1);
                    lAssembly.Add("D=A");
                }
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("M=D");
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (dSymbolTable.ContainsKey(operator2))
                {
                    lAssembly.Add("@" + dSymbolTable[operator2]);
                    lAssembly.Add("A=A+D");
                    lAssembly.Add("D=M");
                }
                else
                {
                    lAssembly.Add("@" + operator2);
                    lAssembly.Add("D=A");
                }
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("M=D");
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("D=M");
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("D=D+M");
                lAssembly.Add("@RESULT");
                lAssembly.Add("A=M");
                lAssembly.Add("M=D");
            }
            
            else if (aSimple.Value is BinaryOperationExpression && ((BinaryOperationExpression) (aSimple.Value)).Operator == "-")
            {
                string operator1 = ((BinaryOperationExpression) (aSimple.Value)).Operand1.ToString();
                string operator2 = ((BinaryOperationExpression) (aSimple.Value)).Operand2.ToString();
                // Console.WriteLine(operator1);
                // Console.WriteLine(operator2);
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (!dSymbolTable.ContainsKey(aSimple.Variable))
                {
                    Token t = new Identifier(aSimple.Variable,0,0);
                    throw new SyntaxErrorException("key already in map: " + ((Identifier)t).Name, t);
                }
                lAssembly.Add("@" + dSymbolTable[aSimple.Variable]);
                lAssembly.Add("A=A+D");
                lAssembly.Add("D=A");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (dSymbolTable.ContainsKey(operator1))
                {
                    lAssembly.Add("@" + dSymbolTable[operator1]);
                    lAssembly.Add("A=A+D");
                    lAssembly.Add("D=M");
                }
                else
                {
                    lAssembly.Add("@" + operator1);
                    lAssembly.Add("D=A");
                }
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("M=D");
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (dSymbolTable.ContainsKey(operator2))
                {
                    lAssembly.Add("@" + dSymbolTable[operator2]);
                    lAssembly.Add("A=A+D");
                    lAssembly.Add("D=M");
                }
                else
                {
                    lAssembly.Add("@" + operator2);
                    lAssembly.Add("D=A");
                }
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("M=D");
                lAssembly.Add("@OPERAND1");
                lAssembly.Add("D=M");
                lAssembly.Add("@OPERAND2");
                lAssembly.Add("D=D-M");
                lAssembly.Add("@RESULT");
                lAssembly.Add("A=M");
                lAssembly.Add("M=D");
            }

            else if (aSimple.Value is VariableExpression)
            {
                // string name = ((VariableExpression) (aSimple.Value)).Name;
                // lAssembly.Add("@LCL");
                // lAssembly.Add("D=M");
                // if (!dSymbolTable.ContainsKey(aSimple.Variable))
                // {
                //     Token t = new Identifier(aSimple.Variable,0,0);
                //     throw new SyntaxErrorException("key already in map: " + ((Identifier)t).Name, t);
                // }
                // lAssembly.Add("@" + dSymbolTable[aSimple.Variable]);
                // lAssembly.Add("A=A+D");
                // lAssembly.Add("D=A");
                // lAssembly.Add("@RESULT");
                // lAssembly.Add("M=D");
                // lAssembly.Add("@LCL");
                // lAssembly.Add("D=M");
                // if (dSymbolTable.ContainsKey(name))
                // {
                //     lAssembly.Add("@" + dSymbolTable[name]);
                //     lAssembly.Add("A=A+D");
                //     lAssembly.Add("D=M");
                // }
                // else
                // {
                //     lAssembly.Add("@" + name);
                //     lAssembly.Add("D=A");
                // }
                // lAssembly.Add("@RESULT");
                // lAssembly.Add("A=M");
                // lAssembly.Add("D=M");
                string name = ((VariableExpression) (aSimple.Value)).Name;
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (!dSymbolTable.ContainsKey(name))
                {
                    Token t = new Identifier(aSimple.Variable,0,0);
                    throw new SyntaxErrorException("key already in map: " + ((Identifier)t).Name, t);
                }
                lAssembly.Add("@" + dSymbolTable[name]);
                lAssembly.Add("A=A+D");
                lAssembly.Add("D=M");
                lAssembly.Add("@RESULT");
                lAssembly.Add("M=D");
                lAssembly.Add("@LCL");
                lAssembly.Add("D=M");
                if (dSymbolTable.ContainsKey(aSimple.Variable))
                {
                    lAssembly.Add("@" + dSymbolTable[aSimple.Variable]);
                    lAssembly.Add("D=A+D");
                    lAssembly.Add("@ADDRESS");
                    lAssembly.Add("M=D");
                }
                else
                {
                    lAssembly.Add("@" + aSimple.Variable);
                    lAssembly.Add("D=A");
                    lAssembly.Add("@ADDRESS");
                    lAssembly.Add("M=D");
                }
                lAssembly.Add("@RESULT");
                lAssembly.Add("D=M");
                lAssembly.Add("@ADDRESS");
                lAssembly.Add("A=M");
                lAssembly.Add("M=D");
            }
            return lAssembly;
        }


        public Dictionary<string, int> ComputeSymbolTable(List<VarDeclaration> lDeclerations)
        {
            Dictionary<string, int> dTable = new Dictionary<string, int>();
            //add here code to comptue a symbol table for the given var declarations
            //real vars should come before (lower indexes) than artificial vars (starting with _), and their indexes must be by order of appearance.
            //for example, given the declarations:
            //var int x;
            //var int _1;
            //var int y;
            //the resulting table should be x=0,y=1,_1=2
            //throw an exception if a var with the same name is defined more than once
            int count = 0;
            foreach (VarDeclaration variable in lDeclerations)
            {
                if (!variable.Name.StartsWith("_"))
                {
                    if (dTable.ContainsKey(variable.Name))
                    {
                        Token t = new Identifier(variable.Name,0,0);
                        throw new SyntaxErrorException("key already in map: " + ((Identifier)t).Name, t);
                    }
                    dTable.Add(variable.Name, count);
                    count++;
                }
            }
            
            foreach (VarDeclaration variable in lDeclerations)
            {
                if (variable.Name.StartsWith("_"))
                {
                    if (dTable.ContainsKey(variable.Name))
                    {
                        Token t = new Identifier(variable.Name,0,0);
                        throw new SyntaxErrorException("key already in map: " + ((Identifier)t).Name, t);
                    }
                    dTable.Add(variable.Name, count);
                    count++;
                }
            }
            return dTable;
        }
        
        public List<string> GenerateCode(List<LetStatement> lSimpleAssignments, List<VarDeclaration> lVars)
        {
            List<string> lAssembly = new List<string>();
            Dictionary<string, int> dSymbolTable = ComputeSymbolTable(lVars);
            foreach (LetStatement aSimple in lSimpleAssignments)
                lAssembly.AddRange(GenerateCode(aSimple, dSymbolTable));
            return lAssembly;
        }

        public List<LetStatement> SimplifyExpressions(LetStatement s, List<VarDeclaration> lVars)
        {
            //add here code to simply expressins in a statement. 
            //add var declarations for artificial variables.
            List<LetStatement> statements = new List<LetStatement>();
            List<LetStatement> recstatements = new List<LetStatement>();
            List<string> vars = new List<string>();
            VarDeclaration var;
            LetStatement stm1 = new LetStatement();
            LetStatement stm2 = new LetStatement();
            LetStatement f1 = new LetStatement();
            LetStatement f2 = new LetStatement();
            if (s.ToString().Substring(s.ToString().IndexOf("=")).Contains("("))
            {
                if ((((BinaryOperationExpression) (s.Value)).Operand1 is BinaryOperationExpression))
                {
                    stm1.Value = ((BinaryOperationExpression) (s.Value)).Operand1;
                    if (lVars[lVars.Count - 1].Name.Contains("_"))
                    {
                        string value = lVars[lVars.Count - 1].Name;
                        value = value.Substring(value.IndexOf("_") + 1,value.Length - 1);
                        int finalval = Int32.Parse(value);
                        stm1.Variable = "_" + (finalval + 1);
                    }
                    else
                        stm1.Variable = "_1";

                    recstatements = SimplifyExpressions(stm1, lVars);
                    if (recstatements.Count > 0)
                        f1 = recstatements[recstatements.Count - 1];
                    foreach (LetStatement st in recstatements)
                    {
                        var = new VarDeclaration("int", st.Variable);
                        if (!lVars.Contains(var))
                            lVars.Add(var);
                        statements.Add(st);
                    }
                }

                if ((((BinaryOperationExpression) (s.Value)).Operand2 is BinaryOperationExpression))
                {
                    stm2.Value = ((BinaryOperationExpression) (s.Value)).Operand2;
                    if (lVars[lVars.Count - 1].Name.Contains("_"))
                    {
                        string value = lVars[lVars.Count - 1].Name;
                        value = value.Substring(value.IndexOf("_") + 1,value.Length - 1);
                        int finalval = Int32.Parse(value);
                        stm2.Variable = "_" + (finalval + 1);
                    }
                    else
                        stm2.Variable = "_1";

                    recstatements = SimplifyExpressions(stm2, lVars);
                    if (recstatements.Count > 0)
                        f2 = recstatements[recstatements.Count - 1];
                    foreach (LetStatement st in recstatements)
                    {
                        var = new VarDeclaration("int", st.Variable);
                        if (!lVars.Contains(var))
                            lVars.Add(var);
                        statements.Add(st);
                    }
                }

                stm1 = new LetStatement();
                if ((((BinaryOperationExpression) (s.Value)).Operand1 is NumericExpression))
                {
                    stm1.Value = ((BinaryOperationExpression) (s.Value)).Operand1;
                    if (lVars[lVars.Count - 1].Name.Contains("_"))
                    {
                        string value = lVars[lVars.Count - 1].Name;
                        value = value.Substring(value.IndexOf("_") + 1,value.Length - 1);
                        int finalval = Int32.Parse(value);
                        stm1.Variable = "_" + (finalval + 1);
                    }
                    else
                        stm1.Variable = "_1";

                    statements.Add(stm1);
                    var = new VarDeclaration("int", stm1.Variable);
                    if (!lVars.Contains(var))
                        lVars.Add(var);
                }

                stm1 = new LetStatement();
                if ((((BinaryOperationExpression) (s.Value)).Operand1 is VariableExpression))
                {
                    stm1.Value = ((BinaryOperationExpression) (s.Value)).Operand1;
                    if (lVars[lVars.Count - 1].Name.Contains("_"))
                    {
                        string value = lVars[lVars.Count - 1].Name;
                        value = value.Substring(value.IndexOf("_") + 1,value.Length - 1);
                        int finalval = Int32.Parse(value);
                        stm1.Variable = "_" + (finalval + 1);
                    }
                    else
                        stm1.Variable = "_1";

                    statements.Add(stm1);
                    var = new VarDeclaration("int", stm1.Variable);
                    if (!lVars.Contains(var))
                        lVars.Add(var);
                }
                
                stm2 = new LetStatement();
                if ((((BinaryOperationExpression) (s.Value)).Operand2 is NumericExpression))
                {
                    stm2.Value = ((BinaryOperationExpression) (s.Value)).Operand2;
                    if (lVars[lVars.Count - 1].Name.Contains("_"))
                    {
                        string value = lVars[lVars.Count - 1].Name;
                        value = value.Substring(value.IndexOf("_") + 1,value.Length - 1);
                        int finalval = Int32.Parse(value);
                        stm2.Variable = "_" + (finalval + 1);
                    }
                    else
                        stm2.Variable = "_1";

                    statements.Add(stm2);
                    var = new VarDeclaration("int", stm2.Variable);
                    if (!lVars.Contains(var))
                        lVars.Add(var);
                }

                stm2 = new LetStatement();
                if ((((BinaryOperationExpression) (s.Value)).Operand2 is VariableExpression))
                {
                    stm2.Value = ((BinaryOperationExpression) (s.Value)).Operand2;
                    if (lVars[lVars.Count - 1].Name.Contains("_"))
                    {
                        string value = lVars[lVars.Count - 1].Name;
                        value = value.Substring(value.IndexOf("_") + 1,value.Length - 1);
                        int finalval = Int32.Parse(value);
                        stm2.Variable = "_" + (finalval + 1);
                    }
                    else
                        stm2.Variable = "_1";

                    statements.Add(stm2);
                    var = new VarDeclaration("int", stm2.Variable);
                    if (!lVars.Contains(var))
                        lVars.Add(var);
                }

                string newval = lVars[lVars.Count - 1].Name;
                newval = newval.Substring(newval.IndexOf("_") + 1,newval.Length - 1);
                int val = Int32.Parse(newval);
                string newstm = "";
                if (f1.Variable != null && f2.Variable != null && f1.Value != null && f2.Value != null)
                    newstm = "let " + s.Variable + " = (" + f1.Variable +
                             ((BinaryOperationExpression) (s.Value)).Operator + f2.Variable + ");";
                else if (f1.Variable != null && f1.Value != null)
                    newstm = "let _" + (val + 1) + " = (" + f1.Variable + " " +
                             ((BinaryOperationExpression) (s.Value)).Operator + " " + lVars[lVars.Count - 1].Name + ");";
                else if (f2.Variable != null && f2.Value != null)
                    newstm = "let " + s.Variable + " = (" + lVars[lVars.Count - 1].Name + " " +
                             ((BinaryOperationExpression) (s.Value)).Operator + " " + f2.Variable + ");";
                else
                    newstm = "let _" + (val + 1) + " = (" + lVars[lVars.Count - 2].Name
                             + ((BinaryOperationExpression) (s.Value)).Operator + lVars[lVars.Count - 1].Name + ");";
                List<string> final = new List<string>();
                final.Add(newstm);
                List<Token> tokens = Tokenize(final);
                LetStatement letstm = new LetStatement();
                TokensStack stack = new TokensStack(tokens);
                letstm.Parse(stack);
                var = new VarDeclaration("int", letstm.Variable);
                if (!lVars.Contains(var))
                    lVars.Add(var);
                statements.Add(letstm);
            }
            else
            {
                LetStatement stm = new LetStatement();
                stm.Variable = s.Variable;
                stm.Value = s.Value;
                statements.Add(stm);
            }
            //return null;
            return statements;
        }
        public List<LetStatement> SimplifyExpressions(List<LetStatement> ls, List<VarDeclaration> lVars)
        {
            List<LetStatement> lSimplified = new List<LetStatement>();
            foreach (LetStatement s in ls)
                lSimplified.AddRange(SimplifyExpressions(s, lVars));
            return lSimplified;
        }

 
        public LetStatement ParseStatement(List<Token> lTokens)
        {
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            LetStatement s = new LetStatement();
            s.Parse(sTokens);
            return s;
        }

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
        public List<Token> Tokenize(string sLine, int iLine)
        {
            //insert here code from assignment 3.1 to tokenize a single line
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
            if(sLine.Contains("//") || sLine == "\t")
                    return lTokens;
            string line = sLine;
            meaning = line;
            int finalPos = 0;
            while (line != "")
            {
                if (line == null)
                    break;
                if (line != "")
                    line = Next(line, deli, out meaning, out indexPos);
                //finalPos = lCodeLines[i].IndexOf(meaning) + lCodeLines[i].Count(ch => ch == '\t');
                if (meaning != " ")
                {
                    if (meaning.Contains("\t"))
                    {
                        meaning = meaning.Replace("\t", "");
                        indexPos = sLine.Count(ch => ch == '\t');
                        finalPos = indexPos;
                        indexPos = meaning.Length;
                    }
                        
                    if (Token.Statements.Contains(meaning))
                    { 
                        t = new Statement(meaning, iLine, finalPos);
                        lTokens.Add(t);
                    }
                    else if (Token.VarTypes.Contains(meaning))
                    {
                        t = new VarType(meaning, iLine, finalPos);
                        lTokens.Add(t);
                    }
                    else if (Token.Constants.Contains(meaning))
                    {
                        t = new Constant(meaning, iLine, finalPos);
                        lTokens.Add(t);
                    }
                    else if (meaning.Length == 1 && Token.Operators.Contains(char.Parse(meaning)))
                    {
                        t = new Operator(char.Parse(meaning), iLine, finalPos);
                        lTokens.Add(t);
                    }
                    else if (meaning.Length == 1 && Token.Parentheses.Contains(char.Parse(meaning)))
                    {
                        t = new Parentheses(char.Parse(meaning), iLine, finalPos);
                        lTokens.Add(t);
                    }
                    else if (meaning.Length == 1 && Token.Separators.Contains(char.Parse(meaning)))
                    {
                        t = new Separator(char.Parse(meaning), iLine, finalPos);
                        lTokens.Add(t); 
                    }
                    else if (Int32.TryParse(meaning, out num))
                    {
                        t = new Number(meaning, iLine, finalPos);
                        lTokens.Add(t); }
                    else if(meaning != "")//should improve to alphanumeric if its not working
                    { 
                        t = new Identifier(meaning, iLine, finalPos);
                        lTokens.Add(t);
                    } 
                }

                finalPos += indexPos;
            }
            
            return lTokens;
            //return null;
        }

        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();
            for (int i = 0; i < lCodeLines.Count; i++)
            {
                string sLine = lCodeLines[i];
                List<Token> lLineTokens = Tokenize(sLine, i);
                lTokens.AddRange(lLineTokens);
            }
            return lTokens;
        }

    }
}
