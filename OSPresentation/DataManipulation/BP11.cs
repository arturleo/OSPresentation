using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    //TODO 
    public class BP11 : BreakPoint
    {
        #region Contructor

        public BP11(string bpo, int bpn) : base(bpo, bpn)
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
                return "Returned from C function copy_process(), popping gs, esi, edi, ebp, eax\n";
            }
        }
            #endregion
        #region Methods
        #endregion
    }

}
