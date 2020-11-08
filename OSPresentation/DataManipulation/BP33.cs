using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;
using System.Windows.Media.Animation;

namespace OSPresentation.DataManipulation
{
    public class BP33 : BreakPoint
    {
        #region Contructor

        public BP33(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int FD { get=> int.Parse(paras[0]); }
        public bool IsNew { get => paras[1] != "-1" ? false : true; }
        override public string Description
        {
            get
            {
                return paras[1]!="-1"?"Trying to find a free file struct in the array.": "Found a free file struct in the array.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
