using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP37 : BreakPoint
    {
        #region Contructor

        public BP37(string bpo, int bpn) : base(bpo, bpn)
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
                return "To pick a write function according to the target,\n it checks `inode->imode`.\n" +
                    "S_ISCHR(inode->i_mode)="+paras[0]+ ", S_ISBLK(inode->i_mode)=" + paras[1]+
                    ", S_ISREG(inode->i_mode)=" + paras[2] +
                    "\nCurrently the target is char, so it calls `rw_char()`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
