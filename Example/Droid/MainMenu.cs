using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Qoden.UI;

namespace Example
{
    public partial class MainMenu : QodenView
    {
        [View]
        public TabLayout Tabs { get; private set; }
        [View]
        public TodoListView TodoList { get; private set; }
        [View]
        public ProfileForm ProfileForm { get; private set; }

        TabsBuilder _tabs;

        public MainMenu(Context ctx) : base(ctx) { }

        private void CustomizeTab(TabLayout.Tab item)
        {

        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            var tabs = layout.View(Tabs)
                .Left(0).Right(0).Top(0).AutoHeight();

            layout.View(ProfileForm)
                .Below(tabs.LayoutBounds, 0).Left(0).Right(0).Bottom(0);

            layout.View(TodoList)
                .Below(tabs.LayoutBounds, 0).Left(0).Right(0).Bottom(0);
        }
    }
}
