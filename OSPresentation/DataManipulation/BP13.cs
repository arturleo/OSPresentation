using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    //TODO 
    public class BP13 : BreakPoint
    {
        #region Contructor

        public BP13(string bpo, int bpn) : base(bpo, bpn)
        {
        }
        #endregion
        #region Field

        #endregion
            #region Properties
        override public string Description
        {
            get
            {
                return "Returned from function pointed by $eax.\n" +
                    "Popping stored fs, es, ds, edx, ecx to registers\n";
            }
        }
            #endregion
        #region Methods
        #endregion
    }

}
