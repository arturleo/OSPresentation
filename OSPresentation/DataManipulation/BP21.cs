using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    public class BP21 : BreakPoint
    {
        #region Contructor

        public BP21(string bpo, int bpn) : base(bpo, bpn)
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
                return "Setting current process at the head of queue, \nmoving the original one to the `tmp` variable curent process pointing to. \nHowever, current queue didnt have process previously. \nAfter that, the state of current process is set to `TASK_UNINTERRUPTIBLE`";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
