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
using System.Numerics;

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

            buttonDefaultStyle = FindResource("MaterialDesignFlatButton") as Style;
            buttonSelectedStyle= FindResource("MaterialDesignFlatLightBgButton") as Style;
            buttonZombieStyle= FindResource("MaterialDesignOutlinedButton") as Style; 
            buttonSleepStyle = FindResource("MaterialDesignFlatDarkBgButton") as Style;
            buttonActionStyle = FindResource("MaterialDesignFlatAccentBgButton") as Style;

            List<ListView> fsl0 = new List<ListView>();
            //offset
            fsl0.Add(PcsCntList1);
            fsl0.Add(PcsFlipsList1);
            fsl0.Add(PcsFlipList1);
            fsl0.Add(PcsBHList0);
            fsl0.Add(ReqList0);
            fsLists.Add(fsl0);
            List<ListView> fsl1 = new List<ListView>();
            fsl1.Add(PcsCntList1);
            fsl1.Add(PcsFlipsList1);
            fsl1.Add(PcsFlipList1);
            fsl1.Add(PcsBHList1);
            fsl1.Add(ReqList1);
            fsLists.Add(fsl1);
            List<ListView> fsl2 = new List<ListView>();
            fsl2.Add(PcsCntList2);
            fsl2.Add(PcsFlipsList2);
            fsl2.Add(PcsFlipList2);
            fsl2.Add(PcsBHList2);
            fsl2.Add(ReqList2);
            fsLists.Add(fsl2);
        }

        #region Fields
        #region DataManipulation
        List<BreakPoint> breakPoints = new List<BreakPoint>();

        #endregion

        #region AnimationData
        long gtime = 0, ltime=0;
        
        int _ajeffies=-1, _apid=-1, _acounter=-1;
        int _tmpc = -1, _tmpnext = -1;
        int _current=-1;

        int _stackPointer=-1;
        int _taskCheckPointer = -1, _taskCounterPointer = -1;
        bool _stackExpanderOpen, _fileExpanderOpen;
        List<int> _fsStates = new List<int>();

        bool _nxFpFlag = false, _readFp = false;
        int _openScene = -1;
        //store stack, queue, drawboard data here.
        List<ProcessStruct> processStructs = new List<ProcessStruct>();
        Stack<List<StackData>> stackDataList = new Stack<List<StackData>>();
        Stack<string> syscallNames = new Stack<string>();

        // global animation parameters
        const int slightDelay=2, itv0 = 200, itv1=500, itv2=1000;
        #endregion

        #region AnimationHolders
        Storyboard OSAnimation = new Storyboard();
        // Text
        StringAnimationUsingKeyFrames consoleText;
        StringAnimationUsingKeyFrames despTextAnimation;
        StringAnimationUsingKeyFrames tbTextAnimation;
        StringAnimationUsingKeyFrames jeffiesText;
        StringAnimationUsingKeyFrames CNTextAnimation;
        StringAnimationUsingKeyFrames flTextAnimation;
        StringAnimationUsingKeyFrames pidText;
        StringAnimationUsingKeyFrames counterText;
        List<StringAnimationUsingKeyFrames> stackTextAnimations = 
            new List<StringAnimationUsingKeyFrames>();
        List<StringAnimationUsingKeyFrames> stackTooltipTextAnimations = 
            new List<StringAnimationUsingKeyFrames>();
        List<StringAnimationUsingKeyFrames> taskTooltipTextAnimations = 
            new List<StringAnimationUsingKeyFrames>();
        List<StringAnimationUsingKeyFrames> taskTextAnimations =
            new List<StringAnimationUsingKeyFrames>();
        List<StringAnimationUsingKeyFrames> bandageTextAnimations =
            new List<StringAnimationUsingKeyFrames>();
        List<List<List<StringAnimationUsingKeyFrames>>> fsListAnimations =
            new List<List<List<StringAnimationUsingKeyFrames>>>();

        // Bool
        List<BooleanAnimationUsingKeyFrames> stackTooltipAnimations = 
            new List<BooleanAnimationUsingKeyFrames>();
        List<BooleanAnimationUsingKeyFrames> stackItemAnimations = 
            new List<BooleanAnimationUsingKeyFrames>();
        List<BooleanAnimationUsingKeyFrames> taskTooltipShowAnimations =
            new List<BooleanAnimationUsingKeyFrames>();
        List<BooleanAnimationUsingKeyFrames> taskEnableAnimations =
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
        //Style 
        Style buttonDefaultStyle, buttonSelectedStyle, buttonZombieStyle, buttonSleepStyle, buttonActionStyle;
        //Buttons
        List<Button> taskButtons = new List<Button>();
        List<ListViewItem> stackButtons = new List<ListViewItem>();
        // Bandges
        List<Badged> taskBadges = new List<Badged>();
        //Tooltips
        List<ToolTip> taskToolTips=new List<ToolTip>();
        List<ToolTip> stackToolTips = new List<ToolTip>();
        // FS obj
        List<List<ListView>> fsLists = new List<List<ListView>>();
        #endregion
        #endregion

        #region Methods
        void dataManipulate(string dataFile)
        {
            addUIElements_RegisterAnimation();
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
                            bpn != 13 &&
                            bpn != 14 &&
                            bpn != 15 &&
                            bpn != 16 &&
                            bpn != 17 &&
                            bpn != 19 &&
                            bpn != 20 &&
                            bpn != 21 &&
                            bpn != 22 &&
                            bpn != 23 &&
                            bpn != 24 &&
                            bpn != 26 &&
                            bpn != 27 &&
                            bpn != 27 &&
                            bpn != 32 &&
                            bpn != 33 &&
                            bpn != 35 &&
                            bpn != 36 &&
                            bpn != 37 &&
                            bpn != 38 &&
                            bpn != 44 &&
                            bpn != 49 &&
                            bpn != 52 &&
                            bpn != 50 &&
                            bpn != 51 &&
                            bpn != 54 &&
                            bpn != 55 &&
                            bpn != 56 &&
                            bpn != 57 &&
                            bpn != 58 &&
                            bpn != 47 &&
                            bpn != 45 &&
                            bpn != 42)
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

            addAnimation();
            Trace.WriteLine("Animation processing succeeds");
            intializeUI();
        }

        // TODO
        void addUIElements_RegisterAnimation()
        {
            gtime = 0;
            ltime = 0;

            _ajeffies = -1;
            _apid = -1;
            _acounter = -1;

            _tmpc = -1;
            _tmpnext = -1;
            _current = -1;

            _stackPointer = -1;
            _taskCheckPointer = -1;
            _taskCounterPointer = -1;
            _stackExpanderOpen = StackAnimationExpander.IsExpanded;
            _fileExpanderOpen = FileAnimationExpander.IsExpanded;

            _fsStates.Clear();

            _nxFpFlag = false;
            _readFp = false;
            _openScene = -1;

            breakPoints.Clear();
            //button
            taskButtons.Clear();
            stackButtons.Clear();
            taskBadges.Clear();
            taskToolTips.Clear();
            stackToolTips.Clear();
            //Animation
            OSAnimation = new Storyboard();
            consoleText = new StringAnimationUsingKeyFrames();
            despTextAnimation = new StringAnimationUsingKeyFrames();
            tbTextAnimation = new StringAnimationUsingKeyFrames();
            CNTextAnimation = new StringAnimationUsingKeyFrames();
            flTextAnimation = new StringAnimationUsingKeyFrames();
            jeffiesText = new StringAnimationUsingKeyFrames();
            pidText = new StringAnimationUsingKeyFrames();
            counterText = new StringAnimationUsingKeyFrames();
            stackExpandeAnimation = new BooleanAnimationUsingKeyFrames();
            fileExpanderAnimation = new BooleanAnimationUsingKeyFrames();

            syscallNames.Clear();
            taskTextAnimations.Clear();
            bandageTextAnimations.Clear();

            taskTooltipShowAnimations.Clear();
            taskEnableAnimations.Clear();

            stackVisibilityAnimations.Clear();

            // refresh process
            processStructs.Clear();
            taskTooltipTextAnimations.Clear();
            processStructs.Add(new ProcessStruct(0, 0, 1, 0, 0, -1, -1, -1));
            foreach (var ind in Range(0, 63))
                processStructs.Add(null);
            TaskList.Items.Clear();
            addProcessButton(0, true);
            foreach (var index in Range(1, 63))
                addProcessButton(index);

            // Stack
            stackDataList.Clear();
            stackTooltipAnimations.Clear();
            stackItemAnimations.Clear();
            stackTooltipTextAnimations.Clear();
            stackTextAnimations.Clear();
            KernelStackList.Items.Clear();
            foreach (var index in Range(1, 18))
                addStackButton();

            _stackPointer = -1;

            // FS
            _fsStates.Clear();
            _fsStates.AddRange(Enumerable.Repeat(0, 3).ToList());
            _openScene = -1;
            fsListAnimations.Clear();
            foreach (var v1 in fsLists)
            {
                List<List<StringAnimationUsingKeyFrames>> l = 
                    new List<List<StringAnimationUsingKeyFrames>>();
                foreach (var v2 in v1)
                {
                    List<StringAnimationUsingKeyFrames> ll = new List<StringAnimationUsingKeyFrames>();
                    foreach (ListViewItem v3 in v2.Items)
                    {
                        addStringAUKF(v3, ll);
                    }
                    l.Add(ll);
                }
                fsListAnimations.Add(l);
            }
        }
        void addProcessButton(int i, bool pid0=false)
        {
            Button button = new Button();
            button.Style = buttonDefaultStyle;
            button.Content = i.ToString();
            button.IsEnabled = false;
            taskButtons.Add(button);

            var badge = new Badged();
            badge.Badge = "";
            badge.Content = button;
            taskBadges.Add(badge);

            ToolTip tt = new ToolTip();
            tt.PlacementTarget = button;
            tt.Placement = PlacementMode.Bottom;
            tt.VerticalOffset = 15;
            tt.Content = "???";
            taskToolTips.Add(tt);

            TaskList.Items.Add(badge);
            // Register Animation
            // ToolTip show
            BooleanAnimationUsingKeyFrames taskTooltip = new BooleanAnimationUsingKeyFrames();
            Storyboard.SetTarget(taskTooltip, tt);
            Storyboard.SetTargetProperty(taskTooltip, new PropertyPath("IsOpen"));
            taskTooltip.FillBehavior = FillBehavior.HoldEnd;
            taskTooltipShowAnimations.Add(taskTooltip);

            //change tooltip content
            StringAnimationUsingKeyFrames saufTT = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(saufTT, tt);
            Storyboard.SetTargetProperty(saufTT, new PropertyPath("Content"));
            saufTT.FillBehavior = FillBehavior.HoldEnd;
            taskTooltipTextAnimations.Add(saufTT);

            //change button content
            StringAnimationUsingKeyFrames saufTsk = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(saufTsk, button);
            Storyboard.SetTargetProperty(saufTsk, new PropertyPath("Content"));
            saufTsk.FillBehavior = FillBehavior.HoldEnd;
            taskTextAnimations.Add(saufTsk);

            // enable button
            BooleanAnimationUsingKeyFrames baukfTsk = new BooleanAnimationUsingKeyFrames();
            Storyboard.SetTarget(baukfTsk, button);
            Storyboard.SetTargetProperty(baukfTsk, new PropertyPath("IsEnabled"));
            baukfTsk.FillBehavior = FillBehavior.HoldEnd;
            taskEnableAnimations.Add(baukfTsk);

            // bandge animation
            StringAnimationUsingKeyFrames saufBdg = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(saufBdg, badge);
            Storyboard.SetTargetProperty(saufBdg, new PropertyPath("Badge"));
            saufBdg.FillBehavior = FillBehavior.HoldEnd;
            bandageTextAnimations.Add(saufBdg);

            if (pid0)
            {
                button.Content = "pid=0";
                button.IsEnabled = true;
                tt.Content = "state: TASK_INTERRUPTIBLE\n???: ???";
            }
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

            // Register Animation
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
        void addStringAUKF(Control c, List<StringAnimationUsingKeyFrames> sl)
        {
            StringAnimationUsingKeyFrames saufStk = new StringAnimationUsingKeyFrames();
            Storyboard.SetTarget(saufStk, c);
            Storyboard.SetTargetProperty(saufStk, new PropertyPath("Content"));
            saufStk.FillBehavior = FillBehavior.HoldEnd;
            sl.Add(saufStk);
        }
        void addAnimation()
        {
            //temp data
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
                if (!string.IsNullOrEmpty(bp.FileName))
                    changeFileName(bp.FileName);
                switch (bp.BPN)
                {
                    case 6:
                        BP6 bp6 = (BP6)bp;
                        changeConsole(bp6.Console);
                        break;
                    case 7:
                        checkFileClose();
                        checkStackOpen();
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
                        gtime += itv1;
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
                        checkFileClose();
                        checkStackOpen();
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
                        stackDataList.Pop();
                        break;
                    case 14:
                        gtime += itv1;
                        break;
                    case 15:
                        BP15 bp15 = (BP15)bp;
                        restoreStack();
                        changeStackContent(4, bp15.EIP);
                        changeStackContent(1, bp15.ESP);
                        gtime += itv1;
                        while (_stackPointer > -1)
                        {
                            popStack(true);
                        }
                        stackDataList.Pop();
                        break;
                    case 16:
                        BP16 bp16 = (BP16)bp;
                        if (bp16.Empty == -1)
                        {
                            changeCN("Empty task is " + bp16.TaskN, true);
                            DiscreteStringKeyFrame d162 = new DiscreteStringKeyFrame();
                            d162.Value = "pid=" + bp16.LastPid;
                            d162.KeyTime = TimeSpan.FromMilliseconds(gtime);
                            taskTextAnimations[bp16.TaskN].KeyFrames.Add(d162);
                        }
                        else
                        {
                            changeCN("Finding empty task");
                        }
                        changeSelectButton(bp16.TaskN);
                        DiscreteBooleanKeyFrame d161 = new DiscreteBooleanKeyFrame();
                        d161.Value = true;
                        d161.KeyTime = TimeSpan.FromMilliseconds(gtime);
                        taskEnableAnimations[bp16.TaskN].KeyFrames.Add(d161);
                        gtime += itv0;
                        if (_current != bp16.TaskN)
                            changeSelectButton(bp16.TaskN, false);
                        else
                            changeRunningTask(bp16.TaskN);
                        break;
                    case 17:
                        BP17 bp17 = (BP17)bp;
                        ProcessStruct ps = bp17.Process();
                        if (_taskCheckPointer == -1)
                            _taskCheckPointer = ps.TaskN;
                        changAndShowTask(ps);
                        break;
                    case 19:
                        BP19 bp19 = (BP19)bp;
                        if (_taskCheckPointer >= 0)
                        {
                            _taskCheckPointer = -1;
                            _tmpc = -1;
                        }
                        ProcessStruct prc19 = getProcess(bp19.ProcessPid);
                        changeCN("Changing Counter");
                        if (_taskCounterPointer < 0)
                            _taskCounterPointer = prc19.TaskN;
                        changAndShowTaskCounter(bp19.ProcessCounter, prc19.TaskN);
                        break;
                    case 20:
                        _taskCheckPointer = -1;
                        _tmpc = -1;
                        _taskCounterPointer = -1;
                        BP20 bp20 = (BP20)bp;
                        changeRunningTask(bp20.NextTask);
                        changeCN("Switching to pid:" + bp20.NextPid, true);
                        gtime += itv1;
                        HandlePid(bp20.NextPid);
                        break;
                    case 21:
                        BP21 bp21 = (BP21)bp;
                        ProcessStruct prc21 = getProcess(bp21.Pid);
                        prc21.State = 2;
                        taskChangeCounter(prc21.TaskN);
                        break;
                    case 22:
                        break;
                    case 23:
                        BP23 bp23 = (BP23)bp;
                        if (bp23.NextPid == -1) ;
                        else
                        {
                            ProcessStruct prc23 = getProcess(bp23.NextPid);
                            prc23.State = 0;
                            taskChangeCounter(prc23.TaskN);
                        }
                        break;
                    case 24:
                        BP24 bp24 = (BP24)bp;
                        taskDisable(bp24.TaskN);
                        gtime += itv2;
                        break;
                    case 26:
                        gtime += itv1;
                        break;
                    case 27:
                        BP27 bp27 = (BP27)bp;
                        ProcessStruct prc27 = getProcess(bp27.Pid);
                        prc27.State = 1;
                        taskChangeCounter(prc27.TaskN);
                        gtime += itv2;
                        break;
                    case 32:
                        BP32 bp32 = (BP32)bp;
                        ProcessStruct prc32 = getProcess(_apid);
                        prc32.State = 3;
                        taskChangeCounter(prc32.TaskN);
                        gtime += itv0;
                        prc32 = getProcess(bp32.FatherPid);
                        prc32.Signal = bp32.FatherSignal;
                        taskChangeCounter(prc32.TaskN);
                        gtime += itv2;
                        break;
                    // sys_read()
                    case 33:
                        BP33 bp33 = (BP33)bp;
                        ProcessStruct prc33 = getProcess(bp33.Pid);
                        int id33 = bp33.Pid == 11 ? 1 : bp33.Pid == 10 ? 2 : 0;
                        if (bp33.Pid == 11)
                        {
                            showFPButton(id33);
                            showScene(id33);
                            checkStackClose();
                            checkFileOpen();
                            if (_fsStates[id33] == 0)
                            {
                                setFSOpacity(fsObj101);
                                gtime += itv1;
                                setFSListString(id33, 0, 0, "pid=" + prc33.Pid);
                                setFSListString(id33, 0, 1, "flips");
                                setFSListString(id33, 0, 2, "state=" + prc33.State);
                                setFSListString(id33, 0, 3, "counter=" + prc33.Counter);
                                setFSListString(id33, 0, 4, "priority=" + prc33.Priority);
                                setFSListString(id33, 0, 5, "father=" + prc33.Father);
                                setFSListString(id33, 0, 6, "exitcode=" + prc33.ExitCode);
                                setFSListString(id33, 0, 7, "signal=" + prc33.Signal);
                                gtime += itv1;
                                setFSOpacity(fsObj102);
                                gtime += itv1;
                                setFSOpacity(fsObj103);
                                _fsStates[id33] = 1;
                            }
                            else
                                ;

                            setFSListString(id33, 1, bp33.FD, "flip[" + bp33.FD + "]");
                            gtime += itv1;
                            if (bp33.IsNew)
                                gtime += itv2;
                        }
                        else if(bp33.Pid == 10)
                        {
                            showFPButton(id33);
                            showScene(id33);
                            checkStackClose();
                            checkFileOpen();
                            if (_fsStates[id33] == 0)
                            {
                                setFSOpacity(fsObj201);
                                gtime += itv1;
                                setFSListString(id33, 0, 0, "pid=" + prc33.Pid);
                                setFSListString(id33, 0, 1, "flips");
                                setFSListString(id33, 0, 2, "state=" + prc33.State);
                                setFSListString(id33, 0, 3, "counter=" + prc33.Counter);
                                setFSListString(id33, 0, 4, "priority=" + prc33.Priority);
                                setFSListString(id33, 0, 5, "father=" + prc33.Father);
                                setFSListString(id33, 0, 6, "exitcode=" + prc33.ExitCode);
                                setFSListString(id33, 0, 7, "signal=" + prc33.Signal);
                                gtime += itv1;
                                setFSOpacity(fsObj202);
                                gtime += itv1;
                                setFSOpacity(fsObj203);
                                _fsStates[id33] = 1;
                            }
                            else
                                ;

                            setFSListString(id33, 1, bp33.FD, "flip[" + bp33.FD + "]");
                            gtime += itv1;
                            if (bp33.IsNew)
                                gtime += itv2;
                        }
                        else
                        {
                            Trace.WriteLine("33," + bp33.Pid);
                        }
                        break;
                    case 35:
                        BP35 bp35 = (BP35)bp;
                        int id35 = bp35.Pid == 11 ? 1 : bp35.Pid == 10 ? 2 : 0;
                        if(_apid==11)
                        {
                            showScene(id35);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj104);
                            gtime += itv1;
                            setFSOpacity(fsObj105);
                            gtime += itv1;
                            setFSListString(id35, 2, 0, bp35.Mode);
                            gtime += itv1;
                            setFSListString(id35, 2, 1, bp35.Flags);
                            gtime += itv1;
                            setFSListString(id35, 2, 2, bp35.Count);
                            gtime += itv1;
                            setFSListString(id35, 2, 3, bp35.Inode);
                            gtime += itv1;
                            setFSListString(id35, 2, 4, bp35.Pos);
                            gtime += itv1;
                            _fsStates[id35] = 2;
                        }
                        else if(_apid == 10)
                        {
                            showScene(id35);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj204);
                            gtime += itv1;
                            setFSOpacity(fsObj205);
                            gtime += itv1;
                            setFSListString(id35, 2, 0, bp35.Mode);
                            gtime += itv1;
                            setFSListString(id35, 2, 1, bp35.Flags);
                            gtime += itv1;
                            setFSListString(id35, 2, 2, bp35.Count);
                            gtime += itv1;
                            setFSListString(id35, 2, 3, bp35.Inode);
                            gtime += itv1;
                            setFSListString(id35, 2, 4, bp35.Pos);
                            gtime += itv1;
                            _fsStates[id35] = 2;
                        }
                        else
                        {
                            Trace.WriteLine("35," + bp35.Pid);
                        };
                        break;
                    case 36:
                        if (_apid == 11)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            setCardSelected(fsObj109);
                            setFSOpacity(fsObj109);
                            gtime += itv1;
                            setFSOpacity(fsObj110);
                            setFSOpacity(fsObj119);
                            gtime += itv1;
                            setCardSelected(fsObj109, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj111);
                            setFSOpacity(fsObj111);
                            gtime += itv1;
                        }
                        else if(_apid == 10)
                        {
                            showScene(2);
                            checkStackClose();
                            checkFileOpen();
                            setCardSelected(fsObj209);
                            setFSOpacity(fsObj209);
                            gtime += itv1;
                            setFSOpacity(fsObj210);
                            setFSOpacity(fsObj219);
                            gtime += itv1;
                            setCardSelected(fsObj209, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj211);
                            setFSOpacity(fsObj211);
                            gtime += itv1;
                        }
                        else
                        {
                            Trace.WriteLine("36," + _apid);
                        }
                        break;
                    case 37:
                        gtime += itv1;
                        break;
                    case 38:
                        gtime += itv1;
                        break;
                    case 42:
                        BP42 bp42 = (BP42)bp;
                        if (_apid == 11)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            fsObj149.Text = bp42.Buffer;
                            setFSOpacity(fsObj149);
                            setFSOpacity(fsObj143);
                            gtime += itv1;
                            setCardSelected(fsObj113, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj111);
                            setFSOpacity(fsObj144);
                            gtime += itv1;
                            setCardSelected(fsObj111, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj109);
                        }
                        else if (_apid == 10)
                        {
                            showScene(2);
                            checkStackClose();
                            checkFileOpen();
                            fsObj249.Text = bp42.Buffer;
                            setFSOpacity(fsObj249);
                            setFSOpacity(fsObj243);
                            gtime += itv1;
                            setCardSelected(fsObj213, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj211);
                            setFSOpacity(fsObj244);
                            gtime += itv1;
                            setCardSelected(fsObj211, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj209);
                        }
                        else
                        {
                            Trace.WriteLine("42," + _apid);
                        }
                        break;
                    // bread()
                    case 44:
                        BP44 bp44 = (BP44)bp;
                        if (_apid == 11)
                        {
                            showScene(1);
                            fsObj121.Text = "dev=" + bp44.Dev + " block=" + bp44.Block;
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj150);
                            setFSOpacity(fsObj151);
                            gtime += itv1;
                            setFSOpacity(fsObj123);
                            gtime += itv1;
                            setFSOpacity(fsObj121);
                            setFSOpacity(fsObj124);
                            gtime += itv1;
                            setFSOpacity(fsObj112);
                            gtime += itv1;
                            setCardSelected(fsObj111, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj113);
                            setFSOpacity(fsObj113);
                            gtime += itv1;
                            setFSOpacity(fsObj130);
                            gtime += itv1;
                            setFSOpacity(fsObj135);
                            gtime += itv1;
                            setFSOpacity(fsObj135, false);
                            gtime += itv1;
                        }
                        else if (_apid == 10)
                        {
                            showScene(2);
                            fsObj221.Text = "dev=" + bp44.Dev + " block=" + bp44.Block;
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj250);
                            setFSOpacity(fsObj251);
                            gtime += itv1;
                            setFSOpacity(fsObj223);
                            gtime += itv1;
                            setFSOpacity(fsObj221);
                            setFSOpacity(fsObj224);
                            gtime += itv1;
                            setFSOpacity(fsObj212);
                            gtime += itv1;
                            setCardSelected(fsObj211, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj213);
                            setFSOpacity(fsObj213);
                            gtime += itv1;
                            setFSOpacity(fsObj230);
                            gtime += itv1;
                            setFSOpacity(fsObj235);
                            gtime += itv1;
                            setFSOpacity(fsObj235, false);
                            gtime += itv1;
                        }
                        else if (_apid == 9)
                        {
                            showFPButton(0);
                            showScene(0);
                            fsObj025.Text= "  block:"+ bp44.Block;
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj023);
                            gtime += itv1;
                            setFSOpacity(fsObj024);
                            gtime += itv1;
                            setCardSelected(fsObj013);
                            setFSOpacity(fsObj013);
                            gtime += itv1;
                            setFSOpacity(fsObj030);
                            gtime += itv1;
                            setFSOpacity(fsObj035);
                            gtime += itv1;
                            setFSOpacity(fsObj035, false);
                            gtime += itv1;
                        }
                        else
                        {
                            Trace.WriteLine("44," + _apid);
                        }
                        break;
                    case 45:
                        BP45 bp45 = (BP45)bp;
                        if (_apid == 11)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj135);
                            gtime += itv1;
                            setFSOpacity(fsObj135, false);
                            gtime += itv1;
                        }
                        else if (_apid == 10)
                        {
                            showScene(2);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj235);
                            gtime += itv1;
                            setFSOpacity(fsObj235, false);
                            gtime += itv1;
                            
                        }
                        else if (_apid == 9)
                        {
                            showScene(0);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj035);
                            gtime += itv1;
                            setFSOpacity(fsObj035, false);
                            gtime += itv1;
                            // end request, close.
                            checkFileClose();
                        }
                        else
                        {
                            Trace.WriteLine("45," + _apid);
                        }
                        break;
                    case 47:
                        BP47 bp47 = (BP47)bp;
                        if (_apid == 11)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj140);
                            gtime += itv1;
                            setCardSelected(fsObj117, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj115);
                            setFSOpacity(fsObj141);
                            gtime += itv1;
                            setCardSelected(fsObj115, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj113);
                            setFSOpacity(fsObj142);
                            gtime += itv1;
                            setFSOpacity(fsObj135);
                            gtime += itv1;
                            setFSOpacity(fsObj135, false);
                            gtime += itv1;
                            showQButton(DiskQ3);
                        }
                        else if(_apid == 10)
                        {
                            showScene(2);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj240);
                            gtime += itv1;
                            setCardSelected(fsObj217, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj215);
                            setFSOpacity(fsObj241);
                            gtime += itv1;
                            setCardSelected(fsObj215, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj213);
                            setFSOpacity(fsObj242);
                            gtime += itv1;
                            setFSOpacity(fsObj235);
                            gtime += itv1;
                            setFSOpacity(fsObj235, false);
                            gtime += itv1;
                            showQButton(DiskQ2);
                            // wait for interruption
                            checkFileClose();
                            
                        }
                        else if (_apid == 9)
                        {
                            showScene(0);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj040);
                            gtime += itv1;
                            setCardSelected(fsObj017, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj015);
                            setFSOpacity(fsObj041);
                            gtime += itv1;
                            setCardSelected(fsObj015, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj013);
                            setFSOpacity(fsObj042);
                            gtime += itv1;
                            setFSOpacity(fsObj035);
                            gtime += itv1;
                            setFSOpacity(fsObj035, false);
                            gtime += itv1;
                            showQButton(DiskQ1);
                        }
                        else
                        {
                            Trace.WriteLine("47," + _apid);
                        }
                        break;
                    case 49:
                        BP49 bp49 = (BP49)bp;
                        int id49 = bp49.Pid == 11 ? 1 : bp49.Pid == 10 ? 2 : 0;
                        if (_apid == 11)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj131);
                            gtime += itv1;
                            setFSOpacity(fsObj114);
                            gtime += itv1;
                            setCardSelected(fsObj113, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj115);
                            setFSOpacity(fsObj115);
                            gtime += itv1;
                            setFSOpacity(fsObj136);
                            gtime += itv1;
                            setFSListString(id49, 3, 1, "b_lock=1");
                            gtime += itv0;
                            setFSOpacity(fsObj136, false);
                            gtime += itv1;
                        }
                        else if (_apid == 10)
                        {
                            showScene(2);
                            checkStackClose();
                            checkFileOpen();
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj231);
                            gtime += itv1;
                            setFSOpacity(fsObj214);
                            gtime += itv1;
                            setCardSelected(fsObj213, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj215);
                            setFSOpacity(fsObj215);
                            gtime += itv1;
                            setFSOpacity(fsObj236);
                            gtime += itv1;
                            setFSListString(id49, 3, 1, "b_lock=1");
                            gtime += itv0;
                            setFSOpacity(fsObj236, false);
                            gtime += itv1;
                        }
                        else if (_apid == 9)
                        {
                            showScene(0);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj031);
                            gtime += itv1;
                            setFSOpacity(fsObj014);
                            gtime += itv1;
                            setCardSelected(fsObj013, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj015);
                            setFSOpacity(fsObj015);
                            gtime += itv1;
                            setFSOpacity(fsObj036);
                            gtime += itv1;
                            setFSListString(id49, 3, 1, "b_lock=1");
                            gtime += itv0;
                            setFSOpacity(fsObj036, false);
                            gtime += itv1;
                        }
                        else
                        {
                            Trace.WriteLine("49," + _apid);
                        }    
                        
                        break;
                    case 50:
                        BP50 bp50 = (BP50)bp;
                        if (_apid == 11)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj116);
                            gtime += itv1;
                            setCardSelected(fsObj115, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj117);
                            setFSOpacity(fsObj117);
                            gtime += itv1;
                        }
                        else if(_apid==9)
                        {
                            showScene(0);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj016);
                            gtime += itv1;
                            setCardSelected(fsObj015, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj017);
                            setFSOpacity(fsObj017);
                            gtime += itv1;
                        }
                        else
                            Trace.WriteLine("50," + _apid);
                        break;
                    case 51:
                        if (_apid == 10)
                        {
                            showScene(2);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj216);
                            gtime += itv1;
                            setCardSelected(fsObj215, false);
                            gtime += itv0 / 2;
                            setCardSelected(fsObj217);
                            setFSOpacity(fsObj217);
                            gtime += itv1;
                            setFSOpacity(fsObj218);
                            gtime += itv1;
                        }
                        else
                            Trace.WriteLine("51," + _apid);
                        break;
                    case 52:
                        BP52 bp52 = (BP52)bp;
                        int id52 = bp52.Pid == 11 ? 1 : bp52.Pid == 10 ? 2 : 0;
                        if (_apid == 11)
                        {
                            showScene(id52);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj132);
                            gtime += itv0;
                            setFSListString(id52, 4, 0, bp52.Dev);
                            gtime += itv0;
                            setFSListString(id52, 4, 1, bp52.Cmd);
                            gtime += itv0;
                            setFSListString(id52, 4, 2, bp52.Errors);
                            gtime += itv0;
                            setFSListString(id52, 4, 3, bp52.Sector);
                            gtime += itv0;
                            setFSListString(id52, 4, 4, bp52.Nr_sectors);
                            gtime += itv0;
                            setFSListString(id52, 4, 5, bp52.Buffer);
                            gtime += itv0;
                            setFSListString(id52, 4, 6, bp52.Waiting);
                            gtime += itv0;
                            setFSListString(id52, 4, 7, bp52.Bh);
                            gtime += itv0;
                            setFSListString(id52, 4, 8, bp52.Next);
                            gtime += itv0;
                            setFSOpacity(fsObj133);
                            setFSOpacity(fsObj134);
                            gtime += itv1;
                        }
                        else if(_apid == 10)
                        {
                            showScene(id52);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj232);
                            gtime += itv0;
                            setFSListString(id52, 4, 0, bp52.Dev);
                            gtime += itv0;
                            setFSListString(id52, 4, 1, bp52.Cmd);
                            gtime += itv0;
                            setFSListString(id52, 4, 2, bp52.Errors);
                            gtime += itv0;
                            setFSListString(id52, 4, 3, bp52.Sector);
                            gtime += itv0;
                            setFSListString(id52, 4, 4, bp52.Nr_sectors);
                            gtime += itv0;
                            setFSListString(id52, 4, 5, bp52.Buffer);
                            gtime += itv0;
                            setFSListString(id52, 4, 6, bp52.Waiting);
                            gtime += itv0;
                            setFSListString(id52, 4, 7, bp52.Bh);
                            gtime += itv0;
                            setFSListString(id52, 4, 8, bp52.Next);
                            gtime += itv0;
                            setFSOpacity(fsObj233);
                            setFSOpacity(fsObj234);
                            gtime += itv1;
                        }
                        else if (_apid == 9)
                        {
                            showScene(id52);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj032);
                            gtime += itv0;
                            setFSListString(id52, 4, 0, bp52.Dev);
                            gtime += itv0;
                            setFSListString(id52, 4, 1, bp52.Cmd);
                            gtime += itv0;
                            setFSListString(id52, 4, 2, bp52.Errors);
                            gtime += itv0;
                            setFSListString(id52, 4, 3, bp52.Sector);
                            gtime += itv0;
                            setFSListString(id52, 4, 4, bp52.Nr_sectors);
                            gtime += itv0;
                            setFSListString(id52, 4, 5, bp52.Buffer);
                            gtime += itv0;
                            setFSListString(id52, 4, 6, bp52.Waiting);
                            gtime += itv0;
                            setFSListString(id52, 4, 7, bp52.Bh);
                            gtime += itv0;
                            setFSListString(id52, 4, 8, bp52.Next);
                            gtime += itv0;
                            setFSOpacity(fsObj033);
                            setFSOpacity(fsObj034);
                            gtime += itv1;
                        }
                        else
                        {
                            Trace.WriteLine("52," + _apid);
                        }
                        break;
                    case 54:
                        BP54 bp54 = (BP54)bp;
                        if(_apid == 11 && _fsStates[2] == 0)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj118);
                            gtime += itv1;
                            setButtonRunning();
                            gtime += itv1;
                        }
                        else if (_fsStates[2] > 0 && _fsStates[1] > 0)
                        {
                            if (_nxFpFlag)
                            {
                                showScene(1);
                                checkStackClose();
                                checkFileOpen();
                                setFSOpacity(fsObj147);
                                gtime += itv2;
                                setButtonRunning();
                                gtime += itv1;
                                _nxFpFlag = false;
                            }
                            else
                            {
                                Trace.WriteLine("54, error");
                            }
                        }
                        else if (_apid == 9)
                        {
                            showScene(0);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj018);
                            gtime += itv1;
                            setButtonRunning();
                            gtime += itv1;
                        }
                        else
                        {
                            Trace.WriteLine("54," + _apid);
                        }
                        break;
                    case 55:
                        BP55 bp55 = (BP55)bp;
                        if (_apid == 0 && _fsStates[1] > 0)
                        {
                            showScene(1);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj145);
                            setFSOpacity(fsObj146);
                            gtime += itv2;
                            setButtonRunning(false);
                            gtime += itv1;
                            setFSListString(1, 3, 0, "b_uptodate=1");
                            gtime += itv1;
                            setFSListString(1, 3, 1, "b_lock=0");
                            gtime += itv1;
                            showQButton(DiskQ3,false);
                        }
                        else if(_apid == 0)
                        {
                            showScene(0);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj045);
                            setFSOpacity(fsObj046);
                            gtime += itv2;
                            setButtonRunning(false);
                            gtime += itv1;
                            setFSListString(0, 3, 0, "b_uptodate=1");
                            gtime += itv1;
                            setFSListString(0, 3, 1, "b_lock=0");
                            gtime += itv1;
                            showQButton(DiskQ1, false);
                        }
                        else if(_apid == 11)
                        {
                            showScene(2);
                            checkStackClose();
                            checkFileOpen();
                            setFSOpacity(fsObj145);
                            setFSOpacity(fsObj146);
                            gtime += itv2;
                            setButtonRunning(false);
                            gtime += itv1;
                            setFSListString(2, 3, 0, "b_uptodate=1");
                            gtime += itv1;
                            setFSListString(2, 3, 1, "b_lock=0");
                            gtime += itv1;
                            showQButton(DiskQ2, false);
                        }
                        else
                        {
                            Trace.WriteLine("55," + _apid);
                        }
                        break;
                    case 56:
                        gtime += itv2;
                        break;
                    case 57:
                        gtime += itv2;
                        break;
                    case 58:
                        BP58 bp58 = (BP58)bp;
                        gtime += itv2;
                        if (bp58.Next != "0x0")
                            _nxFpFlag = true;
                        break;
                    default:
                        throw new InvalidOperationException("invalid breakpoint added");
                        //we should crash here;
                }
                changeFileName(bp.FileName, false);
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

            Storyboard.SetTarget(CNTextAnimation, CNText);
            Storyboard.SetTargetProperty(CNTextAnimation, new PropertyPath("Text"));
            CNTextAnimation.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(CNTextAnimation);

            Storyboard.SetTarget(flTextAnimation, FLText);
            Storyboard.SetTargetProperty(flTextAnimation, new PropertyPath("Text"));
            flTextAnimation.FillBehavior = FillBehavior.HoldEnd;
            OSAnimation.Children.Add(flTextAnimation);

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

            Storyboard.SetTarget(fileExpanderAnimation, FileAnimationExpander);
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
            //Tasks

            foreach (var baukf in bandageTextAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in taskTooltipShowAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in taskTooltipTextAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in taskTextAnimations)
                OSAnimation.Children.Add(baukf);
            foreach (var baukf in taskEnableAnimations)
                OSAnimation.Children.Add(baukf);
            // FS
            foreach (var v1 in fsListAnimations)
            {
                foreach (var v2 in v1)
                {
                    foreach (var v3 in v2)
                    {
                        OSAnimation.Children.Add(v3);
                    }
                }
            }
        }

        // Text
        void changeConsole(string str)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = str;
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            consoleText.KeyFrames.Add(dsk);

            gtime += itv1;
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
        void changeCN(string str, bool clear=false)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = str;
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            CNTextAnimation.KeyFrames.Add(dsk);
            if (clear)
            {
                DiscreteStringKeyFrame dsk2 = new DiscreteStringKeyFrame();
                dsk2.Value = "";
                dsk2.KeyTime = TimeSpan.FromMilliseconds(gtime += itv1);

                CNTextAnimation.KeyFrames.Add(dsk2);
            }

        }
        void changeTL(bool clear=false)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = clear?"":"Floppy Loading...";
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            flTextAnimation.KeyFrames.Add(dsk);
        }
        void HandleJeffies(int j)
        {
            //start status
            if (_ajeffies < 0)
                _ajeffies = j - 1;

            for (int i = _ajeffies; i < j;)
            {
                
                if (_acounter <= 0)
                {
                    //NOT TODO: recheck preocess table with the counter
                    // This is because the data is not full
                    // ignore it
                    Trace.WriteLine("counter is less then 0," + _apid + ", "
                        + _ajeffies);
                    break;
                }
                --_acounter;
                DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
                dsk.Value = (++i).ToString();
                dsk.KeyTime = TimeSpan.FromMilliseconds(gtime += itv0);
                jeffiesText.KeyFrames.Add(dsk);

                DiscreteStringKeyFrame dsk2 = new DiscreteStringKeyFrame();
                dsk2.Value = (_acounter).ToString();
                dsk2.KeyTime = TimeSpan.FromMilliseconds(gtime);

                counterText.KeyFrames.Add(dsk2);

            }
            if (_acounter < 0)
            {
                DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
                dsk.Value = j.ToString();
                dsk.KeyTime = TimeSpan.FromMilliseconds(gtime += itv0);
                jeffiesText.KeyFrames.Add(dsk);
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
                _acounter = pnew.Counter;
                ccounter = pnew.Counter.ToString();
            }

            _apid = p;
            DiscreteStringKeyFrame dsk2 = new DiscreteStringKeyFrame();
            dsk2.Value = ccounter;
            dsk2.KeyTime = TimeSpan.FromMilliseconds(gtime);
            counterText.KeyFrames.Add(dsk2);
#nullable restore
        }
        void changeFileName(string str, bool sel=true)
        {
            TreeViewItem it;
            if(str == "floppy.c")
                it = TVFloppy;
            else if (str == "blk.h")
                it = TVBlk;
            else if (str == "ll_rw_blk.c")
                it = TVLl_rw_blk;
            else if (str == "tty_io.c")
                it = TVTtyio;
            else if (str == "fork.c")
                it = TVFork;
            else if (str == "sched.c")
                it = TVSched;
            else if (str == "system_call.s")
                it = TVSystem_call;
            else if (str == "exit.c")
                it = TVExit;
            else if (str == "exec.c")
                it = TVExec;
            else if (str == "buffer.c")
                it = TVBuffer;
            else if (str == "file_dev.c")
                it = TVFile_dev;
            else if (str == "char_dev.c")
                it = TVChar_dev;
            else if (str == "read_write.c")
                it = TVRead_write;
            else if (str == "open.c")
                it = TVOpen;
            else
            {
                Trace.WriteLine(str + ", unknown file.");
                return;
            }
            BooleanAnimationUsingKeyFrames oa = new BooleanAnimationUsingKeyFrames();
            Storyboard.SetTarget(oa, it);
            Storyboard.SetTargetProperty(oa, new PropertyPath("IsSelected"));
            DiscreteBooleanKeyFrame dok = new DiscreteBooleanKeyFrame(sel,TimeSpan.FromMilliseconds(gtime));
            oa.KeyFrames.Add(dok);
            OSAnimation.Children.Add(oa);
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
                gtime += itv1;
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
                gtime += itv1;
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
                fileExpanderAnimation.KeyFrames.Add(dbkf);

                _fileExpanderOpen = true;
                gtime += itv2;
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
                gtime += itv1;
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
        // assume old stack is at the top of the list
        void restoreStack()
        {
            if (_stackPointer >= 0)
                return;
            foreach(var sd in stackDataList.Peek())
            {
                pushStack(sd, false);
            }
            gtime += slightDelay;
        }
        void emptyStack()
        {
            while(_stackPointer>-1)
            {
                popStack(false);
            }
        }
        void changeStackContent(int index, string content)
        {
            var sd = stackDataList.Peek()[index];
            sd.Content = content;

            DiscreteBooleanKeyFrame ttShow = new DiscreteBooleanKeyFrame();
            ttShow.Value = true;
            ttShow.KeyTime = TimeSpan.FromMilliseconds(gtime + slightDelay);
            stackTooltipAnimations[index].KeyFrames.Add(ttShow);

            DiscreteBooleanKeyFrame selectItem = new DiscreteBooleanKeyFrame();
            selectItem.Value = true;
            selectItem.KeyTime = TimeSpan.FromMilliseconds(gtime + slightDelay);
            stackItemAnimations[index].KeyFrames.Add(selectItem);

            gtime += itv0;
            DiscreteStringKeyFrame tt = new DiscreteStringKeyFrame();
            tt.Value = sd.Description;
            tt.KeyTime = TimeSpan.FromMilliseconds(gtime);
            stackTooltipTextAnimations[index].KeyFrames.Add(tt);

            gtime += itv1;

            DiscreteBooleanKeyFrame dttShow = new DiscreteBooleanKeyFrame();
            dttShow.Value = false;
            dttShow.KeyTime = TimeSpan.FromMilliseconds(gtime);
            stackTooltipAnimations[index].KeyFrames.Add(dttShow);

            DiscreteBooleanKeyFrame dselectItem = new DiscreteBooleanKeyFrame();
            dselectItem.Value = false;
            dselectItem.KeyTime = TimeSpan.FromMilliseconds(gtime);
            stackItemAnimations[index].KeyFrames.Add(dselectItem);
        }
        // Tasks
        // We refresh task simutaneously
        void taskChangeFocus(int idx, ProcessStruct ps)
        {
#nullable enable
            ProcessStruct? prcs = processStructs[idx];

            if (ps.Counter > _tmpc && ps.State==0)
            {
                changeCN(ps.PrintGreaterCounter(_tmpc, _tmpnext));
                _tmpc = ps.Counter;
                _tmpnext = ps.TaskN;
            }

            if (prcs == null || prcs.PidContent != ps.PidContent)
            {
                DiscreteStringKeyFrame d3 = new DiscreteStringKeyFrame();
                d3.Value = ps.PidContent;
                d3.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskTextAnimations[idx].KeyFrames.Add(d3);
            }

            if (prcs == null || prcs.Pid == -1)
            {
                DiscreteBooleanKeyFrame d = new DiscreteBooleanKeyFrame();
                d.Value = true;
                d.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskEnableAnimations[idx].KeyFrames.Add(d);
            }

            changeSelectButton(idx);

            if (prcs == null || prcs.ToolTipContent != ps.ToolTipContent || ps.Counter > _tmpc && ps.State == 0)
            {
                DiscreteBooleanKeyFrame d1 = new DiscreteBooleanKeyFrame();
                d1.Value = true;
                d1.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskTooltipShowAnimations[idx].KeyFrames.Add(d1);

                gtime += itv0;
                DiscreteStringKeyFrame d = new DiscreteStringKeyFrame();
                d.Value = ps.ToolTipContent;
                d.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskTooltipTextAnimations[idx].KeyFrames.Add(d);

                gtime += itv1;
                DiscreteBooleanKeyFrame d2 = new DiscreteBooleanKeyFrame();
                d2.Value = false;
                d2.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskTooltipShowAnimations[idx].KeyFrames.Add(d2);
            }
            else
            {
                gtime += itv0;
            }

            if (_current != idx)
                changeSelectButton(idx, false);
            else
                changeRunningTask(idx);
            changeTaskBandge(idx, ps.State);

            processStructs[idx] = ps;
        }
        void taskDisable(int idx)
        {
            ProcessStruct? prcs = processStructs[idx];

            changeSelectButton(idx);
            
            if (prcs == null || prcs.Pid == -1) ;
            else
            {
                DiscreteStringKeyFrame d2 = new DiscreteStringKeyFrame();
                d2.Value = "???";
                d2.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskTooltipTextAnimations[idx].KeyFrames.Add(d2);
                DiscreteStringKeyFrame d3 = new DiscreteStringKeyFrame();
                d3.Value = idx.ToString();
                d3.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskTextAnimations[idx].KeyFrames.Add(d3);

                DiscreteBooleanKeyFrame d = new DiscreteBooleanKeyFrame();
                d.Value = false;
                d.KeyTime = TimeSpan.FromMilliseconds(gtime);
                taskEnableAnimations[idx].KeyFrames.Add(d);
            }

            if (prcs == null || prcs.Pid == -1) ;
            else
            {
                changeTaskBandge(idx);
            }
            gtime += itv0;
            changeSelectButton(idx, false);

            if (prcs == null)
                prcs = new ProcessStruct(idx);
            else
                prcs.SetToDefault();
#nullable restore
        }
        void taskChangeCounter(int idx)
        {
            if (processStructs[idx] == null || processStructs[idx].Pid == -1)
            {
                Trace.WriteLine("Target process is empty when changing process.");
                return;
            }
            changeSelectButton(idx);

            DiscreteBooleanKeyFrame d1 = new DiscreteBooleanKeyFrame();
            d1.Value = true;
            d1.KeyTime = TimeSpan.FromMilliseconds(gtime);
            taskTooltipShowAnimations[idx].KeyFrames.Add(d1);

            gtime += itv0;
            
            DiscreteStringKeyFrame d = new DiscreteStringKeyFrame();
            d.Value = processStructs[idx].ToolTipContent;
            d.KeyTime = TimeSpan.FromMilliseconds(gtime);
            taskTooltipTextAnimations[idx].KeyFrames.Add(d);

            gtime += itv1;
            DiscreteBooleanKeyFrame d2 = new DiscreteBooleanKeyFrame();
            d2.Value = false;
            d2.KeyTime = TimeSpan.FromMilliseconds(gtime);
            taskTooltipShowAnimations[idx].KeyFrames.Add(d2);

            if (_current != idx)
                changeSelectButton(idx, false);
            else
                changeRunningTask(idx);
            changeTaskBandge(idx, processStructs[idx].State);
        }
        void changAndShowTask(ProcessStruct ps, int index=-1)
        {
            while (index >= 0 && _taskCheckPointer > index)
            {
                taskDisable(_taskCheckPointer);
                _taskCheckPointer--;
            }

            if(_taskCheckPointer >= 0&&(_taskCheckPointer == index||index<0))
            {
                taskChangeFocus(_taskCheckPointer,ps);
                _taskCheckPointer--;
            }
        }
        void changeRunningTask(int idx)
        {
            if (_current<0||_current==idx);
            else
            {
                changeTaskButton(_current, false);
            }
            changeTaskButton(idx,true);
            _current = idx;
        }
        void changAndShowTaskCounter(int c, int index = -1)
        {
            while (index >= 0 && _taskCounterPointer > index)
            {
                taskDisable(_taskCounterPointer);
                _taskCounterPointer--;
            }

            if (_taskCounterPointer >= 0 && (_taskCounterPointer == index || index < 0))
            {
                processStructs[_taskCounterPointer].Counter = c;
                taskChangeCounter(_taskCounterPointer);
                _taskCounterPointer--;
            }
        }
        void changeBandge(int idx, string str)
        {
            DiscreteStringKeyFrame v = new DiscreteStringKeyFrame();
            v.Value =  str;
            v.KeyTime = TimeSpan.FromMilliseconds(gtime);
            bandageTextAnimations[idx].KeyFrames.Add(v);
        }
        void changeTaskBandge(int idx,int state=-1)
        {
            string str= state == 1 ? "INTERRUPTIBLE" : (state == 2 ? "UNINTERRUPTIBLE" : (state == 3 ? "ZOMBIE" : (state == 4 ? "STOPPED" : "")));
            changeBandge(idx, str);
        }
        ColorAnimation buttonForeBackgroundAnimation(Button button, bool changeTo = true, bool isFore = false, bool selected=false)
        {
            ColorAnimation cc = new ColorAnimation();
            if (changeTo && !isFore && !selected)
                cc.To = Color.FromArgb(0xff, 0xae, 0xea, 00);
            else if (!changeTo && !isFore )
                cc.To = Colors.Transparent;
            else if (changeTo && isFore && !selected)
                cc.To = Colors.Black;
            else if (!changeTo && isFore || selected && changeTo && !isFore)
                cc.To = Color.FromArgb(0xff, 0x67, 0x3a, 0xb7);
            else if (selected && changeTo && isFore)
                cc.To = Colors.White;


            cc.Duration = TimeSpan.FromMilliseconds(itv0/2);
            cc.BeginTime = TimeSpan.FromMilliseconds(gtime);
            Storyboard.SetTarget(cc, button);
            if (isFore)
                Storyboard.SetTargetProperty(cc, new PropertyPath("(Foreground).(SolidColorBrush.Color)"));
            else
                Storyboard.SetTargetProperty(cc, new PropertyPath("(Background).(SolidColorBrush.Color)"));
            OSAnimation.Children.Add(cc);
            return cc;
        }
        void changeTaskButton(int idx, bool To = true)
        {
            Button button = taskButtons[idx];
            buttonForeBackgroundAnimation(button, To);
            buttonForeBackgroundAnimation(button, To, true);
            gtime += itv0 / 2;
        }
        void changeSelectButton(int idx, bool To= true)
        {
            Button button = taskButtons[idx];
            buttonForeBackgroundAnimation(button, To, true, true);
            buttonForeBackgroundAnimation(button, To, false, true);
            gtime += itv0 / 2;
        }
        // FS
        // change list string
        void setFSListString(int cvs, int lt, int it, string str)
        {
            DiscreteStringKeyFrame dsk = new DiscreteStringKeyFrame();
            dsk.Value = str;
            dsk.KeyTime = TimeSpan.FromMilliseconds(gtime);

            fsListAnimations[cvs][lt][it].KeyFrames.Add(dsk);
        }
        void setFSOpacity(object obj, bool show = true)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = TimeSpan.FromMilliseconds(itv0) ;
            da.BeginTime = TimeSpan.FromMilliseconds(gtime);
            Storyboard.SetTarget(da, (DependencyObject)obj);
            Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
            if (show)
            {
                da.To = 1;
            }
            else
            {
                da.To = 0;
            }
            
            OSAnimation.Children.Add(da);
        }

        void setCardSelected(Card c, bool to=true)
        {
            ColorAnimation cc = new ColorAnimation();
            if (to)
                cc.To = Color.FromArgb(0xff, 0xae, 0xea, 00);
            else 
                cc.To = Colors.White;

            cc.Duration = TimeSpan.FromMilliseconds(itv0/2);
            cc.BeginTime = TimeSpan.FromMilliseconds(gtime);
            Storyboard.SetTarget(cc, c);
            Storyboard.SetTargetProperty(cc, new PropertyPath("(Background).(SolidColorBrush.Color)"));
            OSAnimation.Children.Add(cc);
        }
        void setButtonRunning(bool to=true)
        {
            setFSOpacity(FlpStateButton11, to);
            setFSOpacity(FlpStateButton12, !to);
            setFSOpacity(FlpStateButton21, to);
            setFSOpacity(FlpStateButton22, !to);
            setFSOpacity(FlpStateButton01, to);
            setFSOpacity(FlpStateButton02, !to);

            changeTL(!to);
            _readFp = to;
        }
        void showQButton(Button b, bool isShow=true)
        {
            ObjectAnimationUsingKeyFrames oa = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(oa, b);
            Storyboard.SetTargetProperty(oa, new PropertyPath("Visibility"));
            DiscreteObjectKeyFrame dok = new DiscreteObjectKeyFrame(isShow? Visibility.Visible: Visibility.Collapsed, TimeSpan.FromMilliseconds(gtime));
            oa.KeyFrames.Add(dok);
            OSAnimation.Children.Add(oa);
        }
        void showFPButton(int idx)
        {
            if(_readFp)
            {
                if (idx == 1)
                {
                    setFSOpacity(FlpStateButton11, _readFp);
                    setFSOpacity(FlpStateButton12, !_readFp);
                }
                else if (idx == 2)
                {
                    setFSOpacity(FlpStateButton21, _readFp);
                    setFSOpacity(FlpStateButton22, !_readFp);
                }
                else if (idx == 0)
                {
                    setFSOpacity(FlpStateButton01, _readFp);
                    setFSOpacity(FlpStateButton02, !_readFp);
                }
            }
            else
            {
                if (idx == 1)
                {
                    setFSOpacity(FlpStateButton11, false);
                    setFSOpacity(FlpStateButton12, false);
                }
                else if (idx == 2)
                {
                    setFSOpacity(FlpStateButton21, false);
                    setFSOpacity(FlpStateButton22, false);
                }
                else if (idx == 0)
                {
                    setFSOpacity(FlpStateButton01, false);
                    setFSOpacity(FlpStateButton02, false);
                }
            }
        }
        void showScene(int idx)
        {
            if (_openScene == idx)
                return;
            if(_openScene==0 )
            {
                setFSOpacity(FSPid00Canvas, false);
            }
            else if (_openScene == 1)
            {
                setFSOpacity(FSPid10Canvas, false);
            }
            else if (_openScene == 2)
            {
                setFSOpacity(FSPid20Canvas, false);
            }

            if (idx == 0)
            {
                setFSOpacity(FSPid00Canvas);
            }
            else if (idx == 1)
            {
                setFSOpacity(FSPid10Canvas);
            }
            else if (idx == 2)
            {
                setFSOpacity(FSPid20Canvas);
            }
            _openScene = idx;
        }
        #region StructureHandlers
        ProcessStruct getProcess(int pid)
        {
            foreach(var ps in processStructs)
            {
                if (ps != null&& ps.Pid == pid)
                    return ps;
            }
            return null;
        }
        #endregion
        #endregion

        #region Events
        void AnimationCompleted(object sender, EventArgs e)
        {
            if (_playing == 1)
            {
                _playing = 2;
                OSAnimation.Stop(App.Current.MainWindow);
                OSAnimation.Seek(App.Current.MainWindow, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
                OSAnimation.Pause(App.Current.MainWindow);
            }
        }
        #endregion

        #region UI
        void intializeUI()
        {
            StartPauseAnimationButton.IsEnabled = true;
            AnimationSlider.Maximum = gtime / 1000;
            AnimationSlider.IsEnabled = true;
            AnimationSlider.Value = 0;
            _playing = 0;
        }

        #region ButtonInteactions
        void Load_Click(object sender, RoutedEventArgs args)
        {
            if (_playing != 0)
                OSAnimation.Pause(App.Current.MainWindow);
        }
        void LoadBreakpointFile(object sender, DialogClosingEventArgs args)
        {
            string dataFile = "";
            if (!Equals(args.Parameter, true)) return;

            if (string.IsNullOrWhiteSpace(bpFileBox.Text))
                dataFile = "./Data/output_all_xs.txt";
            else
                dataFile = bpFileBox.Text.Trim();
            OSAnimation.Stop(App.Current.MainWindow);
            OSAnimation.Remove(App.Current.MainWindow);
            dataManipulate(dataFile);
            Trace.WriteLine("here");
        }

        void StartPauseAnimation(object sender, RoutedEventArgs args)
        {
            if (_playing == 0)
            {
                OSAnimation.Begin(App.Current.MainWindow, true);
                if (AnimationSlider.Value > 0)
                    OSAnimation.Seek(App.Current.MainWindow,
                     TimeSpan.FromSeconds(AnimationSlider.Value), TimeSeekOrigin.BeginTime);
                OSAnimation.SetSpeedRatio(App.Current.MainWindow, SpeedSlider.Value / 10);
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

        private void Slider_DragCompleted(object sender, RoutedEventArgs e)
        {
            if(_playing != 0)
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
        private void SpeedSlider_DragCompleted(object sender, RoutedEventArgs e)
        {
            if (_playing != 0)
                OSAnimation.SetSpeedRatio(App.Current.MainWindow, ((Slider)sender).Value / 10);
            SpeedSlider.Value = ((Slider)sender).Value;
            if (_playing == 3)
            {
                _playing = 1;
                OSAnimation.Resume(App.Current.MainWindow);
            }   
        }

        private void SpeedSlider_DragStarted(object sender, RoutedEventArgs e)
        {
            if (_playing == 1)
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