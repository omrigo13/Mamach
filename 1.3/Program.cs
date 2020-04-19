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

/*            WireSet omri = new WireSet(4);
            omri[0].Value = 1;
            omri[1].Value = 1;
            omri[2].Value = 0;
            omri[3].Value = 1;
            omri.SetValue(10);
            Console.WriteLine(omri.Get2sComplement());
            Console.WriteLine(omri.GetValue());
            omri.Set2sComplement(-4);*/

            //Create a gate
            MuxGate mux = new MuxGate();
            //Test that the unit testing works properly
            if (!mux.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            Demux demux = new Demux();
            //Test that the unit testing works properly
            if (!demux.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            BitwiseAndGate BWAndGate = new BitwiseAndGate(10);
            //Test that the unit testing works properly
            if (!BWAndGate.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            BitwiseNotGate BWNotGate = new BitwiseNotGate(10);
            //Test that the unit testing works properly
            if (!BWNotGate.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            BitwiseOrGate BWOrGate = new BitwiseOrGate(10);
            //Test that the unit testing works properly
            if (!BWOrGate.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            BitwiseMux BWMux = new BitwiseMux(6);
            //Test that the unit testing works properly
            if (!BWMux.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            BitwiseDemux BWDemux = new BitwiseDemux(10);
            //Test that the unit testing works properly
            if (!BWDemux.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            BitwiseMultiwayMux BWMultiMux = new BitwiseMultiwayMux(10,3);
            //Test that the unit testing works properly
            if (!BWMultiMux.TestGate())
                Console.WriteLine("bugbug");

/*            BitwiseMultiwayDemux BWMultiDemux2 = new BitwiseMultiwayDemux(4, 4);
            for (int i = 0; i < BWMultiDemux2.ControlBits; i++)
                BWMultiDemux2.Control[i].Value = 0;
            BWMultiDemux2.Input[0].Value = 0;
            BWMultiDemux2.Input[1].Value = 1;
            BWMultiDemux2.Input[2].Value = 1;
            BWMultiDemux2.Input[3].Value = 1;
            Console.WriteLine(BWMultiDemux2.Outputs[0][2].Value);*/

            //Create a gate
            BitwiseMultiwayDemux BWMultiDemux = new BitwiseMultiwayDemux(10, 4);
            //Test that the unit testing works properly
            if (!BWMultiDemux.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            HalfAdder half_adder = new HalfAdder();
            //Test that the unit testing works properly
            if (!half_adder.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            FullAdder full_adder = new FullAdder();
            //Test that the unit testing works properly
            if (!full_adder.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            MultiBitAdder MBAdder = new MultiBitAdder(5);
            //Test that the unit testing works properly
            if (!MBAdder.TestGate())
                Console.WriteLine("bugbug");

            //Create a gate
            ALU alu = new ALU(4);
            //Test that the unit testing works properly
            if (!alu.TestGate())
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
