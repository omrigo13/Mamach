using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseDemux : Gate
    {
        public int Size { get; private set; }
        public WireSet Output1 { get; private set; }
        public WireSet Output2 { get; private set; }
        public WireSet Input { get; private set; }
        public Wire Control { get; private set; }

        private Demux[] m_gDemuxArr;

        public BitwiseDemux(int iSize)
        {
            Size = iSize;
            Control = new Wire();
            Input = new WireSet(Size);
            Output1 = new WireSet(Size);
            Output2 = new WireSet(Size);

            m_gDemuxArr = new Demux[Size];
            for (int i = 0; i < Size; i++)
            {
                m_gDemuxArr[i] = new Demux();
                m_gDemuxArr[i].ConnectControl(Control);
                m_gDemuxArr[i].ConnectInput(Input[i]);
                Output1[i].ConnectInput(m_gDemuxArr[i].Output1);
                Output2[i].ConnectInput(m_gDemuxArr[i].Output2);
            }
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public override bool TestGate()
        {
            //throw new NotImplementedException();

            Control.Value = 0;

            //c bit = 0, Xi = 0
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = 0;
                if (Output1[i].Value != 0 || Output2[i].Value != 0)
                    return false;
            }
            //c bit = 0, Xi = 1
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = 1;
                if (Output1[i].Value != 1 || Output2[i].Value != 0)
                    return false;
            }

            Control.Value = 1;

            //c bit = 1, Xi = 0
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = 0;
                if (Output1[i].Value != 0 || Output2[i].Value != 0)
                    return false;
            }
            //c bit = 1, Xi = 1
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = 1;
                if (Output1[i].Value != 0 || Output2[i].Value != 1)
                    return false;
            }
            return true;
        }
    }
}
