using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Windows.Media.Animation;

namespace OSPresentation.DataManipulation
{
    public class BP29 : BreakPoint
    {
        #region Contructor

        public BP29(string bpo, int bpn) : base(bpo, bpn)
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
                return "Exiting the current process. Closing opened files and process file table.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
