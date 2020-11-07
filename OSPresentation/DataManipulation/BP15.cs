using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Text.RegularExpressions;

namespace OSPresentation.DataManipulation
{
    //TODO 
    public class BP15 : BreakPoint
    {
        #region Contructor

        public BP15(string bpo, int bpn) : base(bpo, bpn)
        {
            EIP = Regex.Match(addresses[0], @"(0x.*?)\s<").Groups[1].Value;
            ESP = addresses[1];
        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string EIP { set; get; }
        public string ESP { set; get; }
        public List<StackData> Stacks
        {
            set; get;
        }
        override public string Description
        {
            get
            {
                return "Changing eip and esp so that after returning to the kernel stack and calling ret_from_syscall, \nit will go to the esp which points to the address new program read from the excutive file.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
