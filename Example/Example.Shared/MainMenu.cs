using System;
using Qoden.UI;

namespace Example
{
    public partial class MainMenu : QodenView
    {
        protected override void CreateView()
        {
            base.CreateView();
            _tabs = new TabsBuilder(Tabs);
            _tabs.AddTab(new Tab { Title = "Sample Profile", Content = ProfileForm }, CustomizeTab);
            _tabs.AddTab(new Tab { Title = "Sample List", Content = TodoList }, CustomizeTab);
            _tabs.Build();
        }
    }
}
