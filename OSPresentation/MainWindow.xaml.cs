using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

using static System.Linq.Enumerable;

using MaterialDesignThemes.Wpf;

using OSPresentation.TempStruct;
using OSPresentation.DataManipulation;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

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
            _stackExpanderOpen = StackAnimationExpander.IsExpanded;
            _fileExpanderOpen = FileAnimationExpander.IsExpanded;
        }

        #region Fields
        #region DataManipulation
        List<BreakPoint> breakPoints = new List<BreakPoint>();

        #endregion

        #region AnimationData
        long gtime = 0, ltime=0;
        
        int _ajeffies=-1, _apid=-1, _acounter=-1;

        int _stackPointer=-1;
        bool _stackExpanderOpen, _fileExpanderOpen;
        //TODO: store stack, queue, drawboard data here.
        List<ProcessStruct> processStructs = new List<ProcessStruct>();
        Stack<List<StackData>> stackDataList = new Stack<List<StackData>>();
        Stack<string> syscallNames = new Stack<string>();

        // global animation parameters
        const int slightDelay=5, itv0 = 200, itv1=200, itv2=1000;
        #endregion

        #region AnimationHolders
        Storyboard OSAnimation = new Storyboard();
        // Text
        StringAnimationUsingKeyFrames consoleText = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames despTextAnimation = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames tbTextAnimation = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames jeffiesText = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames pidText = new StringAnimationUsingKeyFrames();
        StringAnimationUsingKeyFrames counterText = new StringAnimationUsingKeyFrames();
        List<StringAnimationUsingKeyFrames> stackTextAnimations = 
            new List<StringAnimationUsingKeyFrames>();
        List<StringAnimationUsingKeyFrames> stackTooltipTextAnimations = 
            new List<StringAnimationUsingKeyFrames>();
        // Bool
        List<BooleanAnimationUsingKeyFrames> stackTooltipAnimations = 
            new List<BooleanAnimationUsingKeyFrames>();
        List<BooleanAnimationUsingKeyFrames> stackItemAnimations = 
            new List<BooleanAnimationUsingKeyFrames>();
        BooleanAnimationUsingKeyFrames stackExpandeAnimation;
        BooleanAnimationUsingKeyFrames fileExpanderAnimation;

        // Double

        // Color

        // Object
        List<ObjectAnimationUsingKeyFrames> stackVisibilityAnimations = 
            new List<ObjectAnimationUsingKeyFrames>();

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
                        if (bpn != 6 &&
                            bpn != 7 &&
                            bpn != 8 &&
                            bpn != 9 &&
                            bpn != 10 &&
                            bpn != 11 &&
                            bpn != 12 &&
                            bpn != 13)
                            continue;
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
            addUIElements_RegisterAnimation();



            addAnimation();
            Trace.WriteLine("Animation processing succeeds");
            intializeUI();
        }

        // The data are fixed here
        // There are some subfuncitons for dulplicate buttons
        // TODO
        void addUIElements_RegisterAnimation()
        {
            OSAnimation.Stop();
            OSAnimation.Children.Clear();
            consoleText.KeyFrames.Clear();
            stackExpandeAnimation = new BooleanAnimationUsingKeyFrames();
            fileExpanderAnimation = new BooleanAnimationUsingKeyFrames();

            // refresh process
            TaskList.Items.Clear();

            // refresh stack
            stackTooltipAnimations.Clear();
            stackItemAnimations.Clear();
            stackTooltipTextAnimations.Clear();
            stackTextAnimations.Clear();
            KernelStackList.Items.Clear();
            foreach (var index in Range(1, 18))
                addStackButton();
            _stackPointer = -1;
        }

        void addProcessButton(int i)
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

            // register Animation
            // ToolTip
            BooleanAnimationUsingKeyFrames stackTooltip = new BooleanAnimationUsingKeyFrames();
            Storyboard.SetTarget(stackTooltip, tt);
            Storyboard.SetTargetProperty(stackTooltip, new PropertyPath("IsOpen"));
            stackTooltip.FillBehavior = FillBehavior.HoldEnd;
            stackTooltipAnimations.Add(stackTooltip);

            //change tooltip content
            StringAnimationUsingKeyFrames saufTT = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(saufTT, tt);
            Storyboard.SetTargetProperty(saufTT, new PropertyPath("Content"));
            saufTT.FillBehavior = FillBehavior.HoldEnd;
            stackTooltipTextAnimations.Add(saufTT);

            // stack visible
            ObjectAnimationUsingKeyFrames oaukfStk = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(oaukfStk, stackObject);
            Storyboard.SetTargetProperty(oaukfStk, new PropertyPath("Visibility"));
            oaukfStk.FillBehavior = FillBehavior.HoldEnd;
            stackVisibilityAnimations.Add(oaukfStk);

            // select the stack
            BooleanAnimationUsingKeyFrames stackItem = new BooleanAnimationUsingKeyFrames();
            Storyboard.SetTarget(stackItem, stackObject);
            Storyboard.SetTargetProperty(stackItem, new PropertyPath("IsSelected"));
            stackItem.FillBehavior = FillBehavior.HoldEnd;
            stackItemAnimations.Add(stackItem);

            // change stack content
            StringAnimationUsingKeyFrames saufStk = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(saufStk, stackObject);
            Storyboard.SetTargetProperty(saufStk, new PropertyPath("Content"));
            saufStk.FillBehavior = FillBehavior.HoldEnd;
            stackTextAnimations.Add(saufStk);
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
                if(!string.IsNullOrEmpty(bp.Description))
                    changeDescription(bp.Description);
                if (!string.IsNullOrEmpty(bp.CodeLine))
                    changeTrace(bp.CodeLine);
                switch (bp.BPN)
                {
                    case 6:
                        BP6 bp6 = (BP6)bp;
                        changeConsole(bp6.Console);
                        break;
                    case 7:
                        BP7 bp7 = (BP7)bp;
                        emptyStack();
                        syscallNames.Push(bp7.Syscall);
                        stackDataList.Push(new List<StackData>());
                        foreach (var sd in bp7.Stacks)
                            pushStack(sd, true);
                        break;
                    case 8:
                        BP8 bp8 = (BP8)bp;
                        foreach (var sd in bp8.Stacks)
                            pushStack(sd, true);
                        break;
                    case 9:
                        BP9 bp9 = (BP9)bp;
                        foreach (var sd in bp9.Stacks)
                            pushStack(sd, true);
                        gtime += itv0;
                        break;
                    case 10:
                        BP10 bp10 = (BP10)bp;
                        foreach (var sd in bp10.Stacks)
                            pushStack(sd, true);
                        break;
                    case 11:
                        foreach (var index in Range(1, 5))
                            popStack(true);
                        break;
                    case 12:
                        BP12 bp12 = (BP12)bp;
                        emptyStack();
                        syscallNames.Push(bp12.Syscall);
                        stackDataList.Push(new List<StackData>());
                        foreach (var sd in bp12.Stacks)
                            pushStack(sd, true);
                        break;
                    case 13:
                        foreach (var index in Range(1, 6))
                            popStack(true);
                        break;
                    default:
                        throw new InvalidOperationException("invalid breakpoint added");
                        //we should crash here;
                }
                ind++;
                ltime = gtime;
            };

            addToStoryBoard();
            OSAnimation.Completed += new EventHandler(AnimationCompleted);

        }
        #region AnimationMethods

        void addToStoryBoard()
        {
            DoubleAnimation daSlider = new DoubleAnimation();
            daSlider.From = 0;
            daSlider.To = gtime / 1000;
            daSlider.Duration = TimeSpan.FromMilliseconds(gtime);
            Storyboard.SetTargetName(daSlider, AnimationSlider.Name);
            Storyboard.SetTargetProperty(daSlider, new PropertyPath("Value"));
            OSAnimation.Children.Add(daSlider);

            //Text
            Storyboard.SetTarget(consoleText, consoleBox);
            Storyboard.SetTargetProperty(consoleText, new PropertyPath("Text"));
            consoleText.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(consoleText);

            Storyboard.SetTarget(despTextAnimation, DescriptionBox);
            Storyboard.SetTargetProperty(despTextAnimation, new PropertyPath("Text"));
            despTextAnimation.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(despTextAnimation);

            Storyboard.SetTarget(tbTextAnimation, TracebackBox);
            Storyboard.SetTargetProperty(tbTextAnimation, new PropertyPath("Text"));
            tbTextAnimation.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(tbTextAnimation);

            Storyboard.SetTargetName(jeffiesText, JeffiesN.Name);
            Storyboard.SetTargetProperty(jeffiesText, new PropertyPath("Text"));
            jeffiesText.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(pidText);

            Storyboard.SetTargetName(pidText, PidN.Name);
            Storyboard.SetTargetProperty(pidText, new PropertyPath("Text"));
            pidText.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(jeffiesText);

            Storyboard.SetTargetName(counterText, CounterN.Name);
            Storyboard.SetTargetProperty(counterText, new PropertyPath("Text"));
            counterText.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(counterText);

            //Bool
            Storyboard.SetTarget(stackExpandeAnimation, StackAnimationExpander);
            Storyboard.SetTargetProperty(stackExpandeAnimation, new PropertyPath("IsExpanded"));
            stackExpandeAnimation.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(stackExpandeAnimation);

            Storyboard.SetTarget(fileExpanderAnimation, StackAnimationExpander);
            Storyboard.SetTargetProperty(fileExpanderAnimation, new PropertyPath("IsExpanded"));
            fileExpanderAnimation.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(fileExpanderAnimation);

            // Stacks 
            foreach (var baukf in stackTooltipAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in stackTooltipTextAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in stackVisibilityAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in stackItemAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in stackTextAnimations)
                OSAnimation.Children.Add(baukf);
        }

        // Text
        void changeConsole(string str)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = str;
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            consoleText.KeyFrames.Add(dsk);

            gtime += itv0;
        }
        void changeDescription(string str)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = str;
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            despTextAnimation.KeyFrames.Add(dsk);
        }
        void changeTrace(string str)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = str;
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            tbTextAnimation.KeyFrames.Add(dsk);
        }
        void HandleJeffies(int j)
        {
            //start status
            if (_ajeffies < 0)
                _ajeffies = j - 1;

            for (int i = _ajeffies; i < j;)
            {
                DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
                dsk.Value = (++i).ToString();
                dsk.KeyTime = TimeSpan.FromMilliseconds(gtime += itv0);
                jeffiesText.KeyFrames.Add(dsk);

                --_acounter;
                if (_acounter < 0)
                {
                    //TODO: recheck preocess table with the counter
                    Trace.WriteLine("counter is less then 0," + _apid + ", "
                        + _ajeffies);
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
            if (p != _apid && pnew == null)
            {
                ccounter = "???";
            }
            else if (p == _apid && (pnew == null || pnew.Counter == _acounter))
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
        // Bool
        void checkStackOpen()
        {
            if (_stackExpanderOpen)
                return;
            else
            {
                DiscreteBooleanKeyFrame dbkf = new DiscreteBooleanKeyFrame();
                dbkf.Value = true;
                dbkf.KeyTime = TimeSpan.FromMilliseconds(gtime);
                stackExpandeAnimation.KeyFrames.Add(dbkf);

                _stackExpanderOpen=true;
                gtime += itv0;
            }
        }
        void checkStackClose()
        {
            if (!_stackExpanderOpen)
                return;
            else
            {
                DiscreteBooleanKeyFrame dbkf = new DiscreteBooleanKeyFrame();
                dbkf.Value = false;
                dbkf.KeyTime = TimeSpan.FromMilliseconds(gtime);
                stackExpandeAnimation.KeyFrames.Add(dbkf);

                _stackExpanderOpen = false;
                gtime += itv0;
            }
        }
        void checkFileOpen()
        {
            if (_fileExpanderOpen)
                return;
            else
            {
                DiscreteBooleanKeyFrame dbkf = new DiscreteBooleanKeyFrame();
                dbkf.Value = true;
                dbkf.KeyTime = TimeSpan.FromMilliseconds(gtime);
                stackExpandeAnimation.KeyFrames.Add(dbkf);

                _stackExpanderOpen = true;
                gtime += itv0;
            }
        }
        void checkFileClose()
        {
            if (!_fileExpanderOpen)
                return;
            else
            {
                DiscreteBooleanKeyFrame dbkf = new DiscreteBooleanKeyFrame();
                dbkf.Value = false;
                dbkf.KeyTime = TimeSpan.FromMilliseconds(gtime);
                fileExpanderAnimation.KeyFrames.Add(dbkf);

                _fileExpanderOpen = false;
                gtime += itv0;
            }
        }
        // Stacks
        // We dont add new list within the functions below.

        // new with delay and move pointer
        // else add immediately
        // the pointer shoudld be placed at the right place
        void pushStack(StackData sd, bool isNew)
        {
            checkStackOpen();
            _stackPointer += 1;
            DiscreteStringKeyFrame name = new DiscreteStringKeyFrame();
            name.Value = sd.Register;
            name.KeyTime = TimeSpan.FromMilliseconds(gtime);
            stackTextAnimations[_stackPointer].KeyFrames.Add(name);

            DiscreteStringKeyFrame tt = new DiscreteStringKeyFrame();
            tt.Value = sd.Description;
            tt.KeyTime = TimeSpan.FromMilliseconds(gtime);
            stackTooltipTextAnimations[_stackPointer].KeyFrames.Add(tt);

            DiscreteObjectKeyFrame visibility = new DiscreteObjectKeyFrame();
            visibility.Value = Visibility.Visible;
            visibility.KeyTime = TimeSpan.FromMilliseconds(gtime);
            stackVisibilityAnimations[_stackPointer].KeyFrames.Add(visibility);
            if (isNew)
            {
                stackDataList.Peek().Add(sd);  

                DiscreteBooleanKeyFrame ttShow = new DiscreteBooleanKeyFrame();
                ttShow.Value = true;
                ttShow.KeyTime = TimeSpan.FromMilliseconds(gtime + slightDelay);
                stackTooltipAnimations[_stackPointer].KeyFrames.Add(ttShow);

                DiscreteBooleanKeyFrame selectItem = new DiscreteBooleanKeyFrame();
                selectItem.Value = true;
                selectItem.KeyTime = TimeSpan.FromMilliseconds(gtime + slightDelay);
                stackItemAnimations[_stackPointer].KeyFrames.Add(selectItem);

                gtime += itv0;
                DiscreteBooleanKeyFrame dttShow = new DiscreteBooleanKeyFrame();
                dttShow.Value = false;
                dttShow.KeyTime = TimeSpan.FromMilliseconds(gtime);
                stackTooltipAnimations[_stackPointer].KeyFrames.Add(dttShow);

                DiscreteBooleanKeyFrame dselectItem = new DiscreteBooleanKeyFrame();
                dselectItem.Value = false;
                dselectItem.KeyTime = TimeSpan.FromMilliseconds(gtime);
                stackItemAnimations[_stackPointer].KeyFrames.Add(dselectItem);
            }
        }
        void popStack(bool clear)
        {
            checkStackOpen();
            DiscreteObjectKeyFrame visibility = new DiscreteObjectKeyFrame();
            visibility.Value = Visibility.Hidden;
            visibility.KeyTime = TimeSpan.FromMilliseconds(gtime);
            stackVisibilityAnimations[_stackPointer].KeyFrames.Add(visibility);

            if (clear)
            {
                stackDataList.Peek().RemoveAt(stackDataList.Peek().Count-1);
                gtime += itv0;
            }

            _stackPointer -= 1;
        }
        // assume old stack is at the top
        void restoreStack()
        {
            foreach(var sd in stackDataList.Peek())
            {
                pushStack(sd, false);
            }
        }
        void emptyStack()
        {
            while(_stackPointer>-1)
            {
                popStack(false);
            }
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