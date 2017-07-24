using System;
using System.Collections.Generic;
using UIKit;

namespace Qoden.UI
{
    public struct Tab
    {
        public string Title;
        public UIImage Image;
        public UIView Content;
    }

    public class TabsBuilder
    {        
        List<UIView> _tabs = new List<UIView>();
        List<UITabBarItem> _items = new List<UITabBarItem>();

        public TabsBuilder(UITabBar layout)
        {
            View = layout;
            View.ItemSelected += OnTabSelected;
        }

        private void OnTabSelected(object sender, UITabBarItemEventArgs e)
        {
            int position = Array.IndexOf(View.Items, e.Item);
            OnTabSelected(position);
        }

        private void OnTabSelected(int position)
        {
            for (int i = 0; i < _tabs.Count; ++i)
            {
                var enable = i == position;
                _tabs[i].SetVisibility(enable);
                if (enable)
                {
                    _tabs[i].Superview.BringSubviewToFront(_tabs[i]);
                }
            }
        }

        public void AddTab(Tab tabConfig, Action<UITabBarItem> customize = null)
        {
            var tab = new UITabBarItem();
            if (tabConfig.Title != null) tab.Title = tabConfig.Title;
            if (tabConfig.Image != null) tab.Image = tabConfig.Image;

            customize?.Invoke(tab);
            _tabs.Add(tabConfig.Content);
            _items.Add(tab);
        }

        public void Build()
        {
            View.Items = _items.ToArray();
            View.SelectedItem = _items[0];
            OnTabSelected(0);
        }

        public UITabBar View { get; private set; }

        public IEnumerable<UIView> ContentViews => _tabs;
    }
}
