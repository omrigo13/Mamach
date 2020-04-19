using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a HalfAdder, taking as input 2 bits - 2 numbers and computing the result in the output, and the carry out.

    class HalfAdder : TwoInputGate
    {
        public Wire CarryOutput { get; private set; }

        private AndGate m_gAnd;
        private XorGate m_gXor;


        public HalfAdder()
        {
            //init the gates
            m_gAnd = new AndGate();
            m_gXor = new XorGate();
            //wire and between the 2 Inputs
            m_gAnd.ConnectInput1(Input1);
            m_gAnd.ConnectInput2(Input2);
            //wire xor between the 2 Inputs
            m_gXor.ConnectInput1(Input1);
            m_gXor.ConnectInput2(Input2);

            //sum output is the output of the xor gate and carry output is the output of the and gate
            Output = m_gXor.Output;
            CarryOutput = m_gAnd.Output;

        }


        public override string ToString()
        {
            return "HA " + Input1.Value + "," + Input2.Value + " -> " + Output.Value + " (C" + CarryOutput + ")";
        }

        public override bool TestGate()
        {
            //throw new NotImplementedException();

            //x = 0, y = 0
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0 || CarryOutput.Value != 0)
                return false;
            //x = 0, y = 1
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1 || CarryOutput.Value != 0)
                return false;
            //x = 1, y = 0
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1 || CarryOutput.Value != 0)
                return false;
            //x = 1, y = 1
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 0 || CarryOutput.Value != 1)
                return false;
            return true;
        }
    }
}
