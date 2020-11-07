using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    public class BP22 : BreakPoint
    {
        #region Contructor

        public BP22(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        // For simplicisity, we use the data of fixed breakpoints.
        override public string Description
        {
            get
            {
                return "Returning from `schedule()` in sleep_on().\nThe `tmp` pointer points to the next process in queue.\nNext process is to be set at the head of queue and state should be `RUNNING`.\nHowever, next process is empty.";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
