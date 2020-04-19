using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    public class CPU16 
    {
        //this "enum" defines the different control bits names
        public const int J3 = 0, J2 = 1, J1 = 2, D3 = 3, D2 = 4, D1 = 5, C6 = 6, C5 = 7, C4 = 8, C3 = 9, C2 = 10, C1 = 11, A = 12, X2 = 13, X1 = 14, Type = 15;

        public int Size { get; private set; }

        //CPU inputs
        public WireSet Instruction { get; private set; }
        public WireSet MemoryInput { get; private set; }
        public Wire Reset { get; private set; }

        //CPU outputs
        public WireSet MemoryOutput { get; private set; }
        public Wire MemoryWrite { get; private set; }
        public WireSet MemoryAddress { get; private set; }
        public WireSet InstructionAddress { get; private set; }

        //CPU components
        private ALU m_gALU;
        private Counter m_rPC;
        private MultiBitRegister m_rA, m_rD;
        private BitwiseMux m_gAMux, m_gMAMux;

        //here we initialize and connect all the components, as in Figure 5.9 in the book
        public CPU16()
        {
            Size =  16;

            Instruction = new WireSet(Size);
            MemoryInput = new WireSet(Size);
            MemoryOutput = new WireSet(Size);
            MemoryAddress = new WireSet(Size);
            InstructionAddress = new WireSet(Size);
            MemoryWrite = new Wire();
            Reset = new Wire();

            m_gALU = new ALU(Size);
            m_rPC = new Counter(Size);
            m_rA = new MultiBitRegister(Size);
            m_rD = new MultiBitRegister(Size);

            m_gAMux = new BitwiseMux(Size);
            m_gMAMux = new BitwiseMux(Size);

            m_gAMux.ConnectInput1(Instruction);
            m_gAMux.ConnectInput2(m_gALU.Output);

            m_rA.ConnectInput(m_gAMux.Output);

            m_gMAMux.ConnectInput1(m_rA.Output);
            m_gMAMux.ConnectInput2(MemoryInput);
            m_gALU.InputY.ConnectInput(m_gMAMux.Output);

            m_gALU.InputX.ConnectInput(m_rD.Output);

            m_rD.ConnectInput(m_gALU.Output);

            MemoryOutput.ConnectInput(m_gALU.Output);
            MemoryAddress.ConnectInput(m_rA.Output);

            InstructionAddress.ConnectInput(m_rPC.Output);
            m_rPC.ConnectInput(m_rA.Output);
            m_rPC.ConnectReset(Reset);

            //now, we call the code that creates the control unit
            ConnectControls();
        }

        //add here components to implement the control unit 
        private BitwiseMultiwayMux m_gJumpMux;//an example of a control unit compnent - a mux that controls whether a jump is made

        //5
        private AndGate registerDAndGate;

        //6
        private OrGate registerAOrGate;
        private NotGate registerANotGate;

        //7
        private AndGate MemoryWriteAndGate;

        //8
        private WireSet NoJump = new WireSet(1);

        private WireSet JGT = new WireSet(1);
        private AndGate JGTAndGate = new AndGate();

        private WireSet JEQ = new WireSet(1);
        private NotGate JEQNotGate = new NotGate();

        private WireSet JGE = new WireSet(1);

        private WireSet JLT = new WireSet(1);
        private NotGate JLTNotGate = new NotGate();

        private WireSet JNE = new WireSet(1);

        private WireSet JLE = new WireSet(1);
        private OrGate JLEOrGate = new OrGate();

        private WireSet JMP = new WireSet(1);

        private void ConnectControls()
        {
            //1. connect control of mux 1 (selects entrance to register A)
            m_gAMux.ConnectControl(Instruction[Type]);

            //2. connect control to mux 2 (selects A or M entrance to the ALU)
            m_gMAMux.ConnectControl(Instruction[A]);

            //3. consider all instruction bits only if C type instruction (MSB of instruction is 1)

            //4. connect ALU control bits
            m_gALU.ZeroX.ConnectInput(Instruction[C1]);
            m_gALU.NotX.ConnectInput(Instruction[C2]);
            m_gALU.ZeroY.ConnectInput(Instruction[C3]);
            m_gALU.NotY.ConnectInput(Instruction[C4]);
            m_gALU.F.ConnectInput(Instruction[C5]);
            m_gALU.NotOutput.ConnectInput(Instruction[C6]);

            //5. connect control to register D (very simple)
            registerDAndGate = new AndGate();
            registerDAndGate.ConnectInput1(Instruction[D2]);
            registerDAndGate.ConnectInput2(Instruction[Type]);
            m_rD.Load.ConnectInput(registerDAndGate.Output);

            //6. connect control to register A (a bit more complicated)
            registerAOrGate = new OrGate();
            registerANotGate = new NotGate();
            registerANotGate.ConnectInput(Instruction[Type]);
            registerAOrGate.ConnectInput1(registerANotGate.Output);
            registerAOrGate.ConnectInput2(Instruction[D1]);
            m_rA.Load.ConnectInput(registerAOrGate.Output);

            //7. connect control to MemoryWrite
            MemoryWriteAndGate = new AndGate();
            MemoryWriteAndGate.ConnectInput1(Instruction[Type]);
            MemoryWriteAndGate.ConnectInput2(Instruction[D3]);
            MemoryWrite.ConnectInput(MemoryWriteAndGate.Output);

            //8. create inputs for jump mux
            JEQNotGate.ConnectInput(m_gALU.Zero);
            JLTNotGate.ConnectInput(m_gALU.Negative);

            JGTAndGate.ConnectInput1(JEQNotGate.Output);
            JGTAndGate.ConnectInput2(JLTNotGate.Output);

            JLEOrGate.ConnectInput1(m_gALU.Zero);
            JLEOrGate.ConnectInput2(m_gALU.Negative);

            JGT[0].ConnectInput(JGTAndGate.Output);
            JEQ[0].ConnectInput(m_gALU.Zero);
            JGE[0].ConnectInput(JLTNotGate.Output);
            JLT[0].ConnectInput(m_gALU.Negative);
            JNE[0].ConnectInput(JEQNotGate.Output);
            JLE[0].ConnectInput(JLEOrGate.Output);

            //9. connect jump mux (this is the most complicated part)
            m_gJumpMux = new BitwiseMultiwayMux(1, 3);
            AndGate LoadAndGate1 = new AndGate();
            AndGate LoadAndGate2 = new AndGate();
            AndGate LoadAndGate3 = new AndGate();

            m_gJumpMux.ConnectInput(0, NoJump);
            m_gJumpMux.ConnectInput(1, JGT);
            m_gJumpMux.ConnectInput(2, JEQ);
            m_gJumpMux.ConnectInput(3, JGE);
            m_gJumpMux.ConnectInput(4, JLT);
            m_gJumpMux.ConnectInput(5, JNE);
            m_gJumpMux.ConnectInput(6, JLE);
            m_gJumpMux.ConnectInput(7, JMP);

            LoadAndGate1.ConnectInput1(Instruction[J3]);
            LoadAndGate1.ConnectInput2(Instruction[Type]);
            LoadAndGate2.ConnectInput1(Instruction[J2]);
            LoadAndGate2.ConnectInput2(Instruction[Type]);
            LoadAndGate3.ConnectInput1(Instruction[J1]);
            LoadAndGate3.ConnectInput2(Instruction[Type]);

            m_gJumpMux.Control[0].ConnectInput(LoadAndGate1.Output);
            m_gJumpMux.Control[1].ConnectInput(LoadAndGate2.Output);
            m_gJumpMux.Control[2].ConnectInput(LoadAndGate3.Output);

            NoJump[0].Value = 0;
            JMP[0].Value = 1;

            //10. connect PC load control
            m_rPC.Load.ConnectInput(m_gJumpMux.Output[0]);
        }

        public override string ToString()
        {
            return "A=" + m_rA + ", D=" + m_rD + ", PC=" + m_rPC + ",Ins=" + Instruction;
        }

        private string GetInstructionString()
        {
            if (Instruction[Type].Value == 0)
                return "@" + Instruction.GetValue();
            return Instruction[Type].Value + "XX " +
               "a" + Instruction[A] + " " +
               "c" + Instruction[C1] + Instruction[C2] + Instruction[C3] + Instruction[C4] + Instruction[C5] + Instruction[C6] + " " +
               "d" + Instruction[D1] + Instruction[D2] + Instruction[D3] + " " +
               "j" + Instruction[J1] + Instruction[J2] + Instruction[J3];
        }

        //use this function in debugging to print the current status of the ALU. Feel free to add more things for printing.
        public void PrintState()
        {
            Console.WriteLine("CPU state:");
            Console.WriteLine("PC=" + m_rPC + "=" + m_rPC.Output.GetValue());
            Console.WriteLine("A=" + m_rA + "=" + m_rA.Output.GetValue());
            Console.WriteLine("D=" + m_rD + "=" + m_rD.Output.GetValue());
            Console.WriteLine("Ins=" + GetInstructionString());
            Console.WriteLine("ALU=" + m_gALU);
            Console.WriteLine("inM=" + MemoryInput);
            Console.WriteLine("outM=" + MemoryOutput);
            Console.WriteLine("addM=" + MemoryAddress);
        }
    }
}
