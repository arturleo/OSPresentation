using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP42 : BreakPoint
    {
        #region Contructor

        public BP42(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Buffer { get =>Regex.Match(functionName,"(\".*?\")").Groups[1].Value; }
        override public string Description
        {
            get
            {
                return "Returing to `file_read`. First it calculates to start position to copy from `flip->f_poz`, `nr` is 0, ``";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
