using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{ 
    public class BP19 : BreakPoint
    {
        #region Contructor

        public BP19(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int ProcessPid { get => int.Parse(paras[0]); }
        public int ProcessCounter { get => int.Parse(paras[1]); }
        override public string Description
        {
            get
            {
                return "If no process counter is positive, refresh the counter according to the priority.";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
