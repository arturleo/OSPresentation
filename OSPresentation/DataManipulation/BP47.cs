using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP47 : BreakPoint
    {
        #region Contructor

        public BP47(string bpo, int bpn) : base(bpo, bpn)
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
                return "After requesting the block, process returned back to\n" +
                    "`bread()`. Then it goes to `wait_on_buffer()` and sleeps \n" +
                    "until the buffer head is unlocked and some process calls\n" +
                    "`wake_up()`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
