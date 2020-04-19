using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a register that can maintain 1 bit.
    class SingleBitRegister : Gate
    {
        public Wire Input { get; private set; }
        public Wire Output { get; private set; }
        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        private MuxGate m_gMux;
        private DFlipFlopGate dff;

        public SingleBitRegister()
        {
            
            Input = new Wire();
            Load = new Wire();
            Output = new Wire();
            m_gMux = new MuxGate();
            dff = new DFlipFlopGate();

            //wire control bit and inputs to the mux and set dff input and register output
            m_gMux.ConnectControl(Load);
            m_gMux.ConnectInput1(dff.Output);
            m_gMux.ConnectInput2(Input);
            dff.ConnectInput(m_gMux.Output);
            Output.ConnectInput(dff.Output);

        }

        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }

      

        public void ConnectLoad(Wire wLoad)
        {
            Load.ConnectInput(wLoad);
        }


        public override bool TestGate()
        {
            //throw new NotImplementedException();
            //Input = 1, Load = 1
            Load.Value = 1;
            Input.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            //Input = 0, Load = 0
            Load.Value = 0;
            Input.Value = 0;
            if (Output.Value != 1)
                return false;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 1)
                return false;
            //Input = 0, Load = 1
            Load.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 0)
                return false;
            //Input = 1, Load = 1
            Input.Value = 1;
            if (Output.Value != 0)
                return false;
            Clock.ClockDown();
            Clock.ClockUp();
            if (Output.Value != 1)
                return false;
            return true;
        }
    }
}
