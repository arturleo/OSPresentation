using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP38 : BreakPoint
    {
        #region Contructor

        public BP38(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        override public string Description
        {
            get
            {
                return "Trying to find the character device to write to.\n" +
                    "According to `dev` "+ paras[0]+" in character table, \n" +
                    "it find the entry address of function `rw_ttyx`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
