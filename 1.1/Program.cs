using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is an example of a testing code that you should run for all the gates that you create

            //Create a gate
            AndGate and = new AndGate();
            //Test that the unit testing works properly
            if (!and.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            OrGate or = new OrGate();
            //Test that the unit testing works properly
            if (!or.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            XorGate xor = new XorGate();
            //Test that the unit testing works properly
            if (!xor.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            MultiBitAndGate multiAnd = new MultiBitAndGate(10);
            //Test that the unit testing works properly
            if (!multiAnd.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            MultiBitOrGate multiOr = new MultiBitOrGate(10);
            //Test that the unit testing works properly
            if (!multiOr.TestGate())
                Console.WriteLine("bugbug");

            //Now we ruin the nand gates that are used in all other gates. The gate should not work properly after this.
            NAndGate.Corrupt = true;
            if (and.TestGate())
                Console.WriteLine("bugbug");

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
