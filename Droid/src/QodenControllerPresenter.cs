using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Qoden.Binding;
using Qoden.UI.Wrappers;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using View = Android.Views.View;

namespace Qoden.UI
{
    public sealed partial class QodenControllerPresenter : DialogFragment
    {
        private const int ContainerId = 1;
        
        public static QodenControllerPresenter Wrap(QodenController controller, Action completionHandler, bool withNavigation) => 
            new QodenControllerPresenter { _viewController = controller, _completionHandler = completionHandler, _withNavigation = withNavigation};

        private QodenController _viewController;
        private Action _completionHandler;

        private QodenControllerPresenter()
        {
            SetStyle(StyleNoTitle, Resource.Style.FullscreenDialog);
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var linearLayout = new LinearLayout(Context) { Orientation = Orientation.Vertical };

            Toolbar = new CustomViewToolbar(Context, GravityFlags.CenterHorizontal | GravityFlags.CenterVertical) 
                { Visibility = _withNavigation ? ViewStates.Visible : ViewStates.Gone };
            Toolbar.MenuItemClick += OnOptionsItemSelected;
            linearLayout.AddView(Toolbar, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, QodenActivity.GetDefaultToolbarHeight(Context.Theme)));

            var contentFrameLayout = new FrameLayout(Context) { Id = ContainerId };
            linearLayout.AddView(contentFrameLayout, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            
            ChildFragmentManager.BeginTransaction()
                                .Add(contentFrameLayout.Id, _viewController)
                                .Commit();

            return linearLayout;
        }

        private void OnOptionsItemSelected(object sender, Toolbar.MenuItemClickEventArgs e) => _viewController.OnOptionsItemSelected(e.Item);

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            Activity.InvalidateOptionsMenu();
            _completionHandler?.Invoke();
        }
    }
    
    // toolbar
    public partial class QodenControllerPresenter
    {
        private bool _withNavigation;
        private EventHandler<Toolbar.NavigationClickEventArgs> _navigationHandler;
        
        public CustomViewToolbar Toolbar { get; private set; }

        public void CreateAndSetOptionsMenu(IEnumerable<MenuItemInfo> menuItems)
        {
            var menu = Toolbar.Menu;
            menu.Clear();
            Toolbar.NavigationClick -= _navigationHandler;
            Toolbar.NavigationIcon = null;
            foreach (var itemInfo in menuItems)
            {
                if (itemInfo.Side == Side.Left)
                {
                    Toolbar.NavigationIcon = itemInfo.Icon;
                    _navigationHandler = (sender, args) => itemInfo.Command.Execute();
                    Toolbar.NavigationClick += _navigationHandler;
                }
                else
                {
                    var item = menu.Add(Menu.None, itemInfo.Id, Menu.None, itemInfo.Title);
                    item.SetShowAsAction(ShowAsAction.IfRoom);
                    item.SetIcon(itemInfo.Icon);
                }
            }
            
        }
    }
}
