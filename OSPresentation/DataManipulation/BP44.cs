using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP44 : BreakPoint
    {
        #region Contructor

        public BP44(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int Dev { get =>int.Parse(Regex.Match(functionName,@"dev=(.*?),").Groups[1].Value); }
        public int Block { get =>int.Parse(Regex.Match(functionName,@"block=(.*?)\)").Groups[1].Value); }
        override public string Description
        {
            get
            {
                return "Stepping into "+ functionName+".\n" +
                    "Aftering requiring a buffer block and testing whether it's up-to-date,\n" +
                    " it calls `ll_rw_bloack` requiring to read the block.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
