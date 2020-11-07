using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    //TODO 
    public class BP16 : BreakPoint
    {
        #region Contructor

        public BP16(string bpo, int bpn) : base(bpo, bpn)
        {

        }

        #endregion
        #region Field
        #endregion
        #region Properties
        public int LastPid { get => int.Parse(paras[2]); }
        public int Empty { get => int.Parse(paras[1]); }
        public int TaskN { get => int.Parse(paras[0]); }

        override public string Description
        {
            get
            {
                return "Iterating over all tasks to find the find empty process\n for the new process.";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
