using System;
using System.Collections.Generic;
using System.Text;

namespace OSPresentation.TempStruct
{
    public class ProcessStruct
    {
        #region Fields

        #endregion
        #region Properties
        public int Pid { set; get; }
        public int State { set; get; }
        public int Counter { set; get; }
        public int Priority { set; get; }
        public int Father { set; get; }
        public int ExitCode{ set; get; }
        public int Signal { set; get; }
        #endregion
        public ProcessStruct(int pid, int state, int counter, 
            int priority, int father, int exitCode, int signal)
        {
            Pid = pid;
            State = state;
            Counter = counter;
            Priority = priority;
            Father = father;
            ExitCode = exitCode;
            Signal = signal;
        }

        public ProcessStruct(string pid, string state, string counter,
    string priority, string father, string exitCode, string signal)
        {
            Pid = Int32.Parse(pid);
            State = Int32.Parse(state);
            Counter = Int32.Parse(counter);
            Priority = Int32.Parse(priority);
            Father = Int32.Parse(father);
            ExitCode = Int32.Parse(exitCode);
            Signal = Int32.Parse(signal);
        }
        #region Methods
        public string PrintStatus()
        {
            return "state: "+State+"\n test";
        }
        #endregion
    }
}
