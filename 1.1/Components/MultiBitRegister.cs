using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents an n bit register that can maintain an n bit number
    class MultiBitRegister : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        //Word size - number of bits in the register
        public int Size { get; private set; }

        private SingleBitRegister[] SBRegisterArr;

        public MultiBitRegister(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            Load = new Wire();
            SBRegisterArr = new SingleBitRegister[Size];

            //init the array and insert the inputs of the wireset to it  and updates the output of every bit
            for(int i = 0; i < Size; i++)
            {
                SBRegisterArr[i] = new SingleBitRegister();
                SBRegisterArr[i].ConnectLoad(Load);
                SBRegisterArr[i].ConnectInput(Input[i]);
                Output[i].ConnectInput(SBRegisterArr[i].Output);
            }
        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        
        public override string ToString()
        {
            return Output.ToString();
        }


        public override bool TestGate()
        {
            //throw new NotImplementedException();
/*            Input.SetValue(10);
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output[0].Value != 0)
                return false;*/
            //input value = 1, Load = 1
            Input.SetValue(1);
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.GetValue() != 1)
                return false;
            //input value = 0, Load = 1
            Input.SetValue(0);
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.GetValue() != 0)
                return false;
            //input value = 0, Load = 0
            Load.Value = 0;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.GetValue() != 0)
                return false;
            return true;
        }
    }
}
