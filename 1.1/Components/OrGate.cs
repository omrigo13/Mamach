using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This gate implements the or operation. To implement it, follow the example in the And gate.
    class OrGate : TwoInputGate
    {
        //we will use a not, another not and nand after it
        private NAndGate m_gNand;
        private NotGate m_gNot;
        private NotGate m_gNot2;

        public OrGate()
        {
            //init the gates
            m_gNand = new NAndGate();
            m_gNot = new NotGate();
            m_gNot2 = new NotGate();
            //wire the output of the not gates to nand gate
            m_gNand.ConnectInput1(m_gNot.Output);
            m_gNand.ConnectInput2(m_gNot2.Output);
            //set the inputs and the output of the or gate
            Output = m_gNand.Output;
            Input1 = m_gNot.Input;
            Input2 = m_gNot2.Input;
        }


        public override string ToString()
        {
            return "Or " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }

        public override bool TestGate()
        {
            //throw new NotImplementedException();
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            return true;
        }
    }

}
