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
using static System.Linq.Enumerable;
using System.Text.RegularExpressions;

using MaterialDesignThemes.Wpf;

using OSPresentation.TempStruct;
using OSPresentation.DataManipulation;
using System.Windows.Controls.Primitives;

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



            //tricks here
            //ListViewItem lvi = (ListViewItem)(KernelStackList.FindName("s1"));

            Storyboard.SetTargetName(consoleText, consoleBox.Name);
            Storyboard.SetTargetProperty(consoleText, new PropertyPath("Text"));
            consoleText.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTargetName(jeffiesText, JeffiesN.Name);
            Storyboard.SetTargetProperty(jeffiesText, new PropertyPath("Text"));
            jeffiesText.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTargetName(pidText, PidN.Name);
            Storyboard.SetTargetProperty(pidText, new PropertyPath("Text"));
            pidText.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTargetName(counterText, CounterN.Name);
            Storyboard.SetTargetProperty(counterText, new PropertyPath("Text"));
            counterText.FillBehavior = FillBehavior.HoldEnd;
        }

        #region Fields
        #region DataManipulation
        List<BreakPoint> breakPoints = new List<BreakPoint>();

        #endregion

        #region AnimationData
        long gtime = 0, ltime=0;

        int _ajeffies=-1, _apid=-1, _acounter=-1;
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

        #region ElementList
        //Buttons
        List<Button> processButtons = new List<Button>();
        List<ListViewItem> stackButtons = new List<ListViewItem>();
        List<List<Button>> fileButtons = new List<List<Button>>();
        //Tooltips
        List<ToolTip> processToolTips=new List<ToolTip>();
        List<ToolTip> stackToolTips = new List<ToolTip>();
        List<List<ToolTip>> fileToolTips = new List<List<ToolTip>>();
        #endregion

        #endregion

        #region Methods
        void dataManipulate(string dataFile)
        {
            List<BreakPoint> lasts = new List<BreakPoint>();
            try
            {
                using (var sr = new StreamReader(dataFile))
                {
                    string dataInput = sr.ReadToEnd();
                    Type objType = typeof(BP6);

                    foreach (var bp in Regex.Split(dataInput, @"Breakpoint\s"))
                    {
                        int bpn = -1;
                        try
                        {
                            bpn = Int32.Parse(Regex.Match(bp, @"(\d+?),").Groups[1].Value);
                        }
                        catch (FormatException fe)
                        {
                            Trace.WriteLine(Regex.Match(bp, @"(\d+?),").Groups[1].Value +
                                ", no match for int, skip.");
                            continue;
                        }
                        if (bpn != 6) continue;
                        if (6 <= bpn && bpn <= 58 && bpn != 48 && bpn != 25)
                        {
                            Type bpC = Type.GetType("OSPresentation.DataManipulation.BP" + bpn);
                            switch (bpn)
                            {
                                case 44:

                                default:
                                    breakPoints.Add((BreakPoint)Activator.CreateInstance(bpC, bp, bpn));
                                    break;
                            }
                        }
                        else continue;
                    };
                }
            }
            catch(InvalidOperationException)
            {
                //show on dialogue;
            }
            Trace.WriteLine("Data Manipulation succeeds");
            addUIElementsRegisterAnimation();



            addAnimation();
            Trace.WriteLine("Animation processing succeeds");
            intializeUI();
        }

        // The data are fixed here
        // There are some subfuncitons for dulplicate buttons
        void addUIElementsRegisterAnimation()
        {
            foreach (var index in Range(1, 7))
                addStackButton();
        }

        void addProcessButton()
        {

        }

        void addStackButton()
        {
            ListViewItem stackObject = new ListViewItem();
            ToolTip tt = new ToolTip();
            tt.Content = "";
            tt.PlacementTarget = stackObject;
            tt.HorizontalOffset = 15;
            tt.Placement = PlacementMode.Right;
            stackObject.ToolTip = tt;
            stackObject.Content = "";
            stackObject.Visibility = Visibility.Collapsed;

            stackButtons.Add(stackObject);
            stackToolTips.Add(tt);

            KernelStackList.Items.Add(stackObject);
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
                        throw new InvalidOperationException("invalid breakpoint added");
                        //we should crash here;
                }
                ind++;
                ltime = gtime;
            };

            setDurationAfterAll();
            addToStoryBoard();

            ToolTip tt=(ToolTip)pid0Button.ToolTip;
            // I have to do so due to bug in xaml
            tt.PlacementTarget = pid0Button;
            //temporary area
            BooleanAnimationUsingKeyFrames pid0Mouseover = new BooleanAnimationUsingKeyFrames();

            Storyboard.SetTarget(pid0Mouseover, tt);
            Storyboard.SetTargetProperty(pid0Mouseover, new PropertyPath("IsOpen"));
            consoleText.FillBehavior = FillBehavior.HoldEnd;

            DiscreteBooleanKeyFrame dbkf = new DiscreteBooleanKeyFrame();
            dbkf.Value = true;
            dbkf.KeyTime = TimeSpan.FromSeconds(3);
            pid0Mouseover.KeyFrames.Add(dbkf);
            OSAnimation.Children.Add(pid0Mouseover);

        }
        #region AnimationMethods
        void setDurationAfterAll()
        {
            consoleText.Duration = TimeSpan.FromMilliseconds(gtime);
            jeffiesText.Duration = TimeSpan.FromMilliseconds(gtime);
            pidText.Duration = TimeSpan.FromMilliseconds(gtime);
            counterText.Duration = TimeSpan.FromMilliseconds(gtime);

            DoubleAnimation daSlider = new DoubleAnimation();
            daSlider.From = 0;
            daSlider.To = gtime / 1000;
            daSlider.Duration = TimeSpan.FromMilliseconds(gtime);
            Storyboard.SetTargetName(daSlider, AnimationSlider.Name);
            Storyboard.SetTargetProperty(daSlider, new PropertyPath("Value"));
            OSAnimation.Children.Add(daSlider);
        }

        void addToStoryBoard()
        {
            OSAnimation.Children.Add(consoleText);
            OSAnimation.Children.Add(pidText);
            OSAnimation.Children.Add(jeffiesText);
            OSAnimation.Children.Add(counterText);

            OSAnimation.Completed += new EventHandler(AnimationCompleted);

        }



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

                --_acounter;
                if (_acounter < 0)
                {
                    //TODO: recheck preocess table with the counter
                    Trace.WriteLine("counter is less then 0,"+_apid+", "
                        +_ajeffies);
                    continue;
                }
                DiscreteStringKeyFrame dsk2 = new DiscreteStringKeyFrame();
                dsk2.Value = (_acounter).ToString();
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
            if (p != _apid)
            {
                DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
                dsk.Value = p.ToString();
                dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);
                pidText.KeyFrames.Add(dsk);
            }
            
            string ccounter;
            if(p != _apid&& pnew == null)
            {
                ccounter = "???";
            }
            else if(p == _apid && (pnew == null||pnew.Counter==_acounter))
            {
                return;
            }
            else
            {
                ccounter = pnew.Counter.ToString();
            }

            _apid = p;
            DiscreteStringKeyFrame dsk2 = new DiscreteStringKeyFrame();
            dsk2.Value = ccounter;
            dsk2.KeyTime = TimeSpan.FromMilliseconds(gtime);
            counterText.KeyFrames.Add(dsk2);
