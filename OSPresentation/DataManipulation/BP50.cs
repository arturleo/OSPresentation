using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP50 : BreakPoint
    {
        #region Contructor

        public BP50(string bpo, int bpn) : base(bpo, bpn)
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
                return "Adding the request to coorresponding device.\n" +
                    "Since there is no request, it directly calls the request function\n" +
                    "`do_fd_request()`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
