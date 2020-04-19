using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A mux has 2 inputs. There is a single output and a control bit, selecting which of the 2 inpust should be directed to the output.
    class MuxGate : TwoInputGate
    {
        public Wire ControlInput { get; private set; }

        ////we will use a not on c bit and use and with input 2, also using c bit and with input 1 and then or between the 2 and gates
        private AndGate m_gAnd;
        private AndGate m_gAnd2;
        private NotGate m_gNot;
        private OrGate m_gOr;


        public MuxGate()
        {
            ControlInput = new Wire();

            //init the gates
            m_gAnd = new AndGate();
            m_gAnd2 = new AndGate();
            m_gNot = new NotGate();
            m_gOr = new OrGate();
            //wire not to c bit and And Gate with input 1
            m_gNot.ConnectInput(ControlInput);
            m_gAnd.ConnectInput1(Input1);
            m_gAnd.ConnectInput2(m_gNot.Output);
            //wire c bit and And Gate with input 2
            m_gAnd2.ConnectInput1(Input2);
            m_gAnd2.ConnectInput2(m_gNot.Input);
            //wire Or Gate between the 2 And Gates
            m_gOr.ConnectInput1(m_gAnd.Output);
            m_gOr.ConnectInput2(m_gAnd2.Output);

            Output = m_gOr.Output;
        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }


        public override string ToString()
        {
            return "Mux " + Input1.Value + "," + Input2.Value + ",C" + ControlInput.Value + " -> " + Output.Value;
        }



        public override bool TestGate()
        {
            //throw new NotImplementedException();
            //c bit = 0, x = 0, y = 0
            Input1.Value = 0;
            Input2.Value = 0;
            ControlInput.Value = 0;
            if (Output.Value != 0)
                return false;
            //c bit = 0, x = 1, y = 1
            Input1.Value = 1;
            Input2.Value = 1;
            ControlInput.Value = 0;
            if (Output.Value != 1)
                return false;
            //c bit = 0, x = 1, y = 0
            Input1.Value = 1;
            Input2.Value = 0;
            ControlInput.Value = 0;
            if (Output.Value != 1)
                return false;
            //c bit = 0, x = 0, y = 1
            Input1.Value = 0;
            Input2.Value = 1;
            ControlInput.Value = 0;
            if (Output.Value != 0)
                return false;
            //c bit = 1, x = 0, y = 0
            Input1.Value = 0;
            Input2.Value = 0;
            ControlInput.Value = 1;
            if (Output.Value != 0)
                return false;
            //c bit = 1, x = 1, y = 1
            Input1.Value = 1;
            Input2.Value = 1;
            ControlInput.Value = 1;
            if (Output.Value != 1)
                return false;
            //c bit = 1, x = 1, y = 0
            Input1.Value = 1;
            Input2.Value = 0;
            ControlInput.Value = 1;
            if (Output.Value != 0)
                return false;
            //c bit = 1, x = 0, y = 1
            Input1.Value = 0;
            Input2.Value = 1;
            ControlInput.Value = 1;
            if (Output.Value != 1)
                return false;
            return true;
        }
    }
}
