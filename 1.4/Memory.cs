using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a memory unit, containing k registers, each of size n bits.
    class Memory : SequentialGate
    {
        //The address size determines the number of registers
        public int AddressSize { get; private set; }
        //The word size determines the number of bits in each register
        public int WordSize { get; private set; }

        //Data input and output - a number with n bits
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        //The address of the active register
        public WireSet Address { get; private set; }
        //A bit setting the memory operation to read or write
        public Wire Load { get; private set; }

        private MultiBitRegister[] MBRegisterArr;
        private BitwiseMultiwayDemux BWMDemux;
        private BitwiseMultiwayMux BWMultiMux;

        public Memory(int iAddressSize, int iWordSize)
        {
            AddressSize = iAddressSize;
            WordSize = iWordSize;

            Input = new WireSet(WordSize);
            Output = new WireSet(WordSize);
            Address = new WireSet(AddressSize);
            Load = new Wire();
            WireSet LoadConnect = new WireSet(1);
            LoadConnect[0].ConnectInput(Load);
            MBRegisterArr = new MultiBitRegister[(int)Math.Pow(2,AddressSize)];
            BWMDemux = new BitwiseMultiwayDemux(1, AddressSize);
            BWMultiMux = new BitwiseMultiwayMux(WordSize, AddressSize);

            //wire MultiWayDemux input and control and MultiWayMux control, set the output of the Memory
            BWMDemux.ConnectInput(LoadConnect);
            BWMDemux.ConnectControl(Address);
            BWMultiMux.ConnectControl(Address);
            Output.ConnectInput(BWMultiMux.Output);

            //wire Inputs to Multi Bit Registers and wire the Load, Set the Mux Inputs
            for (int i = 0; i < MBRegisterArr.Length; i++)
            {
                MBRegisterArr[i] = new MultiBitRegister(WordSize);
                MBRegisterArr[i].ConnectInput(Input);
                MBRegisterArr[i].Load.ConnectInput(BWMDemux.Outputs[i][0]);
                BWMultiMux.Inputs[i].ConnectInput(MBRegisterArr[i].Output);

            }

        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectAddress(WireSet wsAddress)
        {
            Address.ConnectInput(wsAddress);
        }


        public override void OnClockUp()
        {
        }

        public override void OnClockDown()
        {
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override bool TestGate()
        {
            //throw new NotImplementedException();
            //address value = 0, Input Value = 0
            Address.SetValue(0);
            Input.SetValue(0);
            Load.Value = 0;
            for (int i = 0; i < WordSize; i++)
                if (Output[i].Value != 0)
                    return false;
            Clock.ClockDown();
            Clock.ClockUp();
            for (int i = 0; i < WordSize; i++)
                if (Output[i].Value != 0)
                    return false;
            //Input Value = 0, Load = 1
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            for (int i = 0; i < WordSize; i++)
                if (Output[i].Value != 0)
                    return false;
            //address value = 1, Input Value = 1
            Address.SetValue(1);
            Input.SetValue(1);
            //Input Value = 1, Load = 0
            Load.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            for (int i = 0; i < WordSize; i++)
                if (Output[i].Value != 0)
                    return false;
            //Input bits = 1, Load = 1
            for (int i = 0; i < WordSize; i++)
                Input[i].Value = 1;
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            for (int i = 0; i < WordSize; i++)
                if (Output[i].Value != 1)
                    return false;
            //set max value as input
            Input.SetValue((int)Math.Pow(2, WordSize) - 1);
            Load.Value = 0;
            Address.SetValue(0);
            Clock.ClockDown();
            Clock.ClockUp();
            for (int i = 0; i < WordSize; i++)
                if (Output[i].Value != 0)
                    return false;
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.GetValue() != (int)Math.Pow(2, WordSize) - 1)
                return false;
            Address.SetValue(1);
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.GetValue() != (int)Math.Pow(2, WordSize) - 1)
                return false;
            return true;
        }
    }
}
