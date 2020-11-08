using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP36 : BreakPoint
    {
        #region Contructor

        public BP36(string bpo, int bpn) : base(bpo, bpn)
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
                return "To pick a read function according to the target,\n it checks `inode->imode`.\n" +
                    "S_ISCHR(inode->i_mode)="+paras[0]+ ", S_ISBLK(inode->i_mode)=" + paras[1]+
                    "S_ISDIR(inode->i_mode)=" + paras[2] + ", S_ISREG(inode->i_mode)=" + paras[3] +
                    "\nCurrently the target is block file, so it calls `file_read()`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
