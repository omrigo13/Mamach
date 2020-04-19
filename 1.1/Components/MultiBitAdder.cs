using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements an adder, receving as input two n bit numbers, and outputing the sum of the two numbers
    class MultiBitAdder : Gate
    {
        //Word size - number of bits in each input
        public int Size { get; private set; }

        public WireSet Input1 { get; private set; }
        public WireSet Input2 { get; private set; }
        public WireSet Output { get; private set; }
        //An overflow bit for the summation computation
        public Wire Overflow { get; private set; }

        private FullAdder[] FullAdderArr;
        private HalfAdder HalfAdder;

        public MultiBitAdder(int iSize)
        {
            Size = iSize;
            Input1 = new WireSet(Size);
            Input2 = new WireSet(Size);
            Output = new WireSet(Size);
            Overflow = new Wire();
            FullAdderArr = new FullAdder[Size - 1];

            //init size of FullAdder Array
            for (int i = 0; i < FullAdderArr.Length; i++)
                FullAdderArr[i] = new FullAdder();

            //init first Half Adder
            HalfAdder = new HalfAdder();
            HalfAdder.ConnectInput1(Input1[0]);
            HalfAdder.ConnectInput2(Input2[0]);
            Output[0].ConnectInput(HalfAdder.Output);
            //Overflow.ConnectInput(HalfAdder.CarryOutput);

            //connect the first HalfAdder to the First Full Adder
            FullAdderArr[0].ConnectInput1(Input1[1]);
            FullAdderArr[0].ConnectInput2(Input2[1]);
            FullAdderArr[0].CarryInput.ConnectInput(HalfAdder.CarryOutput);
            Output[1].ConnectInput(FullAdderArr[0].Output);

            //run over the next Full Adders and connect them
            for(int i = 1; i < FullAdderArr.Length; i++)
            {
                FullAdderArr[i].ConnectInput1(Input1[i + 1]);
                FullAdderArr[i].ConnectInput2(Input2[i + 1]);
                FullAdderArr[i].CarryInput.ConnectInput(FullAdderArr[i - 1].CarryOutput);
                Output[i + 1].ConnectInput(FullAdderArr[i].Output);
            }
            Overflow.ConnectInput(FullAdderArr[Size - 2].CarryOutput);
        }

        public override string ToString()
        {
            return Input1 + "(" + Input1.Get2sComplement() + ")" + " + " + Input2 + "(" + Input2.Get2sComplement() + ")" + " = " + Output + "(" + Output.Get2sComplement() + ")";
        }

        public void ConnectInput1(WireSet wInput)
        {
            Input1.ConnectInput(wInput);
        }
        public void ConnectInput2(WireSet wInput)
        {
            Input2.ConnectInput(wInput);
        }


        public override bool TestGate()
        {
            //throw new NotImplementedException();

            //overflow = 1, Xi = 1, Yi = 0 except the last one
            for(int i = 0; i < Size; i++)
                Input1[i].Value = 1;
            Input2[0].Value = 1;
            for (int i = 0; i < Size; i++)
                if (Output[i].Value != 0)
                    return false;
            if (Overflow.Value != 1)
                return false;

            //overflow = 1, Xi = 1, Yi = 1 
            for (int i = 0; i < Size; i++)
            { 
                Input1[i].Value = 1;
                Input2[i].Value = 1;
            }
            for (int i = 1; i < Size; i++)
                if (Output[i].Value != 1)
                    return false;
            if (Output[0].Value != 0)
                return false;
            if (Overflow.Value != 1)
                return false;

            // x = 0, y = 0
            Input1.SetValue(0);
            Input2.SetValue(0);
            for(int i = 0; i < Size; i++)
                if (Output[i].Value != 0)
                    return false;
            if (Overflow.Value != 0)
                return false;
            // x = 0, y = 1
            Input1.SetValue(0);
            Input2.SetValue(1);
            if (Output[0].Value != 1)
                return false;
            for (int i = 1; i < Size; i++)
                if (Output[i].Value != 0)
                    return false;
            if (Overflow.Value != 0)
                return false;
            // x = 1, y = 0
            Input1.SetValue(1);
            Input2.SetValue(0);
            if (Output[0].Value != 1)
                return false;
            for (int i = 1; i < Size; i++)
                if (Output[i].Value != 0)
                    return false;
            if (Overflow.Value != 0)
                return false;
            // x = 1, y = 1
            Input1.SetValue(1);
            Input2.SetValue(1);
            if (Output[0].Value != 0)
                return false;
            if (Output[1].Value != 1)
                return false;
            for (int i = 2; i < Size; i++)
                if (Output[i].Value != 0)
                    return false;
            if (Overflow.Value != 0)
                return false;
            return true;
        }
    }
}