#nullable restore
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

        #region Events
        void AnimationCompleted(object sender, EventArgs e)
        {
            _playing = 0;
        }
        #endregion

        #region UI
        void intializeUI()
        {
            StartPauseAnimationButton.IsEnabled = true;
            ReplayAnimationButton.IsEnabled = true;
            AnimationSlider.Maximum = gtime / 1000;
            AnimationSlider.IsEnabled = true;
        }

        #region ButtonInteactions
        void LoadBreakpointFile(object sender, DialogClosingEventArgs args)
        {
            string dataFile = "";
            if (!Equals(args.Parameter, true)) return;

            if (string.IsNullOrWhiteSpace(bpFileBox.Text))
                dataFile = "./Data/output_all_m.txt";
            else
                dataFile = bpFileBox.Text.Trim();

            dataManipulate(dataFile);
        }

        //TODO not loaded?
        void StartPauseAnimation(object sender, RoutedEventArgs args)
        {
            if (_playing == 0)
            {
                OSAnimation.Begin(App.Current.MainWindow, true);
                if (AnimationSlider.Value > 0)
                    OSAnimation.Seek(App.Current.MainWindow,
                     TimeSpan.FromSeconds(AnimationSlider.Value), TimeSeekOrigin.BeginTime);
                _playing = 1;
            }
            else if (_playing == 1)
            {
                OSAnimation.Pause(App.Current.MainWindow);
                _playing = 2;
            }
            else if (_playing == 2)
            {
                OSAnimation.Resume(App.Current.MainWindow);
                _playing = 1;
            }
        }

        // TODO not loaded?
        void ReplayAnimation(object sender, RoutedEventArgs args)
        {
            OSAnimation.Begin(App.Current.MainWindow, true);
            _playing = 1;
        }

        private void Slider_DragCompleted(object sender, RoutedEventArgs e)
        {
            OSAnimation.Seek(App.Current.MainWindow,
                TimeSpan.FromSeconds(((Slider)sender).Value), TimeSeekOrigin.BeginTime);
            if (_playing == 3)
            {
                _playing = 1;
                OSAnimation.Resume(App.Current.MainWindow);
            }
            else 
                AnimationSlider.Value = ((Slider)sender).Value;
        }

        private void Slider_DragStarted(object sender, RoutedEventArgs e)
        {
            if (_playing==1)
            {
                _playing = 3;
                OSAnimation.Pause(App.Current.MainWindow);
            }
        }

        #endregion
        #endregion
        #endregion
    }
}