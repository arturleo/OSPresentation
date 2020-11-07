using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Windows.Media.Animation;

namespace OSPresentation.DataManipulation
{
    public class BP27 : BreakPoint
    {
        #region Contructor

        public BP27(string bpo, int bpn) : base(bpo, bpn)
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
                return "Father process "+Pid+" found RUNNING child process "+ChildPid+"" +
                    " ,\nchange father state to `TASK_INTERRUPTIBLE` and call `schedule()`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
