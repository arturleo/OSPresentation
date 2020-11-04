using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Globalization;

namespace OSPresentation.DataManipulation
{
    public class BP7 : BreakPoint
    {
        #region Contructor
        public BP7(string bpo, int bpn) : base(bpo, bpn)
        {
            if (!String.IsNullOrEmpty(addresses[0]))
                startAddress = int.Parse(addresses[0], NumberStyles.HexNumber);
            else
            {
                Trace.WriteLine("BP7 Start Address Parse Error!");
            }
            _callNumber = int.Parse(paras[0]);
            Stacks = new List<StackData>();
            // the 12th one is useless
            for(int i=10; i>=0; i--)
            {
                StackData sd = new StackData(startAddress + i*4, stks[i], registers[10-i]);
                Stacks.Add(sd);
            }
        }
        #endregion
        #region Field
        int startAddress = -1;
        int _callNumber=-1;
        List<string> registers = new List<string> { "oldss", "oldesp", "eflags", "cs",
            "oldeip","ds","es","fs","edx","ecx","ebx" };
        #endregion
            #region Properties
        public List<StackData> Stacks
        {
            set;get;
        }
        public string Syscall
        {
            get
            {
                switch (_callNumber)
                {
                    case 2:
                        return "sys_fork()";
                    case 3:
                        return "sys_read()";
                    case 4:
                        return "sys_write()";
                    case 5:
                        return "sys_open()";
                    case 7:
                        return "sys_waitpid()";
                    case 11:
                        return "sys_execve()";
                    default:
                        Trace.WriteLine(_callNumber + ", unknown system call.");
                        return "";
                }
            }
        }
        override public string Description
        {
            get
            {
                return "eax = "+ _callNumber + ", the system call is " + Syscall
                    +".\nPushing user and kernel regsiters into the kernel stack.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
