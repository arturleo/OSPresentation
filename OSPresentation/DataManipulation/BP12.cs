using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    // TODO file animation
    public class BP12 : BreakPoint
    {
        #region Contructor

        public BP12(string bpo, int bpn) : base(bpo, bpn)
        {
            if (!String.IsNullOrEmpty(Regex.Match(addresses[0], @"(.*?)\s<").Groups[1].Value))
                startAddress = int.Parse(Regex.Match(addresses[0], @"(.*?)\s<").Groups[1].Value, NumberStyles.HexNumber);
            else
            {
                Trace.WriteLine("BreakPoint" + bpn + " StartAddress Parsing Error!");
            }

            Stacks = new List<StackData>();
            Stacks.Add(new StackData(startAddress + 20, "0x0000001f", "eax"));
            for (int i=4; i>=0; i--)
            {
                StackData sd = new StackData(startAddress + i*4, stks[i], registers[4-i]);
                Stacks.Add(sd);
            }
        }
        #endregion
        #region Field
        int startAddress = -1;
        List<string> registers = new List<string> { "ecx", "edx", "ds", "es", "fs" };
        #endregion
        #region Properties
        public string Function
        {
            get
            {
                return Regex.Match(addresses[0], @"<(.*?)>:").Groups[1].Value;
            }
        }
        public string Syscall
        {
            get => "floppy_interrupt()";
        }
        public List<StackData> Stacks
        {
            set;get;
        }
        override public string Description
        {
            get
            {
                return "Floppy interrupt, the current floppy request is completed.\n" +
                    "Pushing registers to save data for future restoration.\n"+
                    "Moving the previous defined 'do_floppy' pointer to register eax.\n" +
                    "Then it calls the fucntion that $eax points to.\n" ;
            }
        }
            #endregion
        #region Methods
        #endregion
    }

}
