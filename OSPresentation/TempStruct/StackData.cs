using System;
using System.Collections.Generic;
using System.Text;

namespace OSPresentation.TempStruct
{
    public class StackData
    {
        public StackData(int addr, string c, string r)
        {
            Address = "0x"+addr.ToString("X");
            Content = c;
            Register = r;
        }
        #region Properties
        public String Address { set; get; }
        public String Content { set; get; }
        public String Register {set;get;}

        public string Description => $"Address: {Address} \nContent: {Content}";

        #endregion
        #region Methods
        #endregion
    }
}
