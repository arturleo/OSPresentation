using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP55 : BreakPoint
    {
        #region Contructor

        public BP55(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Bwait { get => paras[0]; }
        override public string Description
        {
            get
            {
                return "After copying buffer from the ready floopy and\n" +
                    " deselecting current drive in `rw_interrupt`,\n" +
                    "call function end_request().\n" +
                    "Inside the function, buffer head state is updated.\n" +
                    "Then the buffer is unlocked.\n" +
                    "Now it's calling wake_up(&bh->b_wait: "+ Bwait+" ) to wake up process \n" +
                    "waiting for the buffer to unlock and locking it after that.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
