using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a FullAdder, taking as input 3 bits - 2 numbers and a carry, and computing the result in the output, and the carry out.
    class FullAdder : TwoInputGate
    {
        public Wire CarryInput { get; private set; }
        public Wire CarryOutput { get; private set; }

        private HalfAdder m_gHalfAdder;
        private HalfAdder m_gHalfAdder2;
        private OrGate m_gOr;


        public FullAdder()
        {
            CarryInput = new Wire();

            //init the gates
            m_gHalfAdder = new HalfAdder();
            m_gHalfAdder2 = new HalfAdder();
            m_gOr = new OrGate();
            //wire HA between X and Cin
            m_gHalfAdder.ConnectInput1(Input1);
            m_gHalfAdder.ConnectInput2(CarryInput);
            //wire HA between Sum of first HA and Y
            m_gHalfAdder2.ConnectInput1(m_gHalfAdder.Output);
            m_gHalfAdder2.ConnectInput2(Input2);
            //wire OrGate between the carry out of the 2 half adders
            m_gOr.ConnectInput1(m_gHalfAdder.CarryOutput);
            m_gOr.ConnectInput2(m_gHalfAdder2.CarryOutput);

            //sum is the output of half adder number 2 and carry out is the output from the OrGate
            Output = m_gHalfAdder2.Output;
            CarryOutput = m_gOr.Output;

        }


        public override string ToString()
        {
            return Input1.Value + "+" + Input2.Value + " (C" + CarryInput.Value + ") = " + Output.Value + " (C" + CarryOutput.Value + ")";
        }

        public override bool TestGate()
        {
            //throw new NotImplementedException();

            CarryInput.Value = 0;
            //x = 0, y = 0, Cin = 0
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0 || CarryOutput.Value != 0)
                return false;
            //x = 0, y = 1, Cin = 0
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1 || CarryOutput.Value != 0)
                return false;
            //x = 1, y = 0, Cin = 0
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1 || CarryOutput.Value != 0)
                return false;
            //x = 1, y = 1, Cin = 0
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 0 || CarryOutput.Value != 1)
                return false;

            CarryInput.Value = 1;
            //x = 0, y = 0, Cin = 1
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 1 || CarryOutput.Value != 0)
                return false;
            //x = 0, y = 1, Cin = 1
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 0 || CarryOutput.Value != 1)
                return false;
            //x = 1, y = 0, Cin = 1
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 0 || CarryOutput.Value != 1)
                return false;
            //x = 1, y = 1, Cin = 1
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 1 || CarryOutput.Value != 1)
                return false;
            return true;
        }
    }
}
