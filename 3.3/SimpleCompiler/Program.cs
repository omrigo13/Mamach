using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class Program
    {

        static void InitLCL(List<string> lAssembly)
        {
            lAssembly.Insert(0, "@20");
            lAssembly.Insert(1, "D=A");
            lAssembly.Insert(2, "@LCL");
            lAssembly.Insert(3, "M=D");

        }
        static void Test1()
        {
            Compiler c = new Compiler();
            List<string> lVars = new List<string>();
            lVars.Add("var int x;");
            List<VarDeclaration> vars = c.ParseVarDeclarations(lVars);

            string s = "let x = 5;";
            List<Token> lTokens = c.Tokenize(s, 0);
            LetStatement assignment = c.ParseStatement(lTokens);
            if(assignment.ToString() != s)
                Console.WriteLine("BUGBUG");


            List<LetStatement> l = new List<LetStatement>();
            l.Add(assignment);
            List<string> lAssembly = c.GenerateCode(l, vars);
            CPUEmulator cpu = new CPUEmulator();
            InitLCL(lAssembly);
            cpu.Code = lAssembly;
            cpu.Run(1000, false);
            if (cpu.M[20] != 5)
                Console.WriteLine("BUGBUG");
        }

        static void Test2()
        {
            Compiler c = new Compiler();
            List<string> lVars = new List<string>();
            lVars.Add("var int x;");
            lVars.Add("var int y;");
            lVars.Add("var int z;");
            List<VarDeclaration> vars = c.ParseVarDeclarations(lVars);

            List<string> lAssignments = new List<string>();
            lAssignments.Add("let x = 10;");
            lAssignments.Add("let y = 15;");
            lAssignments.Add("let z = (x + y);");

            List<LetStatement> ls = c.ParseAssignments(lAssignments);


            List<string> lAssembly = c.GenerateCode(ls, vars);
            CPUEmulator cpu = new CPUEmulator();
            InitLCL(lAssembly);
            cpu.Code = lAssembly;
            cpu.Run(1000, false);
            if (cpu.M[22] != 25)
                Console.WriteLine("BUGBUG");
        }
        static void Test3()
        {
            Compiler c = new Compiler();
            List<string> lVars = new List<string>();
            lVars.Add("var int x;");
            lVars.Add("var int y;");
            lVars.Add("var int z;");
            List<VarDeclaration> vars = c.ParseVarDeclarations(lVars);

            string s = "let x = ((x + 5) + (y - z));";
            List<Token> lTokens = c.Tokenize(s,0);
            LetStatement assignment = c.ParseStatement(lTokens);

            List<LetStatement> lSimple = c.SimplifyExpressions(assignment, vars);
            List<string> lAssembly = c.GenerateCode(lSimple, vars);
            // for(int i = 0; i < lAssembly.Count; i++)
            //     Console.WriteLine(lAssembly[i]);
            CPUEmulator cpu = new CPUEmulator();
            InitLCL(lAssembly);
            cpu.Code = lAssembly;
            cpu.Run(1000, false);
            if (cpu.M[20] != 5)
                Console.WriteLine("BUGBUG");
        }

        static void Test4()
        {
            Compiler c = new Compiler();

            List<string> lVars = new List<string>();
            //lVars.Add("var int x0;");
            lVars.Add("var int x1;");
            lVars.Add("var int x2;");
            lVars.Add("var int x3;");
            lVars.Add("var int x4;");
            lVars.Add("var int x5;");
            // lVars.Add("var int x6;");
            // lVars.Add("var int x7;");
            List<VarDeclaration> vars = c.ParseVarDeclarations(lVars);


            List<string> lAssignments = new List<string>();
            // lAssignments.Add("let x1 = 1;");
            // lAssignments.Add("let x2 = 3;");
            // lAssignments.Add("let x3 = (((x1 + 1) - 4) + ((x2 + x1) - 2));");
            // lAssignments.Add("let x4 = ((x2 + x3) - (x2 -7));");
            // lAssignments.Add("let x5 = (1000 - ((x1 + (((((x2 + x3) - x4) + x1) - x2) + x3)) - ((x1 - x2) + x4)));");
            // lAssignments.Add("let x1 = 7;");
            // lAssignments.Add("let x2 = 5;");
            // lAssignments.Add("let x3 = x1;");
            // lAssignments.Add("let x4 = (x2 + x3);");
            // lAssignments.Add("let x2 = 3;");
            // lAssignments.Add("let x2 = 3;");
            // lAssignments.Add("let x0 = 1;");
            // lAssignments.Add("let x1 = 1;");
            // lAssignments.Add("let x2 = 3;");
            // lAssignments.Add("let x3 = 5;");
            // lAssignments.Add("let x4 = 7;");
            // lAssignments.Add("let x5 = 9;");
            // lAssignments.Add("let x6 = 11;");
            // lAssignments.Add("let x7 = 13;");
            // lAssignments.Add("let x0 = ((((12 - ((x7 + 70) - 78)) - ((x5 + (42 - 76)) - ((20 + 15) - (x4 - 49)))) - ((((x7 - 45) - (x6 + x2)) + ((45 - 86) + (x7 - x7))) - (((x7 + x4) + (x3 + x3)) - ((x3 + 31) + (x0 - x0))))) + ((x4 - (30 + x2)) + ((((x4 - 96) + (4 + x4)) - ((36 - 85) + (x1 - x3))) + ((x3 - (15 + x1)) - ((x3 - 94) + x3)))));");
            // lAssignments.Add("let x2 = (((75 + (((x5 - 49) - (x3 + x0)) + ((29 - x4) - (x5 + 94)))) - ((((x7 - x2) + (x0 - 88)) - ((x7 + 18) - (36 + 48))) - 7)) + (((((x7 - 20) - (x7 + 48)) + 17) + (((x6 + x4) - (x5 + 7)) + ((x4 + x6) + (x2 + 20)))) + ((((x4 + 99) + (x6 + 60)) - (x1 - 37)) + (((x0 - x3) - x2) - ((x7 - x3) + (x1 - x5))))));");
            // lAssignments.Add("let x3=((((((65 - 98) - 66) - ((15 - 14) + (37 - 46))) + (((29 - x0) - (x6 - x2)) + ((x1 + x0) - (14 - x0)))) + ((x2 + (45 - (x7 + 17))) - (((x2 + 36) + (64 + x2)) + ((84 - x3) - (97 - x2))))) - (25 + ((((39 - 20) + (56 + 6)) - (x0 + (5 - x3))) - (((37 - 20) + (x0 - x0)) + ((22 - x0) + (x4 + 10))))));");
            //lAssignments.Add("let x4=((((((x5 - 21) - (x7 - x2)) - ((x6 - 0) + (x4 + 10))) + (83 + ((55 + 52) - (x6 - 39)))) - ((((62 - x5) + (66 - x6)) + (62 + (x6 - x0))) - (((x5 - 51) - (55 + 88)) - 56))) + ((x2 - x5) + ((((x3 - x4) - x2) + ((29 + 19) - (76 - x4))) + (((x3 + x0) - (x7 + 54)) - ((76 + x6) + (x1 + 66))))));");
            //lAssignments.Add("let x5=((((((47 + x2) + (68 + 30)) + (36 + (x4 + x7))) - (((87 + x4) - (x1 + x5)) - ((x1 + x1) + (62 + 99)))) - (((x7 - 80) + ((46 + x0) - (x6 + x2))) - (((x3 + x4) + (38 - 0)) + ((56 - 35) + (74 + x1))))) - (((((85 + 83) - (68 + x5)) - ((x5 - x0) - (30 + x6))) - (((x3 - 34) - (88 + 49)) - ((x3 - 0) - (x6 + x1)))) - ((((54 - 56) + (30 - 51)) - ((45 - 81) + (x5 + x1))) + (((x4 + 83) + (x5 - x6)) + ((10 - x5) + (x6 + x6))))));");
            //lAssignments.Add("let x6=((((((53 + x0) - x7) + ((x2 - x1) + x7)) + 52) + ((((84 + x3) - (35 + 45)) - ((45 - 84) + x5)) - (((59 + x3) - (x5 - x4)) + ((52 - x1) - (78 + 76))))) + ((((74 + (x7 + 92)) + ((x5 - 2) - (50 - x4))) - 68) - ((((40 + 94) - (x5 + x5)) - ((55 - 44) + (72 - x5))) + (((x1 + 33) - (7 + x3)) + ((x3 - 9) - (40 - x4))))));");
            //lAssignments.Add("let x7=(((((71 + (47 - 56)) + ((58 + 64) + (60 + x5))) - (((x0 - x6) - (x3 - 16)) + (x7 + (x0 + x4)))) + ((((76 - x1) - x2) - ((x1 + 52) + 54)) + (((16 - x6) - (19 + x7)) - ((4 - 1) + (5 - x3))))) - (((((x7 + x4) - (81 + x2)) + ((6 - x2) + (82 - 59))) + (((50 + x0) - (6 + 13)) - (24 + (57 - x7)))) - ((((67 + 13) + (0 - 26)) + (23 + (88 - x4))) - (((60 - 19) + x5) - x0))));");

            
            List<LetStatement> ls = c.ParseAssignments(lAssignments);
            Dictionary<string, int> dValues = new Dictionary<string, int>();
            // dValues["x0"] = 0;
            dValues["x1"] = 0;
            dValues["x2"] = 0;
            dValues["x3"] = 0;
            dValues["x4"] = 0;
            dValues["x5"] = 0;
            // dValues["x6"] = 0;
            // dValues["x7"] = 0;

            CPUEmulator cpu = new CPUEmulator();
            cpu.Compute(ls, dValues);

            List<LetStatement> lSimple = c.SimplifyExpressions(ls, vars);

            Dictionary<string, int> dValues2 = new Dictionary<string, int>();
            dValues2["x1"] = 0;
            dValues2["x2"] = 0;
            dValues2["x3"] = 0;
            dValues2["x4"] = 0;
            dValues2["x5"] = 0;
            // dValues2["x0"] = 0;
            // dValues2["x1"] = 0;
            // dValues2["x2"] = 0;
            // dValues2["x3"] = 0;
            // dValues2["x4"] = 0;
            // dValues2["x5"] = 0;
            // dValues2["x6"] = 0;
            // dValues2["x7"] = 0;

            cpu.Compute(lSimple, dValues2);

            foreach (string sKey in dValues.Keys)
                if (dValues[sKey] != dValues2[sKey])
                    Console.WriteLine("BGUBGU");

            List<string> lAssembly = c.GenerateCode(lSimple, vars);

            InitLCL(lAssembly);
            cpu.Code = lAssembly;
            cpu.Run(1000, false);
            if (cpu.M[24] != dValues2["x5"])
                Console.WriteLine("BUGBUG");

        }




        static void Main(string[] args)
        {
            // Test1();
            // Test2();
            // Test3();
            Test4();
            //Test3();
            //TestParseAndErrors();
        }

 
     }
}
