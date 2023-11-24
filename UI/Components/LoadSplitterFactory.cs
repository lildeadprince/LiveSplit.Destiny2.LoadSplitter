using LiveSplit.Model;
using System;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(LoadSplitterFactory))]

namespace LiveSplit.UI.Components
{
    public class LoadSplitterFactory : IComponentFactory
    {
        public string ComponentName => "Destiny 2 Load Splitter";

        public string Description => "Capture activity start in sync with Bungie API timer";

        public ComponentCategory Category => ComponentCategory.Control;

        public IComponent Create(LiveSplitState state) => new LoadSplitterComponent(state);

        public string UpdateName => ComponentName;

        public string UpdateURL => "";

        public Version Version => Version.Parse("1.0.0");

        public string XMLURL => "";
    }
}
