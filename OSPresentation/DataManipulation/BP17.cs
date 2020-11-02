using System;
using System.Collections.Generic;

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
        #endregion
        #region Methods
        public ProcessStruct Refresh()
        {
            return new ProcessStruct(paras[3], paras[4],
                paras[5], paras[6], paras[7], paras[8], paras[9]);
        }
        #endregion
    }

}
