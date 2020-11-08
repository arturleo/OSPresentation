using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP52 : BreakPoint
    {
        #region Contructor

        public BP52(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public string Dev { get => "dev="+paras [0]; }
        public string Cmd { get =>"cmd="+paras[1]; }
        public string Errors { get =>"errors="+paras[2]; }
        public string Sector { get =>"sector="+paras[3]; }
        public string Nr_sectors { get =>"nr_sectors="+paras[4]; }
        public string Buffer { get =>"buffer="+Regex.Match(paras[5], @"([\d\w]+?)\s").Groups[1].Value; }
        public string Waiting { get =>"waiting="+Regex.Match(paras[6], @"\)\s(.*?)\s<").Groups[1].Value; }
        public string Bh{ get =>"bh="+Regex.Match(paras[7], @"\)\s([\d\w]+)").Groups[1].Value; }
        public string Next{ get =>"next="+Regex.Match(paras[8], @"\)\s(.*?)\s<").Groups[1].Value; }
  
        override public string Description
        {
            get
            {
                return "Making and sending a request to the target block device.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
