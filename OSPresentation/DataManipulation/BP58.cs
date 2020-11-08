using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP58 : BreakPoint
    {
        #region Contructor

        public BP58(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Next { get => Regex.Match(paras[1], @"(0x.*?)\s").Groups[1].Value; }
        override public string Description
        {
            get
            {
                return "Switching to next request: "+Next+".";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
