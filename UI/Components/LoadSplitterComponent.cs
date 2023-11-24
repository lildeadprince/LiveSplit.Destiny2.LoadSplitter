using LiveSplit.Destiny2;
using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class LoadSplitterComponent : IComponent
    {
        private readonly float THICCNESS = 3f;

        private LoadSplitter ls;
        private Task currentWatchingTask;
        private bool RestartLoadSplitterOnReset = false;

        public LoadSplitterSettings Settings { get; set; }

        protected LiveSplitState State { get; set; }
        protected Form Form { get; set; }
        protected TimerModel Model { get; set; }
        protected bool AlwaysPauseGameTime { get; set; }

        public string ComponentName => "Destiny 2 Load Splitter";

        public IDictionary<string, Action> ContextMenuControls { get; protected set; }


        #region Component
        public LoadSplitterComponent(LiveSplitState state)
        {
            // Livesplit
            ContextMenuControls = new Dictionary<string, Action>();
            Settings = new LoadSplitterSettings();

            // Component Graphics
            Line = new LineComponent((int)THICCNESS, Color.White);
            Cache = new GraphicsCache();

            // Forward Integration (LS -> splitter)
            Model = new TimerModel();
            Model.CurrentState = state;
            State = state;
            Form = state.Form;

            // Splitter core
            ls = new LoadSplitter();
            ls.OnDestinyActivityStart += OnDestinyActivityStart;
            State.OnReset += State_OnReset;
            State.OnSplit += State_OnSplit;
            State.OnStart += State_OnStart;

            // Commence
            if (Settings.AutoStart)
            {
                StartSplitter();
            }
            else
            {
                StopSpliter();
            }
        }

        public void Dispose()
        {
            //ls.OnStateUpdate -= OnLoadSplitterStateChange;
            ls.OnDestinyActivityStart -= OnDestinyActivityStart;
            State.OnReset -= State_OnReset;
            State.OnSplit -= State_OnSplit;
            State.OnStart -= State_OnStart;
        }

        public void StartSplitter()
        {
            RestartLoadSplitterOnReset = true;
            StartAsyncWatcher();

            ContextMenuControls.Clear();
            ContextMenuControls.Add("Stop Load Splitter", StopSpliter);
        }

        public void StopSpliter()
        {
            RestartLoadSplitterOnReset = false;
            ls.StopWatching();


            ContextMenuControls.Clear();
            ContextMenuControls.Add("Start Load Splitter", StartSplitter);
        }
        #endregion

        #region EventHandlers
        private void OnDestinyActivityStart(object sender, EventArgs e)
        {
            Model.Start();
        }

        private void State_OnSplit(object sender, EventArgs e)
        {
            if (State.CurrentSplitIndex == 0)
            {
                if (State.AttemptStarted.Time == null)
                {
                    // "Split" command can act as a reset if run is complete
                    State_OnReset(sender, TimerPhase.NotRunning);
                }
                // else "Split" command can act as a Start if run was not started
            }

            // else if Split invoked manually, then you know what you're doing; I'm out
            ls.StopWatching();
        }

        private void State_OnStart(object sender, EventArgs e)
        { 
            ls.StopWatching();
        }

        private void State_OnReset(object sender, TimerPhase e)
        {
            if (RestartLoadSplitterOnReset)
            {
                StartAsyncWatcher();
            }
        }

        private void StartAsyncWatcher()
        {
            if (currentWatchingTask == null || currentWatchingTask.IsCompleted)
            {
                currentWatchingTask = Task.Run(() => ls.StartWatchingApiStart());
            } // Reset before previous watcher finished? No-op case, just wait further
        }
        #endregion

        #region GFX
        public float PaddingTop => 0;
        public float PaddingBottom => 0;
        public float PaddingLeft => 0;
        public float PaddingRight => 0;

        public float VerticalHeight => THICCNESS;

        public float MinimumWidth => 0;

        public float HorizontalWidth => THICCNESS;

        public float MinimumHeight => 0;

        public GraphicsCache Cache { get; set; }
        public LineComponent Line { get; set; }
        public Color CurrentColor { get; set; }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            var oldClip = g.Clip;
            var oldMatrix = g.Transform;
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.Clip = new Region();
            Line.LineColor = CurrentColor;
            var scale = g.Transform.Elements[0];
            var newHeight = Math.Max((int)(THICCNESS * scale + 0.5f), 1) / scale;
            Line.VerticalHeight = newHeight;
            g.TranslateTransform(0, (THICCNESS - newHeight) / 2f);
            Line.DrawVertical(g, state, width, clipRegion);
            g.Clip = oldClip;
            g.Transform = oldMatrix;
            g.SmoothingMode = oldMode;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            var oldClip = g.Clip;
            var oldMatrix = g.Transform;
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            g.Clip = new Region();
            Line.LineColor = CurrentColor;
            var scale = g.Transform.Elements[0];
            var newWidth = Math.Max((int)(THICCNESS * scale + 0.5f), 1) / scale;
            g.TranslateTransform((THICCNESS - newWidth) / 2f, 0);
            Line.HorizontalWidth = newWidth;
            Line.DrawHorizontal(g, state, height, clipRegion);
            g.Clip = oldClip;
            g.Transform = oldMatrix;
            g.SmoothingMode = oldMode;
        }

        private Color getColorForState(bool splitterEnabled, LoadSplitterState splitterState)
        {
            if (!splitterEnabled) return Settings.StatusColor_Off;
            if (splitterState == LoadSplitterState.WaitingForDestinyProcess) return Settings.StatusColor_WaitingForDestinyProcess;
            if (splitterState == LoadSplitterState.WaitingForActivityStart) return Settings.StatusColor_WaitingForApiStart;

            return Settings.StatusColor_IdleOn;
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            CurrentColor = getColorForState(RestartLoadSplitterOnReset, ls.state);

            Cache.Restart();
            Cache["IndicatorColor"] = CurrentColor.ToArgb();

            if (invalidator != null && Cache.HasChanged)
                invalidator.Invalidate(0, 0, width, height);
        }

        #endregion

        public XmlNode GetSettings(XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
