using System;
using System.Collections.Generic;
using System.Data;
using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP17 : BreakPoint
    {
        #region Contructor
        public BP17(string bpo, int bpn) : base(bpo, bpn)
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
                return "Checking from the greatest task number to find the running process with the greatest counter.";
            }
        }

        #endregion
        #region Methods
        public ProcessStruct Process()
        {
            return new ProcessStruct(paras[0], paras[1], paras[2], paras[3], paras[4],
                paras[5], paras[6], paras[7]);
        }

        public ProcessStruct Refresh(ProcessStruct ps)
        {
            ps.Update(paras[0], paras[1], paras[2], paras[3], paras[4],
                paras[5], paras[6], paras[7]);
            return ps;
        }
        #endregion
    }

}
