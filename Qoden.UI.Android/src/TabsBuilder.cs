using System;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using Android.Views;

namespace Qoden.UI
{
    public struct Tab
    {
        public string Title;
        public Drawable Image;
        public Android.Views.View Content;
    }

    public class TabsBuilder
    {
        List<Android.Views.View> _tabs = new List<Android.Views.View>();
        List<TabLayout.Tab> _items = new List<TabLayout.Tab>();

        public TabsBuilder(TabLayout layout)
        {
            View = layout;
            View.TabSelected += OnTabSelected;
        }

        private void OnTabSelected(object sender, TabLayout.TabSelectedEventArgs e)
        {
            int position = e.Tab.Position;
            OnTabSelected(position);
        }

        private void OnTabSelected(int position)
        {
            for (int i = 0; i < _tabs.Count; ++i)
            {
                _tabs[i].SetVisibility(i == position);
                if (i == position)
                {
                    _tabs[i].RequestLayout();
                }
            }
        }

        public void AddTab(Tab tabConfig, Action<TabLayout.Tab> customize = null)
        {
            var tab = View.NewTab();
            if (tabConfig.Title != null) tab.SetText(tabConfig.Title);
            if (tabConfig.Image != null) tab.SetIcon(tabConfig.Image);

            customize?.Invoke(tab);
            _tabs.Add(tabConfig.Content);
            _items.Add(tab);
        }

        public void Build()
        {
            foreach (var tab in _items)
            {
                View.AddTab(tab);
            }
            OnTabSelected(View.SelectedTabPosition);
        }

        public TabLayout View { get; private set; }

        public IEnumerable<Android.Views.View> ContentViews => _tabs;
    }
}
