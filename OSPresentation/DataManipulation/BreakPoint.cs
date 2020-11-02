using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Media.Animation;

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
            funcitonName = Regex.Match(bpo, @",\s(.*?)\sat").Groups[1].Value;
            fileName = Regex.Match(bpo, @"at\s(.*?):").Groups[1].Value;
            lineString = Regex.Match(bpo, @"\d+?\t+(.*?)\n").Groups[1].Value;
            description = Regex.Match(bpo, @"\*{5,}\s*(.*?)\s*\*{5,}").Groups[1].Value;
            console = Regex.Match(bpo, @"-{50,}((.|\n)*?)-{50,}").Groups[1].Value;
            line = Int32.Parse(Regex.Match(bpo, @":(\d+?)\n").Groups[1].Value);
            foreach ((var value, int i) in Regex.Matches(bpo, @"\$\d+\s=\s(.*?)\n").Select((value, i) => (value, i)))
            {
                string v = value.Groups[1].Value;
                if (i == 0 && bpn != 94)
                {
                    jeffies = Int32.Parse(v);
                    Jeffies = jeffies;
                }
                else if (i == 1 && (bpn < 7 || bpn > 13) && bpn != 94)
                {
                    pid = Int32.Parse(v);
                    Pid = pid;
                }
                else
                    paras.Add(v);
            }
            var mc = Regex.Match(bpo, @"#\d+\s\s((.|\s)*?)\n(-|\n|\$)");
            foreach (var v in Regex.Split(mc.Groups[1].Value, @"\n#\d+\s\s"))
                bts.Add(v);
            foreach (Match value in Regex.Matches(bpo, @"0x.*?:\t(.*?)\n"))
                foreach (string v in Regex.Split(value.Groups[1].Value, @"\t"))
                    stks.Add(v);
        }
        #endregion
        #region Field
        protected string funcitonName, fileName, lineString;
        protected string description;
        protected string console;
        protected int jeffies, pid, line;
        protected List<string> paras=new List<string>();
        protected List<string> bts = new List<string>();
        protected List<string> stks = new List<string>();

        int _bpn=-1;
        #endregion
        #region Properties
        public int BPN { set=>_bpn=value; get=>_bpn; }
        public int Jeffies { set => jeffies = value; get => jeffies; }
        public int Pid { set => pid = value; get => pid; }
        #endregion
        #region Methods
        #endregion
    }

}
