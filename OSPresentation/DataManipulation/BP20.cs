using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    public class BP20 : BreakPoint
    {
        #region Contructor

        public BP20(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int NextTask { get => int.Parse(paras[0]); }
        public int NextPid { get => int.Parse(paras[1]); }
        override public string Description
        {
            get
            {
                return "Switching to the next process pointed by TASK[next].";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
