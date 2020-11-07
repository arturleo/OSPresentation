using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    public class BP8 : BreakPoint
    {
        #region Contructor

        public BP8(string bpo, int bpn) : base(bpo, bpn)
        {
            if (!String.IsNullOrEmpty(addresses[1]))
                startAddress = int.Parse(addresses[1], NumberStyles.HexNumber);
            else
            {
                Trace.WriteLine("BreakPoint" + bpn + " StartAddress Parsing Error!");
            }
            Stacks = new List<StackData>();
            // now i know the pushed register is acutually a pointer
            for (int i = 2; i >= 1; i--)
            {
                StackData sd = new StackData(startAddress + i * 4, stks[i], registers[2 - i]);
                Stacks.Add(sd);
            }
        }
        #endregion
        #region Field
        int startAddress = -1;
        List<string> registers = new List<string> { "oldeip", "eax(EIP(%esp))" };
        #endregion
        #region Properties
        public int Addr{ get => startAddress; }
        public List<StackData> Stacks
        {
            set;get;
        }
        override public string Description
        {
            get
            {
                return "Jumped to 'sys_execve:', pushed previous EIP representing the returning address in sys_call.\nCalling C function do_execve(), pushing current EIP as return address, \nit will not actually work because do_execve() changes the kernel stack.";
            }
        }
            #endregion
        #region Methods
        #endregion
    }

}
