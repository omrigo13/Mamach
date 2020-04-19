using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)
    class MultiBitAndGate : MultiBitGate
    {
        //we will use AndGate array of AndGates
        private AndGate[] m_gAndArr;

        public MultiBitAndGate(int iInputCount)
            : base(iInputCount)
        {
            //init the array and insert first 2 inputs of the wireset to it
            m_gAndArr = new AndGate[iInputCount-1];
            m_gAndArr[0] = new AndGate();
            m_gAndArr[0].ConnectInput1(m_wsInput[0]);
            m_gAndArr[0].ConnectInput2(m_wsInput[1]);
            //runs over the rest of the inputs and updates the output between every 2 close inputs using AndGate
            for(int i = 1; i < m_wsInput.Size-1; i++)
            {
                m_gAndArr[i] = new AndGate();
                m_gAndArr[i].ConnectInput1(m_gAndArr[i-1].Output);
                m_gAndArr[i].ConnectInput2(m_wsInput[i+1]);
            }
            //set the output of the MultiBitAndGate gate
            Output = m_gAndArr[iInputCount-2].Output;
        }


        public override bool TestGate()
        {
            //throw new NotImplementedException();

            //tests combine MultiAnd Gate of zeros and ones bites
            for (int i = 0; i < m_wsInput.Size; i++)
                m_wsInput[i].Value = 0;
            for (int i = 1; i < m_wsInput.Size; i += 2)
                m_wsInput[i].Value = 1;
            if (Output.Value != 0)
                return false;

            //tests combine MultiAnd Gate of zeros and ones bites
            for (int i = 0; i < m_wsInput.Size; i++)
                m_wsInput[i].Value = 1;
            for (int i = 1; i < m_wsInput.Size; i += 2)
                m_wsInput[i].Value = 0;
            if (Output.Value != 0)
                return false;

            //tests MultiAnd Gate of zero bites
            for (int i = 0; i < m_wsInput.Size; i++)
                m_wsInput[i].Value = 0;
            if(Output.Value != 0)
                return false;

            //tests MultiAnd Gate of one bites
            for (int i = 0; i < m_wsInput.Size; i++)
                m_wsInput[i].Value = 1;
            if (Output.Value != 1)
                return false;

            //tests MultiAnd Gate of zero bites and the first bite is one
            for (int i = 0; i < m_wsInput.Size; i++)
                m_wsInput[i].Value = 0;
            m_wsInput[0].Value = 1;
            if (Output.Value != 0)
                return false;

            //tests MultiAnd Gate of one bites and the first bite is zero
            for (int i = 0; i < m_wsInput.Size; i++)
                m_wsInput[i].Value = 1;
            m_wsInput[0].Value = 0;
            if (Output.Value != 0)
                return false;
            return true;
        }
    }
}
