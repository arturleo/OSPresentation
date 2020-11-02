using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Animation;

using System.Text.RegularExpressions;

using MaterialDesignThemes.Wpf;

using OSPresentation.TempStruct;
using OSPresentation.DataManipulation;

namespace OSPresentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();

            Storyboard.SetTargetName(consoleText, consoleBox.Name);
            Storyboard.SetTargetProperty(consoleText, new PropertyPath("Text"));
            consoleText.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTargetName(jeffiesText, JeffiesN.Name);
            Storyboard.SetTargetProperty(jeffiesText, new PropertyPath(JeffiesN.Text));
            jeffiesText.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTargetName(pidText, PidN.Name);
            Storyboard.SetTargetProperty(pidText, new PropertyPath(PidN.Text));
            pidText.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTargetName(counterText, PidN.Name);
            Storyboard.SetTargetProperty(counterText, new PropertyPath(CounterN.Text));
            counterText.FillBehavior = FillBehavior.HoldEnd;
        }

        #region Fields
        #region DataManipulation
        List<BreakPoint> breakPoints = new List<BreakPoint>();

        #endregion

        #region AnimationData
        long gtime = 0, ltime=0;

        int _ajeffies=-1, __apid=-1, __acounter=-1;
        //TODO: store stack, queue, drawboard data here.
        List<ProcessStruct> processStructs = new List<ProcessStruct>();

        const int itvm = 20, itv0=200, itv1=1000;
        #endregion

        #region AnimationHolders
        Storyboard OSAnimation = new Storyboard();
        StringAnimationUsingKeyFrames consoleText = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames jeffiesText = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames pidText = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames counterText = new StringAnimationUsingKeyFrames();
        #endregion

        #region UIStatus
        int _playing = 0;
        #endregion

        #endregion

        #region Methods
        void dataManipulate(string dataFile)
        {
            try
            {
                List<BreakPoint> lasts = new List<BreakPoint>();
                using (var sr = new StreamReader(dataFile))
                {
                    string dataInput = sr.ReadToEnd();
                    Type objType = typeof(BP6);

                    foreach (var bp in Regex.Split(dataInput, @"Breakpoint\s"))
                    {
                        int bpn=-1;
                        try
                        {
                             bpn= Int32.Parse(Regex.Match(bp, @"(\d+?),").Groups[1].Value);
                        }
                        catch(System.FormatException fe)
                        {
                            continue;
                        }
                        if (bpn != 6) continue;
                        if (6 <= bpn && bpn <= 58 && bpn!=48 && bpn!=25 )
                        {
                            Type bpC = Type.GetType("OSPresentation.DataManipulation.BP" + bpn);
                            switch (bpn)
                            { 
                                case 44:
                                    
                                default:
                                    breakPoints.Add((BreakPoint)Activator.CreateInstance(bpC,bp,bpn));
                                    break;
                            }
                        }
                        else continue;
                    };
                }
            }
            catch(System.InvalidOperationException)
            {
                //show on dialogue;
            }
            Trace.WriteLine("Data Manipulation succeeds");




            addAnimation();
            Trace.WriteLine("Animation processing succeeds");
        }

        void addAnimation()
        {
            gtime=0;


            for (int ind=0;ind<breakPoints.Count;)
            {
                BreakPoint bp = breakPoints[ind];
                if (bp.Jeffies >= 0)
                    HandleJeffies(bp.Jeffies);
                if (bp.Pid >= 0)
                    HandlePid(bp.Pid);

                switch (bp.BPN)
                {
                    case 6:
                        BP6 bp6 = (BP6)bp;
                        changeConsole(bp6.Console);
                        break;
                    default:
                        throw new System.InvalidOperationException("invalid breakpoint added");
                        //we should crash here;
                }
                ind++;
                ltime = gtime;
            };

            setDurationAfterAll();
            addToStoryBoard();
        }
        #region AnimationMethods
        void changeConsole(string str)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = str;
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            consoleText.KeyFrames.Add(dsk);
        }

        void HandleJeffies(int j)
        {
            //start status
            if (_ajeffies < 0)
                _ajeffies = j - 1;

            for (int i = _ajeffies; i < j; )
            {
                DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
                dsk.Value = (++i).ToString();
                dsk.KeyTime = TimeSpan.FromMilliseconds(gtime+=100);
                jeffiesText.KeyFrames.Add(dsk);

                --__acounter;
                if (__acounter < 0)
                {
                    //TODO: recheck preocess table with the counter
                    Trace.WriteLine("counter is less then 0,"+__apid+", "
                        +_ajeffies);
                    continue;
                }
                DiscreteStringKeyFrame dsk2 = new DiscreteStringKeyFrame();
                dsk2.Value = (__acounter).ToString();
                dsk2.KeyTime = TimeSpan.FromMilliseconds(gtime);

                counterText.KeyFrames.Add(dsk2);
                
            }
            _ajeffies = j;
        }

        //normally, it wont work before switching, but who knows?
        void HandlePid(int p)
        {
#nullable enable
            ProcessStruct? pnew = getProcess(p);
            if (p != __apid)
            {
                DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
                dsk.Value = p.ToString();
                dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);
                consoleText.KeyFrames.Add(dsk);
            }
            
            string ccounter;
            if(p != __apid&& pnew == null)
            {
                ccounter = "???";
            }
            else if(p == __apid && (pnew == null||pnew.Counter==__acounter))
            {
                return;
            }
            else
            {
                ccounter = pnew.Counter.ToString();
            }
            __apid = p;
            DiscreteStringKeyFrame dsk2 = new DiscreteStringKeyFrame();
            dsk2.Value = ccounter;
            dsk2.KeyTime = TimeSpan.FromMilliseconds(gtime);
            counterText.KeyFrames.Add(dsk2);
#nullable restore
        }

        void setDurationAfterAll()
        {
            consoleText.Duration = TimeSpan.FromMilliseconds(gtime);
            jeffiesText.Duration = TimeSpan.FromMilliseconds(gtime);
            pidText.Duration = TimeSpan.FromMilliseconds(gtime);
            counterText.Duration = TimeSpan.FromMilliseconds(gtime);
        }

        void addToStoryBoard()
        {
            OSAnimation.Children.Add(consoleText);
            OSAnimation.Children.Add(pidText);
            OSAnimation.Children.Add(jeffiesText);
            OSAnimation.Children.Add(counterText);

        }

        #region StructureHandlers
        ProcessStruct getProcess(int pid)
        {
            foreach(var ps in processStructs)
            {
                if (ps.Pid == pid)
                    return ps;
            }
            return null;
        }
        #endregion
        #endregion
        #region ButtonInteactions
        void LoadBreakpointFile(object sender, DialogClosingEventArgs args)
        {
            string dataFile = "";
            if (!Equals(args.Parameter, true)) return;

            if(string.IsNullOrWhiteSpace(bpFileBox.Text))
                dataFile = "./Data/output_all_m.txt";
            else
                dataFile = bpFileBox.Text.Trim();

            dataManipulate(dataFile);
        }

        //TODO not loaded?
        void StartPauseAnimation(object sender, RoutedEventArgs args)
        {
            if(_playing==0)
            {
                OSAnimation.Begin(App.Current.MainWindow);
                _playing = 1;
            }
            else if(_playing==1)
            {
                OSAnimation.Pause();
                _playing = 2;
            }
            else if(_playing==2)
            {
                OSAnimation.Resume();
                _playing = 1;
            }
        }

        #endregion  
        #endregion
    }
}