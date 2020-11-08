using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using OSPresentation.TempStruct;

namespace OSPresentation.DataManipulation
{
    public class BP51 : BreakPoint
    {
        #region Contructor

        public BP51(string bpo, int bpn) : base(bpo, bpn)
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
                return "Adding the request to coorresponding device.\n" +
                        "Since there are existing request, it adds the request to the queue\n" +
                        "using elevator algorithm.";
            }
        }
        #endregion
        #region Methods
        #endregion
    }

}
