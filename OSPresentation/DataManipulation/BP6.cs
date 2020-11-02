

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
        #endregion
        #region Methods

        #endregion
    }

}
