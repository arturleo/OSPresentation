using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    public class BP10 : BreakPoint
    {
        #region Contructor

        public BP10(string bpo, int bpn) : base(bpo, bpn)
        {
            if (!String.IsNullOrEmpty(addresses[0]))
                startAddress = int.Parse(addresses[0], NumberStyles.HexNumber);
            else
            {
                Trace.WriteLine("BreakPoint" + bpn + " StartAddress Parsing Error!");
            }

            Stacks = new List<StackData>();
            for (int i=4; i>=0; i--)
            {
                StackData sd = new StackData(startAddress + i*4, stks[i], registers[4-i]);
                Stacks.Add(sd);
            }
        }
        #endregion
        #region Field
        int startAddress = -1;
        List<string> registers = new List<string> { "gs", "esi", "edi", "ebp", "eax" };
        #endregion
            #region Properties
        public List<StackData> Stacks
        {
            set;get;
        }
        override public string Description
        {
            get
            {
                return "Calling C function copy_process(), pushing gs, esi, edi, ebp, eax, as input paramaters to copy_process().\n";
            }
        }
            #endregion
        #region Methods
        #endregion
    }

}
