using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class is used to implement the ALU
    //The specification can be found at https://b1391bd6-da3d-477d-8c01-38cdf774495a.filesusr.com/ugd/56440f_2e6113c60ec34ed0bc2035c9d1313066.pdf slides 48,49
    class ALU : Gate
    {
        //The word size = number of bit in the input and output
        public int Size { get; private set; }

        //Input and output n bit numbers
        public WireSet InputX { get; private set; }
        public WireSet InputY { get; private set; }
        public WireSet Output { get; private set; }

        //Control bit 
        public Wire ZeroX { get; private set; }
        public Wire ZeroY { get; private set; }
        public Wire NotX { get; private set; }
        public Wire NotY { get; private set; }
        public Wire F { get; private set; }
        public Wire NotOutput { get; private set; }

        //Bit outputs
        public Wire Zero { get; private set; }
        public Wire Negative { get; private set; }

        //ZeroX
        private BitwiseMux BWMuxZeroX;
        private WireSet ZeroXWireset;
        //ZeroY
        private BitwiseMux BWMuxZeroY;
        private WireSet ZeroYWireset;
        //NotX
        private BitwiseMux BWMuxNotX;
        private BitwiseNotGate BWNotXGate;
        //NotY
        private BitwiseMux BWMuxNotY;
        private BitwiseNotGate BWNotYGate;
        //F
        private BitwiseMux BWMuxF;
        private BitwiseAndGate BWFAndGate;
        private MultiBitAdder MBFAdder;
        //NotOutput
        private BitwiseMux BWMuxNotOutput;
        private BitwiseNotGate BWNotOutputGate;
        //Zero
        private MultiBitOrGate MBZeroOrGate;
        private NotGate ZeroNotGate;

        public ALU(int iSize)
        {
            Size = iSize;
            InputX = new WireSet(Size);
            InputY = new WireSet(Size);
            Output = new WireSet(Size);
            ZeroX = new Wire();
            ZeroY = new Wire();
            NotX = new Wire();
            NotY = new Wire();
            F = new Wire();
            NotOutput = new Wire();
            Negative = new Wire();            
            Zero = new Wire();


            //Create and connect all the internal components

            //create and connect ZeroX and ZeroY
            //ZeroX
            BWMuxZeroX = new BitwiseMux(Size);
            ZeroXWireset = new WireSet(Size);
            ZeroXWireset.SetValue(0);
            BWMuxZeroX.ConnectInput1(InputX);
            BWMuxZeroX.ConnectInput2(ZeroXWireset);
            BWMuxZeroX.ConnectControl(ZeroX);
            //ZeroY
            BWMuxZeroY = new BitwiseMux(Size);
            ZeroYWireset = new WireSet(Size);
            ZeroYWireset.SetValue(0);
            BWMuxZeroY.ConnectInput1(InputY);
            BWMuxZeroY.ConnectInput2(ZeroYWireset);
            BWMuxZeroY.ConnectControl(ZeroY);
            //create and connect NotX and NotY
            //NotX
            BWMuxNotX = new BitwiseMux(Size);
            BWNotXGate = new BitwiseNotGate(Size);
            BWNotXGate.ConnectInput(BWMuxZeroX.Output);
            BWMuxNotX.ConnectControl(NotX);
            BWMuxNotX.ConnectInput1(BWMuxZeroX.Output);
            BWMuxNotX.ConnectInput2(BWNotXGate.Output);
            //NotY
            BWMuxNotY = new BitwiseMux(Size);
            BWNotYGate = new BitwiseNotGate(Size);
            BWNotYGate.ConnectInput(BWMuxZeroY.Output);
            BWMuxNotY.ConnectControl(NotY);
            BWMuxNotY.ConnectInput1(BWMuxZeroY.Output);
            BWMuxNotY.ConnectInput2(BWNotYGate.Output);
            //create and connect the F
            BWFAndGate = new BitwiseAndGate(Size);
            MBFAdder = new MultiBitAdder(Size);
            BWMuxF = new BitwiseMux(Size);
            BWFAndGate.ConnectInput1(BWMuxNotX.Output);
            BWFAndGate.ConnectInput2(BWMuxNotY.Output);
            MBFAdder.ConnectInput1(BWMuxNotX.Output);
            MBFAdder.ConnectInput2(BWMuxNotY.Output);
            BWMuxF.ConnectInput1(BWFAndGate.Output);
            BWMuxF.ConnectInput2(MBFAdder.Output);
            BWMuxF.ConnectControl(F);
            //create and connect the NotOutput
            BWMuxNotOutput = new BitwiseMux(Size);
            BWNotOutputGate = new BitwiseNotGate(Size);
            BWNotOutputGate.ConnectInput(BWMuxF.Output);
            BWMuxNotOutput.ConnectInput1(BWMuxF.Output);
            BWMuxNotOutput.ConnectInput2(BWNotOutputGate.Output);
            BWMuxNotOutput.ConnectControl(NotOutput);
            //Create and connect the Zero
            MBZeroOrGate = new MultiBitOrGate(Size);
            ZeroNotGate = new NotGate();
            MBZeroOrGate.ConnectInput(Output);
            ZeroNotGate.ConnectInput(MBZeroOrGate.Output);
            Zero.ConnectInput(ZeroNotGate.Output);
            //Create and connect the Negative
            Negative.ConnectInput(Output[Size - 1]);

            Output.ConnectInput(BWMuxNotOutput.Output);
        }

        public override bool TestGate()
        {
            //throw new NotImplementedException();
            InputX.SetValue(0);
            InputY.SetValue(0);
            ZeroX.Value = 0;
            ZeroY.Value = 0;
            NotX.Value = 0;
            NotY.Value = 0;
            F.Value = 0;
            NotOutput.Value = 0;
            if (Output.GetValue() != 0)
                return false;
            InputX.SetValue(1);
            InputY.SetValue(1);
            ZeroX.Value = 0;
            ZeroY.Value = 0;
            NotX.Value = 0;
            NotY.Value = 0;
            F.Value = 0;
            NotOutput.Value = 0;
            if (Output.GetValue() != 1)
                return false;
            return true;
        }
    }
}
