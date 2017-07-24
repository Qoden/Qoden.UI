using Qoden.UI;
using UIKit;

namespace Example
{
    public partial class MainMenu
    {
        [View]
        [Theme("Tabs")]
        public UITabBar Tabs { get; private set; }
        [View]
        public ProfileForm ProfileForm { get; private set; }
        [View]
        public TodoListView TodoList { get; private set; }

        TabsBuilder _tabs;

        private void CustomizeTab(UITabBarItem item)
        {
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;

            var tabs = layout.View(Tabs)
                .Left(0).Right(0).Bottom(0).AutoHeight();

            layout.View(ProfileForm)
                    .Above(tabs.LayoutBounds, 0).Left(0).Right(0).Top(0);

            layout.View(TodoList)
                    .Above(tabs.LayoutBounds, 0).Left(0).Right(0).Top(0);
        }
    }
}
