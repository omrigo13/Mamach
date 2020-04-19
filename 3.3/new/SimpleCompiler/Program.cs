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
        
        static List<string> did = new List<string>();
        static int depth = 0;
        static Random randnum = new Random();
        static int TestRuns = 0;

        static void Main(string[] args)
        {

            while(!Console.KeyAvailable)
            {
                Test();
               
            }
            Console.Clear();
            foreach (var item in did)
            {
                Console.WriteLine();
                Console.WriteLine(item);
                Console.WriteLine();
            }
            Console.ReadLine();
            

        }




        static void Test()
        {
            try
            {
                TestRuns++;
                int varNumber = randnum.Next(2, 20); //VAR_NUMBER
        
        
                Compiler c = new Compiler();
        
                List<string> lVars = new List<string>();
                for (int i = 1; i < varNumber + 1; i++)
                {
                    lVars.Add("var int x" + i + ";");
                }
        
                List<VarDeclaration> vars = c.ParseVarDeclarations(lVars);
                List<string> lAssignments = new List<string>();
                for (int i = 1; i < varNumber + 1; i++)
                {
                    string str = generateLet(i);
                    lAssignments.Add(str);
                    did.Add(str);
                }
        
                for (int i = 0; i < randnum.Next(0, 20); i++)   //MIXED_NUMBER
                {
                    string str = generateLet(randnum.Next(1, varNumber + 1), varNumber);
                    lAssignments.Add(str);
                    did.Add(str);
                }
        
        
                List<LetStatement> ls = c.ParseAssignments(lAssignments);
                Dictionary<string, int> dValues = new Dictionary<string, int>();
                for (int i = 1; i < varNumber + 1; i++)
                {
                    dValues["x" + i] = 0;
                }
        
        
                CPUEmulator cpu = new CPUEmulator();
                cpu.Compute(ls, dValues);
        
                List<LetStatement> lSimple = c.SimplifyExpressions(ls, vars);
        
                Dictionary<string, int> dValues2 = new Dictionary<string, int>();
                for (int i = 1; i < varNumber + 1; i++)
                {
                    dValues2["x" + i] = 0;
                }
        
        
        
                cpu.Compute(lSimple, dValues2);
        
                foreach (string sKey in dValues.Keys)
                    if (dValues[sKey] != dValues2[sKey])
                        throw new Exception("Test Failed! after simplyfing, the value of Variable " + sKey + "  has changed from " + dValues[sKey] + "  to " + dValues2[sKey]);
        
                List<string> lAssembly = c.GenerateCode(lSimple, vars);
        
                InitLCL(lAssembly);
                cpu.Code = lAssembly;
                cpu.Run(lAssembly.Count + 5, false);
                List<KeyValuePair<string, int>> asl = dValues.ToList();
                for (int i = 0; i < asl.Count; i++)
                {
                    string name = asl[i].Key;
                    int val = asl[i].Value;
                    if (dValues2[name] != val)
                        throw new Exception("Test Failed! after simplyfing, the value of Variable " + name + "  has changed from " + val + "  to " + dValues2[name]);
                    if (cpu.M[20 + i] != val)
                        throw new Exception("Test Failed! the generated assembly code has calculated the value of variable " + name + " to be " + cpu.M[20 + i] + " . the actoal value should be " + val);
                }
        
                Console.Write("done run number " + TestRuns);
                Console.CursorLeft = 22;
                Console.Write("with " + varNumber);
                Console.CursorLeft = 32;
                Console.Write("variables.    overall " + lAssignments.Count);
                Console.CursorLeft = 57;
                Console.WriteLine(" complex Let statments");
            }
            catch (Exception e)
            {
                int x = 1;
            }
        
        
        }
        
        
        
        
        static string generateLet(int num)
        {
            depth = 0;
            string ans = "let x" + num + " = " + randomBinary(num) + " ;";
            return ans;
        }
        
        static string generateLet(int num, int limit)
        {
            depth = 0;
            string ans = "let x" + num + " = " + randomBinary(num, limit) + " ;";
            return ans;
        }
        
        
        static string randomNum()
        {
        
            return randnum.Next(0, 20) + "";      //CONSTANT_NUMBERS
        
        }
        static string randomVar(int num)
        {
        
            if (num < 2)
                return randomNum();
            return " x" + randnum.Next(1, num) + " ";
        }
        
        static string randomVar(int num, int limit)
        {
        
            if (limit < 2)
                return randomNum();
            int next = randnum.Next(1, limit + 1);
            while (next == num)
                next = randnum.Next(1, limit + 1);
            return " x" + next + " ";
        }
        static string randomOp()
        {
        
            if (randnum.Next(1, 3) == 1)
                return " + ";
            return " - ";
        }
        static string randomBinary(int num, int limit)
        {
            if (depth > 30) //COMPLEX_DEPTH
            {
                return " 0 ";
        
            }
            depth++;
         
            string ans = " ( ";
        
            switch (randnum.Next(1, 4))//COMPLEXITY
            {
                case (1): ans += randomNum(); break;
                case (2): ans += randomVar(num, limit); break;
                case (3): ans += randomBinary(num, limit); break;
                case (4): ans += randomBinary(num, limit); break;
            }
          
            ans += randomOp();
        
            switch (randnum.Next(1, 4))//COMPLEXITY
            {
                case (1): ans += randomNum(); break;
                case (2): ans += randomVar(num, limit); break;
                case (3): ans += randomBinary(num, limit); break;
                case (4): ans += randomBinary(num, limit); break;
            }
        
            ans += " ) ";
            return ans;
        
        }
        
        static string randomBinary(int num)
        {
            if (depth > 30)//COMPLEX_DEPTH
            {
                return " 0 ";
        
            }
            depth++;
          
            string ans = " ( ";
        
            switch (randnum.Next(1, 4)) //COMPLEXITY
            {
                case (1): ans += randomNum(); break;
                case (2): ans += randomVar(num); break;
                case (3): ans += randomBinary(num); break;
                case (4): ans += randomBinary(num); break;
            }
        
            ans += randomOp();
        
            switch (randnum.Next(1, 4))//COMPLEXITY
            {
                case (1): ans += randomNum(); break;
                case (2): ans += randomVar(num); break;
                case (3): ans += randomBinary(num); break;
                case (4): ans += randomBinary(num); break;
            }
        
            ans += " ) ";
            return ans;
        
        }
    
    
    }
}
