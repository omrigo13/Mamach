using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents a set of n wires (a cable)
    class WireSet
    {
        //Word size - number of bits in the register
        public int Size { get; private set; }
        
        public bool InputConected { get; private set; }

        //An indexer providing access to a single wire in the wireset
        public Wire this[int i]
        {
            get
            {
                return m_aWires[i];
            }
        }
        private Wire[] m_aWires;
        
        public WireSet(int iSize)
        {
            Size = iSize;
            InputConected = false;
            m_aWires = new Wire[iSize];
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i] = new Wire();
        }
        public override string ToString()
        {
            string s = "[";
            for (int i = m_aWires.Length - 1; i >= 0; i--)
                s += m_aWires[i].Value;
            s += "]";
            return s;
        }

        //Transform a positive integer value into binary and set the wires accordingly, with 0 being the LSB
        public void SetValue(int iValue)
        {
            //throw new NotImplementedException();
            int count = 0;
            while (iValue >= 0 && count < m_aWires.Length)
            {
                m_aWires[count].Value = iValue % 2;
                iValue /= 2;
                count++;
            }
        }

        //Transform the binary code into a positive integer
        public int GetValue()
        {
            //throw new NotImplementedException();
            int decimal_number = 0;
            for (int i = 0; i < m_aWires.Length; i++)
                decimal_number += m_aWires[i].Value * ((int)Math.Pow(2,i));
            return decimal_number;
        }

        //Transform an integer value into binary using 2`s complement and set the wires accordingly, with 0 being the LSB
        public void Set2sComplement(int iValue)
        {
            //throw new NotImplementedException();

            //positive integer number
            if (iValue >= 0)
                SetValue(iValue);

            //negative integer number
            else
            {
                SetValue(iValue * -1);
                //making not to the binary number
                for (int i = 0; i < m_aWires.Length; i++)
                {
                    if (m_aWires[i].Value == 1)
                        m_aWires[i].Value = 0;
                    else
                        m_aWires[i].Value = 1;
                }
                int carry = 1, index = 0;
                //adding 1 to the binary number
                while (index < m_aWires.Length && carry == 1)
                {
                    if (m_aWires[index].Value == 0)
                    {
                        m_aWires[index].Value = 1;
                        carry = 0;
                    }
                    else
                        m_aWires[index].Value = 0;
                    index++;
                }
            }
        }

        //Transform the binary code in 2`s complement into an integer
        public int Get2sComplement()
        {
            //throw new NotImplementedException();

            //the binary number is positive
            if (m_aWires[m_aWires.Length - 1].Value == 0)
                return GetValue();

            //the binary number is negative
            int carry = 0, index = 0, neg_number;
            //subtractioning 1 to the binary number
            while (index < m_aWires.Length && carry == 0)
            {
                if (m_aWires[index].Value == 1)
                {
                    m_aWires[index].Value = 0;
                    carry = 1;
                }
                else
                    m_aWires[index].Value = 1;
                index++;
            }
            //making not to the binary number
            for (int i = 0; i < m_aWires.Length; i++)
            {
                if (m_aWires[i].Value == 1)
                    m_aWires[i].Value = 0;
                else
                    m_aWires[i].Value = 1;
            }
            neg_number = (GetValue() * -1);
            Set2sComplement(neg_number);
            return neg_number;
        }

        public void ConnectInput(WireSet wIn)
        {
            if (InputConected)
                throw new InvalidOperationException("Cannot connect a wire to more than one inputs");
            if(wIn.Size != Size)
                throw new InvalidOperationException("Cannot connect two wiresets of different sizes.");
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i].ConnectInput(wIn[i]);

            InputConected = true;
            
        }

    }
}
