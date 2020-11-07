using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using OSPresentation.TempStruct;


namespace OSPresentation.DataManipulation
{
    //TODO 
    public class BP14 : BreakPoint
    {
        #region Contructor

        public BP14(string bpo, int bpn) : base(bpo, bpn)
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
                return "Calling `int do_execve(unsigned long * eip,long tmp,char * filename,char** argv, char** envp)`. \nStarting to read the excutive file of the user command.";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
