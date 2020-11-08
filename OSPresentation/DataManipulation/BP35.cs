using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP35 : BreakPoint
    {
        #region Contructor

        public BP35(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Mode { get => "f_mode="+paras[1]; }
        public string Flags { get => "f_flags="+paras[2]; }
        public string Count { get => "f_count="+paras[3]; }
        public string Inode { get => "f_inode="+Regex.Match(paras[4],@"\s(0x.*?)\s<").Groups[1].Value; }
        public string Pos { get => "f_pos="+paras[5]; }
        override public string Description
        {
            get
            {
                return "Initializing the file structure.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
