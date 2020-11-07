using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Windows.Media.Animation;

namespace OSPresentation.DataManipulation
{
    public class BP24 : BreakPoint
    {
        #region Contructor

        public BP24(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int TaskN { get => int.Parse(paras[0]); }
        override public string Description
        {
            get
            {
                return "Emptying task["+TaskN+"], \nfreeing the page of the process.\n";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
