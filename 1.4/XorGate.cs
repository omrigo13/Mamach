using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This gate implements the xor operation. To implement it, follow the example in the And gate.
    class XorGate : TwoInputGate
    {
        //we will use a not and nand between 2 inputs, then or between 2 inputs with not and or between 2 inputs
        private NAndGate m_gNand;
        private NotGate m_gNot;
        private NotGate m_gNot2;
        private NotGate m_gNot3;
        private OrGate m_gOr;
        private OrGate m_gOr2;
        public XorGate()
        {
            //init the gates
            m_gNand = new NAndGate();
            m_gOr = new OrGate();
            m_gOr2 = new OrGate();
            m_gNot = new NotGate();
            m_gNot2 = new NotGate();
            m_gNot3 = new NotGate();
            //wire not to the 2 inputs and, orGate between them
            m_gNot.ConnectInput(m_gOr.Input1);
            m_gNot2.ConnectInput(m_gOr.Input2);
            m_gOr2.ConnectInput1(m_gNot.Output);
            m_gOr2.ConnectInput2(m_gNot2.Output);
            //wire nand to or between 2 inputs and 2 not inputs
            m_gNand.ConnectInput1(m_gOr.Output);
            m_gNand.ConnectInput2(m_gOr2.Output);
            //wire notGate to the answer we get
            m_gNot3.ConnectInput(m_gNand.Output);
            //set the inputs and the output of the xor gate
            Output = m_gNot3.Output;
            Input1 = m_gOr.Input1;
            Input2 = m_gOr.Input2;
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(xor)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Xor " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }


        //this method is used to test the gate. 
        //we simply check whether the truth table is properly implemented.
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
            if (Output.Value != 0)
                return false;
            return true;
        }
    }
}
