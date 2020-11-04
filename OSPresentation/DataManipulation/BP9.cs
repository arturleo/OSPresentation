using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    public class BP9 : BreakPoint
    {
        #region Contructor

        public BP9(string bpo, int bpn) : base(bpo, bpn)
        {
            Eax = int.Parse(Regex.Match(addresses[0],@"(.*?)\s<").Groups[1].Value, NumberStyles.HexNumber);
            if (!String.IsNullOrEmpty(addresses[1]))
                startAddress = int.Parse(addresses[1], NumberStyles.HexNumber);
            else
            {
                Trace.WriteLine("BreakPoint" + bpn + " StartAddress Parsing Error!");
            }

            Stacks = new List<StackData>();
            StackData sd = new StackData(startAddress, stks[1], "eip");
            Stacks.Add(sd);
        }
        #endregion
        #region Field
        int startAddress = -1;
        #endregion
        #region Properties
        public int Eax{set;get;}
        public List<StackData> Stacks
        {
            set;get;
        }
        override public string Description
        {
            get
            {
                return "Jumped to 'sys_fork:', pushed previous EIP representing the returning address in sys_call.\nTesting if $eax is negative. Eax: " +Eax+ "represents the new task number.";
            }
        }
            #endregion
        #region Methods
        #endregion
    }

}
