using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Media.Animation;
using System.Globalization;

namespace OSPresentation.DataManipulation
{

    public abstract class BreakPoint
    {
        #region Contructor
        public BreakPoint(string bpo, int bpn)
        {
            BPN = bpn;
            Jeffies = -1;
            Pid = -1;
            functionName = Regex.Match(bpo, @",\s(.*?)\sat").Groups[1].Value.Trim();
            fileName = Regex.Match(bpo, @"at\s(.*?):").Groups[1].Value.Trim();
            lineString = Regex.Match(bpo, @"\d+?\t+(.*?)\n").Groups[1].Value.Trim();
            desp = Regex.Match(bpo, @"\*{5,}\s*(.*?)\s*\*{5,}").Groups[1].Value.Trim();
            console = Regex.Match(bpo, @"-{50,}((.|\n)*?)-{50,}").Groups[1].Value.Trim();
            line = Int32.Parse(Regex.Match(bpo, @":(\d+?)\n").Groups[1].Value);

            CodeLine = "Breakpoint " + BPN + " " + functionName + " " + fileName + ":" + line + "\n" +
    line + "\t\t" + lineString+ "\n";

            foreach ((var value, int i) in Regex.Matches(bpo, @"\$\d+\s=\s(.*?)\n").Select((value, i) => (value, i)))
            {
                string v = value.Groups[1].Value.Trim();
                if (i == 0 && bpn != 32)
                    Jeffies = Int32.Parse(v);
                else if (i == 1 && (bpn < 7 || bpn > 13) && bpn != 32)
                    Pid = Int32.Parse(v);
                else
                    paras.Add(v);
            }

            var mc = Regex.Match(bpo, @"#\d+\s\s((.|\s)*?)\n(-|\n|\$)");
            if (mc != null)
            {
                foreach (var v in Regex.Split(mc.Groups[1].Value, @"\n#\d+\s\s"))
                {
                    bts.Add(v.Trim());
                    CodeLine += v.Trim() + "\n";
                }
            }

            foreach (Match value in Regex.Matches(bpo, @"0x.*?:\t(.*?)\n"))
                foreach (string v in Regex.Split(value.Groups[1].Value, @"\t"))
                    stks.Add(v.Trim());

            String addr = Regex.Match(bpo, @"0x((\d|\w)*?):").Groups[1].Value;
            if (!String.IsNullOrEmpty(addr))
                startAddress = int.Parse(addr, NumberStyles.HexNumber);
        }
        #endregion
        #region Field
        protected string functionName, fileName, lineString;
        protected string desp;
        protected string console;
        protected int startAddress;
        protected int line;
        protected List<string> paras=new List<string>();
        protected List<string> bts = new List<string>();
        protected List<string> stks = new List<string>();
        #endregion

        #region Properties
        public int BPN { set; get; }
        public int Jeffies { set; get; }
        public int Pid { set; get; }
        public abstract string Description { get; }
        public string CodeLine { set; get; }
        #endregion
        #region Methods
        #endregion
    }

}
