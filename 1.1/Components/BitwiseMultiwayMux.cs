using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a mux with k input, each input with n wires. The output also has n wires.

    class BitwiseMultiwayMux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; }

        public WireSet Output { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Inputs { get; private set; }

        private BitwiseMux[] BWMuxArr;
        private WireSet[] WiresInBWMux;

        public BitwiseMultiwayMux(int iSize, int cControlBits)
        {
            Size = iSize;
            Output = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Inputs = new WireSet[(int)Math.Pow(2, cControlBits)];

            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i] = new WireSet(Size);

            }

            //init BitWiseMux Array and Wireset Array of Wires Inputs in BitWiseMuxes
            BWMuxArr = new BitwiseMux[Inputs.Length];
            WiresInBWMux = new WireSet[Inputs.Length];

            //init size of BitWiseMux Array and Array of Wires Inputs
            for (int i = 0; i < Inputs.Length; i++)
            {
                BWMuxArr[i] = new BitwiseMux(Size);
                WiresInBWMux[i] = new WireSet(Size);
            }

            //running over first Control and connect BitWiseMuxes into the Array
            int index = 0, jump = 0;
            while (index < (Inputs.Length / 2))
            {
                BWMuxArr[index].ConnectControl(Control[0]);
                BWMuxArr[index].ConnectInput1(Inputs[jump]);
                BWMuxArr[index].ConnectInput2(Inputs[jump + 1]);
                WiresInBWMux[index].ConnectInput(BWMuxArr[index].Output);
                index++;
                jump += 2;
            }

            //running over the next Controls and update the BWMuxArr and then set the Output from the Wires
            int next = Inputs.Length / 2, location = 0, next_control = 1;
            while (next_control < Control.Size)
            {
                int pass_helper = (int)(Math.Pow(2, Control.Size - 1 - next_control));
                int position = 0;
                while (position < pass_helper)
                {
                    BWMuxArr[next].ConnectControl(Control[next_control]);
                    BWMuxArr[next].ConnectInput1(WiresInBWMux[location]);
                    BWMuxArr[next].ConnectInput2(WiresInBWMux[location + 1]);
                    WiresInBWMux[next].ConnectInput(BWMuxArr[next].Output);
                    next++;
                    position++;
                    location += 2;
                }
                next_control++;
            }
            Output.ConnectInput(WiresInBWMux[next - 1]);
        }


        public void ConnectInput(int i, WireSet wsInput)
        {
            Inputs[i].ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }



        public override bool TestGate()
        {
            //throw new NotImplementedException();

            //Ci bits = 0, X0 = 0
            for (int i = 0; i < ControlBits; i++)
                Control[i].Value = 0;
            for (int i = 0; i < Size; i++)
                Inputs[0][i].Value = 0;
            for (int i = 0; i < Size; i++)
                if (Output[i].Value != 0)
                    return false;
            //Ci bits = 0, X0 = 1
            for (int i = 0; i < Size; i++)
                Inputs[0][i].Value = 1;
            for (int i = 0; i < Size; i++)
                if (Output[i].Value != 1)
                    return false;
            //Ci bits = 1, X0 = 0
            for (int i = 0; i < ControlBits; i++)
                Control[i].Value = 1;
            for (int i = 0; i < Size; i++)
                Inputs[0][i].Value = 0;
            for (int i = 0; i < Size; i++)
                if (Output[i].Value != 0)
                    return false;
            //Ci bits = 1, X0 = 1
            for (int i = 0; i < Size; i++)
                Inputs[0][i].Value = 1;
            for (int i = 0; i < Size; i++)
                if (Output[i].Value != 1)
                    return false;
            return true;
        }
    }
}
