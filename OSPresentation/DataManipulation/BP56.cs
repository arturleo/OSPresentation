using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP56 : BreakPoint
    {
        #region Contructor

        public BP56(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Waiting { get => paras[0]; }
        override public string Description
        {
            get
            {
                return "Calling wake_up(&CURRENT->waiting: " + Waiting + " ) to wake up process \n" +
                    "waiting for current floppy request.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
