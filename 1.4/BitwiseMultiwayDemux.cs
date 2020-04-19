using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a demux with k outputs, each output with n wires. The input also has n wires.

    class BitwiseMultiwayDemux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; }

        public WireSet Input { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Outputs { get; private set; }

        private BitwiseDemux[] BWDeMuxArr;
        private WireSet[] WiresInBWDeMux;

        public BitwiseMultiwayDemux(int iSize, int cControlBits)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Outputs = new WireSet[(int)Math.Pow(2, cControlBits)];
            for (int i = 0; i < Outputs.Length; i++)
                Outputs[i] = new WireSet(Size);

            //init BitWiseDeMux Array and Wireset Array of Wires Inputs in BitWiseDeMuxes
            BWDeMuxArr = new BitwiseDemux[Outputs.Length];
            WiresInBWDeMux = new WireSet[Outputs.Length * 2];

            //init size of BitWiseDeMux Array and Array of Wires Inputs
            for (int i = 0; i < Outputs.Length; i++)
                BWDeMuxArr[i] = new BitwiseDemux(Size);
            for (int i = 0; i < Outputs.Length * 2; i++)
                WiresInBWDeMux[i] = new WireSet(Size);

            //wire first control and Inputs to the first BWDeMux and wire the BWDemux Outputs to the WiresInBWDeMux
            BWDeMuxArr[0].ConnectControl(Control[Control.Size - 1]);
            BWDeMuxArr[0].ConnectInput(Input);
            WiresInBWDeMux[0].ConnectInput(BWDeMuxArr[0].Output1);
            WiresInBWDeMux[1].ConnectInput(BWDeMuxArr[0].Output2);

            //running over the next Controls and update the BWDeMuxArr and then set the Outputs from the Wires
            int next = 1, location = 2, next_control = 1;
            while (next_control < Control.Size)
            {
                int pass_helper = (int)(Math.Pow(2, next_control));
                int position = 0;
                while (position < pass_helper)
                {
                    BWDeMuxArr[next].ConnectControl(Control[Control.Size - 1 - next_control]);
                    BWDeMuxArr[next].ConnectInput(WiresInBWDeMux[next - 1]);
                    WiresInBWDeMux[location].ConnectInput(BWDeMuxArr[next].Output1);
                    WiresInBWDeMux[location + 1].ConnectInput(BWDeMuxArr[next].Output2);
                    next++;
                    position++;
                    location += 2;
                }
                next_control++;
            }

            //connect all the outputs to the Output Wireset
            location = Outputs.Length - 2;
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i].ConnectInput(WiresInBWDeMux[location]);
                location++;
            }
        }





        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }


        public override bool TestGate()
        {
            //throw new NotImplementedException();

            //Submission System error tests
/*            for (int i = 0; i < ControlBits; i++)
                Control[i].Value = 0;
            Control[2].Value = 1;
            Input[0].Value = 0;
            Input[1].Value = 1;
            Input[2].Value = 1;
            Input[3].Value = 1;
            Console.WriteLine(Outputs[0][0].Value + " " + Outputs[0][1].Value + " " + Outputs[0][2].Value + " " + Outputs[0][3].Value);
*/
            //Ci bits = 0, X0 = 0
            for ( int i = 0; i < ControlBits; i++)
                Control[i].Value = 0;
            for (int i = 0; i < Size; i++)
                Input[i].Value = 0;
            for (int i = 0; i < Size; i++)
                if (Outputs[0][i].Value != 0)
                    return false;
            //Ci bits = 0, X0 = 1
            for (int i = 0; i < Size; i++)
                Input[i].Value = 1;
            for (int i = 0; i < Size; i++)
                if (Outputs[0][i].Value != 1)
                    return false;
            //Ci bits = 1, X0 = 0
            for (int i = 0; i < ControlBits; i++)
                Control[i].Value = 1;
            for (int i = 0; i < Size; i++)
                Input[i].Value = 0;
            for (int i = 0; i < Size; i++)
                if (Outputs[0][i].Value != 0)
                    return false;
            //Ci bits = 1, X0 = 1
            for (int i = 0; i < Size; i++)
                Input[i].Value = 0;
            for (int i = 0; i < Size; i++)
                if (Outputs[0][i].Value != 0)
                    return false;
            return true;
        }
    }
}
