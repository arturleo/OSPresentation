using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP45 : BreakPoint
    {
        #region Contructor

        public BP45(string bpo, int bpn) : base(bpo, bpn)
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
                return "After the buffer head being unlocked and some process calling\n" +
                    "`wake_up()`, it schedules to the process.\n" +
                    "The process checks `bh->b_uptodate` and `bh->b_lock`, then returns\n " +
                    "to parent function.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
