using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP54 : BreakPoint
    {
        #region Contructor

        public BP54(string bpo, int bpn) : base(bpo, bpn)
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
                return "Inside do_fd_request(), it uses a timer\n " +
                    "to turn on the floopy motor.\n" +
                    "When time is up, it will call `floppy_on_interrupt`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
