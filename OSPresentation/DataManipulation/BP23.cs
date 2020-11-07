using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    public class BP23 : BreakPoint
    {
        #region Contructor

        public BP23(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int NextPid { get => int.Parse(paras[1]); }
        public string Queue { get => paras[0]; }
        override public string Description
        {
            get
            {
                return "Waking up the head process "+ NextPid+ " of the queue.\n" +
                    "Changing the state to `RUNNING`.\nThen make the header to null \nbecause when the process is running, \nit will return to `sleep_on()` and make the next one in \n`tmp` pointer head of queue.";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
