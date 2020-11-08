using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP57 : BreakPoint
    {
        #region Contructor

        public BP57(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Wait_for_request { get => paras[0]; }
        override public string Description
        {
            get
            {
                return "Calling wake_up(&wait_for_request: " + Wait_for_request + " )\n " +
                    "to wake up process waiting for an empty request \n" +
                    "if the request number reaches its limit.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
