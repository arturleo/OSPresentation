using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP49 : BreakPoint
    {
        #region Contructor

        public BP49(string bpo, int bpn) : base(bpo, bpn)
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
                return "Called `ll_rw_block`, after checking for errors of buffer head in \n" +
                    "`ll_rw_block` and `make_request`, it is trying to lock the buffer for query.\n" +
                    "Now it is checking if the buffer is locked, if so, it sleeps until it's unlocked.\n" +
                    "Since buffer hasn't been locked, it closes the interruption, locks the buffer, and\n " +
                    "reopen the interrruption.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
