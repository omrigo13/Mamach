using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class Program
    {
        private const int NumberOFTrumpPicsL = 4;
        private const int NumberOFTrumpPicsW = 5;



        public static void Main(string[] args)
        {
            RunEladTests();
        }

        private static void RunEladTests()
        {
            bool success = true;
            Dictionary<int, string> errors = makeErrorDictonery();
            int[] okFiles = makeOkFiles();
            for (int testNumber = 1; testNumber < 40; testNumber++)
            {
                try
                {
                    Compiler sc = new Compiler();
                    List<string> lLines = sc.ReadFile(@"EladTests\Test" + testNumber+ ".Jack");
                    List<Token> lTokens = sc.Tokenize(lLines);
                    TokensStack sTokens = new TokensStack();
                    for (int i = lTokens.Count - 1; i >= 0; i--)
                        sTokens.Push(lTokens[i]);
                    JackProgram prog = new JackProgram();
                    prog.Parse(sTokens);
                    if (okFiles.Contains(testNumber))
                    {
                        Console.WriteLine("Test : " + testNumber + " succsefully done!");
                    }
                    else
                    {
                        Console.WriteLine("you have parsed a syntex error, this might be the problem : " + errors[testNumber]);
                        success = false;
                    }
                }
                catch (SyntaxErrorException exeption)
                {
                    Console.WriteLine("Test : " + testNumber + " succsefully done!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Test : " + testNumber + " have failed, an error have occured");
                    success = false;
                }
            }
            if (success & TestParse()) ShowWinnerPicture();
            else ShowloserPicture();
        }

        private static int[] makeOkFiles()
        {
            return new int[] { 1, 14, 25, 28};
        }

        public static void ShowloserPicture()
        {
            Random ran = new Random();
            string picturePath = @"EladTests\TrumpPics\fail" + (int)(ran.NextDouble()*NumberOFTrumpPicsL) + ".jpg";
            ProcessStartInfo startInfo = new ProcessStartInfo(picturePath);
            startInfo.Verb = "edit";
            Process.Start(startInfo);
            Console.WriteLine("TEST FAILED!! better luck next time");
            Console.ReadLine();
        }
        public static void ShowWinnerPicture()
        {
            Random ran = new Random();
            string picturePath = @"EladTests\TrumpPics\success" + (int)(ran.NextDouble() * NumberOFTrumpPicsW) + ".jpg";
            ProcessStartInfo startInfo = new ProcessStartInfo(picturePath);
            startInfo.Verb = "edit";
            Process.Start(startInfo);
            Console.WriteLine("WELL DONE!! you'r a WINNER!!");
            Console.ReadLine();
        }

        public static string GetName(Token t)
        {
            if (t is Identifier)
            {
                return ((Identifier)t).Name;
            }
            if (t is Keyword)
            {
                return ((Keyword)t).Name;
            }
            if (t is Symbol)
            {
                return ((Symbol)t).Name + "";
            }
            if (t is Number)
            {
                return ((Number)t).Value +"";
            }
            return "";
        }



        private static bool TestParse()
        {
            try
            {
                Compiler sc = new Compiler();
                List<string> lLines = sc.ReadFile(@"Program.Jack");
                List<Token> lTokens = sc.Tokenize(lLines);
                TokensStack sTokens = new TokensStack();
                for (int i = lTokens.Count - 1; i >= 0; i--)
                    sTokens.Push(lTokens[i]);
                JackProgram prog = new JackProgram();
                prog.Parse(sTokens);
                string sAfterParsing = prog.ToString();
                sAfterParsing = sAfterParsing.Replace(" ", "");
                sAfterParsing = sAfterParsing.Replace("\t", "");
                sAfterParsing = sAfterParsing.Replace("\n", "");
                sAfterParsing = sAfterParsing.ToLower();

                string sAllTokens = "";
                foreach (Token t in lTokens)
                    sAllTokens += GetName(t).ToLower();


                for (int i = 0; i < sAllTokens.Length; i++)
                    if(sAllTokens[i] != sAfterParsing[i])
                        return false;
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public  static Dictionary<int , string> makeErrorDictonery()
        {
            Dictionary<int, String> errorMap = new Dictionary<int, string>();
            errorMap.Add(1, "Should have worked!");
            errorMap.Add(2, "A global variable decleration was not closed with a ';' sign should check it in varDeclaration class");
            errorMap.Add(3, "A global variable decleration didn't contain any varType, should check it in varDeclaration class");
            errorMap.Add(4, "A global variable decleration didn't contain any identifier, should check it in varDeclaration class");
            errorMap.Add(5, "A Local variable decleration was not closed with a ';' sign should check it in varDeclaration class");
            errorMap.Add(6, "function should have '{' after the args, should check in the function class");
            errorMap.Add(7, "function should have '{' after the args, should check in the function class");
            errorMap.Add(8, "funtction should wrap the args with (), should check in the function Class");
            errorMap.Add(9, "funtction should wrap the args with (), should check in the function Class");
            errorMap.Add(10, "funtction should wrap the args with (), should check in the function Class");
            errorMap.Add(11, "let statment should end with ';', should check in the letStatment class");
            errorMap.Add(12, "let statment should have operation of type '=' only, should check in the letStatment class");
            errorMap.Add(13, "let statment should have an Identifier only as a left value , should check in the letStatment class");
            errorMap.Add(14, "should have worked!");
            errorMap.Add(15, "while statment should wrap the expression with (), should check the whileStatment class");
            errorMap.Add(16, "while statment should wrap the expression with (), should check the whileStatment class");
            errorMap.Add(17, "while statment should wrap the statments with {}, should check the whileStatment class");
            errorMap.Add(18, "while statment should wrap the statments with {}, should check the whileStatment class");
            errorMap.Add(19, "while statment should contain expression in it, should check the whileStatment class");
            errorMap.Add(20, "while statment should contain at least one statment in it, should check the whileStatment class");
            errorMap.Add(21, "if statment should end wrap it's expression with (), should check the ifStatment class");
            errorMap.Add(22, "if statment should end wrap it's expression with (), should check the ifStatment class");
            errorMap.Add(23, "if statment should end wrap it's statments with {}, should check the ifStatment class");
            errorMap.Add(24, "if statment should end wrap it's statments with {}, should check the ifStatment class");
            errorMap.Add(25, "this one should work fine!");
            errorMap.Add(26, "else statment should end wrap it's statments with {}, should check the ifStatment class");
            errorMap.Add(27, "else statment should end wrap it's statments with {}, should check the ifStatment class");
            errorMap.Add(28, "this one should work fine!");
            errorMap.Add(29, "binary opertion expression must include (), should check the BinaryOpertionExpression class");
            errorMap.Add(30, "binary opertion expression must include (), should check the BinaryOpertionExpression class");
            errorMap.Add(31, "binary opertion expression must include (), should check the BinaryOpertionExpression class");
            errorMap.Add(32, "binary opertion expression must include operator, should check the BinaryOpertionExpression class");
            errorMap.Add(33, "binary opertion expression must include first operand, should check the BinaryOpertionExpression class");
            errorMap.Add(34, "binary opertion expression must include second operand, should check the BinaryOpertionExpression class");
            errorMap.Add(35, "function call must wrap it's args with (), should check the FunctionCallExpression class");
            errorMap.Add(36, "function call must wrap it's args with (), should check the FunctionCallExpression class");
            errorMap.Add(37, "function call must include an argument after the ',', should check the FunctionCallExpression class");
            errorMap.Add(38, "function call must include an ',' between args, should check the FunctionCallExpression class");
            errorMap.Add(39, "Unary opertion expression must include an operand after the operator, should check the UnaryOpertionExpression class");

            return errorMap;
        }
    }
}
