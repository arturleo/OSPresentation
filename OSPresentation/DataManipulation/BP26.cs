using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Windows.Media.Animation;

namespace OSPresentation.DataManipulation
{
    public class BP26 : BreakPoint
    {
        #region Contructor

        public BP26(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int ChildPid { get => int.Parse(paras[0]); }
        override public string Description
        {
            get
            {
                return "Father process found ZOMBIE child process "+ ChildPid + ", \nreleased it and is returning from sys_waitpid().\n";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
