using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP42 : BreakPoint
    {
        #region Contructor

        public BP42(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Buffer { get =>Regex.Match(paras[0],"(\".*?\")").Groups[1].Value; }
        override public string Description
        {
            get
            {
                return "Returing to `file_read`. First it calculates to start position to copy from `flip->f_poz`, `nr` is 0, `chars` number is 6, `left` of unread chars is 0. Second it writes the buffer content: "+Buffer+" to user stack with function `put_fs_byte`. After that, it releases the buffer head and return to parent funciton `sys_read`.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
