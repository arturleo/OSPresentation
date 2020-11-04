

namespace OSPresentation.DataManipulation
{
    public class BP6:BreakPoint
    {
        #region Contructor
        public BP6(string bpo, int bpn): base(bpo, bpn)
        {
            Console = console;
        }
        #endregion
        #region Field



        #endregion
        #region Properties
        public string Console { set => console = value; get => console; }
        override public string Description {
            get
            {
                return "The console is being written to via tty_write().";
            }
        }
        #endregion
        #region Methods

        #endregion
    }

}
