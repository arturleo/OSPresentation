using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OSPresentation.TempStruct
{
    public class ProcessStruct
    {
        #region Constructor
        public ProcessStruct(int taskN)
        {
            TaskN = taskN;
            Pid = -1;
            State = -1;
            Counter = -1;
            Priority = -1;
            Father = -1;
            ExitCode = -1;
            Signal = -1;
        }
        public ProcessStruct(int taskn, int pid, int state, int counter,
            int priority, int father, int exitCode, int signal)
        {
            TaskN = taskn;
            Pid = pid;
            State = state;
            Counter = counter;
            Priority = priority;
            Father = father;
            ExitCode = exitCode;
            Signal = signal;
        }
        public ProcessStruct(string taskn, string pid, string state, string counter,
            string priority, string father, string exitCode, string signal)
        {
            TaskN = int.Parse(taskn);
            Pid = Int32.Parse(pid);
            State = Int32.Parse(state);
            Counter = Int32.Parse(counter);
            Priority = Int32.Parse(priority);
            Father = Int32.Parse(father);
            ExitCode = Int32.Parse(exitCode);
            Signal = Int32.Parse(signal);
        }

        #endregion
        #region Fields
        #endregion
        #region Properties
        public int TaskN { set; get; }
        public int Pid { set; get; }
        public int State { set; get; }
        public int Counter { set; get; }
        public int Priority { set; get; }
        public int Father { set; get; }
        public int ExitCode { set; get; }
        public int Signal { set; get; }
        public string StateString
        {
            get
            {
                switch (State)
                {
                    case 0:
                        return "TASK_RUNNING";
                    case 1:
                        return "TASK_INTERRUPTIBLE";
                    case 2:
                        return "TASK_UNINTERRUPTIBLE";
                    case 4:
                        return "TASK_STOPPED";
                    case 3:
                        return "TASK_ZOMBIE";
                    default:
                        Trace.WriteLine(TaskN+"  "+ State + ", unknown state.");
                        return "";
                }
            }
        }
        public string PidContent
        {
            get => $"pid={Pid}";
        }
        public string ToolTipContent
        {
            get
            {
                return "state: " + StateString + "\ncounter:" + Counter + "\npriority:" +
                Priority + "\nfather:" + Father + "\nexit_code:"+ ExitCode+ "\nsignal:" +Signal;
            }
        }
        #endregion
        #region Methods
        public void SetToDefault()
        {
            this.Update(TaskN.ToString(), "-1", "-1", "-1", "-1", "-1", "-1", "-1");
        }
        public void Update(string taskn, string pid, string state, string counter,
            string priority, string father, string exitCode, string signal)
        {
            TaskN = int.Parse(taskn);
            Pid = Int32.Parse(pid);
            State = Int32.Parse(state);
            Counter = Int32.Parse(counter);
            Priority = Int32.Parse(priority);
            Father = Int32.Parse(father);
            ExitCode = Int32.Parse(exitCode);
            Signal = Int32.Parse(signal);
        }
        public string PrintGreaterCounter(int c, int next)
        {
            if(Counter>c)
            {
                c = Counter;
                next = TaskN;
            }

            return "c: " + c + " next: " + next;
        }
        #endregion
    }
}
