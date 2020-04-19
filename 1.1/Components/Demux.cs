using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A demux has 2 outputs. There is a single input and a control bit, selecting whether the input should be directed to the first or second output.
    class Demux : Gate
    {
        public Wire Output1 { get; private set; }
        public Wire Output2 { get; private set; }
        public Wire Input { get; private set; }
        public Wire Control { get; private set; }

        private AndGate m_gAnd;
        private AndGate m_gAnd2;
        private NotGate m_gNot;

        public Demux()
        {
            Input = new Wire();
            Control = new Wire();
            Output1 = new Wire();
            Output2 = new Wire();

            //init the gates
            m_gAnd = new AndGate();
            m_gAnd2 = new AndGate();
            m_gNot = new NotGate();
            //wire not to c bit and And Gate with input
            m_gNot.ConnectInput(Control);
            m_gAnd.ConnectInput1(Input);
            m_gAnd.ConnectInput2(m_gNot.Output);
            //wire c bit and And Gate with input
            m_gAnd2.ConnectInput1(Input);
            m_gAnd2.ConnectInput2(m_gNot.Input);

            Output1 = m_gAnd.Output;
            Output2 = m_gAnd2.Output;
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }



        public override bool TestGate()
        {
            //throw new NotImplementedException();
            //c bit = 0, x = 0
            Input.Value = 0;
            Control.Value = 0;
            if (Output1.Value != 0 || Output2.Value != 0)
                return false;
            //c bit = 1, x = 1
            Input.Value = 1;
            Control.Value = 1;
            if (Output1.Value != 0 || Output2.Value != 1)
                return false;
            //c bit = 1, x = 0
            Input.Value = 0;
            Control.Value = 1;
            if (Output1.Value != 0 || Output2.Value != 0)
                return false;
            //c bit = 0, x = 1
            Input.Value = 1;
            Control.Value = 0;
            if (Output1.Value != 1 || Output2.Value != 0)
                return false;
            return true;
        }
    }
}
