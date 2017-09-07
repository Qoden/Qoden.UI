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
        public View Content;
        public Action<TabLayout.Tab> Customization;
    }

    public class TabsBuilder
    {
        List<View> _tabs = new List<View>();
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
                _tabs[i].Visibility = i == position ? ViewStates.Visible : ViewStates.Gone;
                if (i == position)
                {
                    _tabs[i].RequestLayout();
                }
            }
        }

        public void AddTab(Tab tabConfig)
        {
            var tab = View.NewTab();
            if (tabConfig.Title != null) tab.SetText(tabConfig.Title);
            if (tabConfig.Image != null) tab.SetIcon(tabConfig.Image);

            tabConfig.Customization?.Invoke(tab);
            _tabs.Add(tabConfig.Content);
            _items.Add(tab);
        }

        public void Build()
        {
            foreach (var tab in _items)
            {
                View.AddTab(tab);
            }
            
            SelectTab(View.SelectedTabPosition);
        }

        public void SelectTab(int index)
        {
            View.GetTabAt(index).Select();
            OnTabSelected(index);
        }

        public TabLayout View { get; private set; }

        public IEnumerable<Android.Views.View> ContentViews => _tabs;
    }
}
