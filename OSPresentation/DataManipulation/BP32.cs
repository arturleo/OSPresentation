using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Windows.Media.Animation;

namespace OSPresentation.DataManipulation
{
    public class BP32 : BreakPoint
    {
        #region Contructor

        public BP32(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int FatherPid { get => int.Parse(paras[0]); }
        public int FatherSignal { get => int.Parse(paras[1]); }

        override public string Description
        {
            get
            {
                return "Changing current state to TASK_ZOMBIE and\n sending signal SIGCHID to father process "+FatherPid+".\nThen father signal field will be "+FatherSignal+".";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
