using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseMux : BitwiseTwoInputGate
    {
        public Wire ControlInput { get; private set; }

        private MuxGate[] m_gMuxArr;

        public BitwiseMux(int iSize)
            : base(iSize)
        {

            ControlInput = new Wire();

            m_gMuxArr = new MuxGate[Size];
            for (int i = 0; i < Size; i++)
            {
                m_gMuxArr[i] = new MuxGate();
                m_gMuxArr[i].ConnectControl(ControlInput);
                m_gMuxArr[i].ConnectInput1(Input1[i]);
                m_gMuxArr[i].ConnectInput2(Input2[i]);
                Output[i].ConnectInput(m_gMuxArr[i].Output);
            }

        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }



        public override string ToString()
        {
            return "Mux " + Input1 + "," + Input2 + ",C" + ControlInput.Value + " -> " + Output;
        }




        public override bool TestGate()
        {
            //throw new NotImplementedException();
            
            ControlInput.Value = 0;
            
            //c bit = 0, Xi = 0, Yi = 0
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 0;
                Input2[i].Value = 0;
                if (Output[i].Value != 0)
                    return false;
            }
            //c bit = 0, Xi = 1, Yi = 1
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 1;
                Input2[i].Value = 1;
                if (Output[i].Value != 1)
                    return false;
            }
            //c bit = 0, Xi = 1, Yi = 0
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 1;
                Input2[i].Value = 0;
                if (Output[i].Value != 1)
                    return false;
            }
            //c bit = 0, Xi = 0, Yi = 1
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 0;
                Input2[i].Value = 1;
                if (Output[i].Value != 0)
                    return false;
            }

            ControlInput.Value = 1;

            //c bit = 1, Xi = 0, Yi = 0
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 0;
                Input2[i].Value = 0;
                if (Output[i].Value != 0)
                    return false;
            }
            //c bit = 1, Xi = 1, Yi = 1
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 1;
                Input2[i].Value = 1;
                if (Output[i].Value != 1)
                    return false;
            }
            //c bit = 1, Xi = 1, Yi = 0
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 1;
                Input2[i].Value = 0;
                if (Output[i].Value != 0)
                    return false;
            }
            //c bit = 1, Xi = 0, Yi = 1
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = 0;
                Input2[i].Value = 1;
                if (Output[i].Value != 1)
                    return false;
            }
            return true;
        }
    }
}
