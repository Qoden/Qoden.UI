using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Microsoft.Extensions.Logging;
using Qoden.Binding;
using Qoden.UI.Wrappers;
using Qoden.Validation;
using View = Android.Views.View;

namespace Qoden.UI
{
    // Bindings, children and initialization
    public partial class QodenController : Fragment, IControllerHost, IViewHost
    {
        ViewHolder _view;
        public new View View
        {
            get => _view.Value;
            set => _view.Value = value;
        }

        BindingListHolder _bindings;
        public BindingList Bindings
        {
            get => _bindings.Value;
            set => _bindings.Value = value;
        }

        ChildViewControllersList _childControllers;
        public ChildViewControllersList ChildControllers
        {
            get
            {
                Assert.State(Context == null || ChildFragmentManager == null, nameof(ChildControllers))
                    .IsFalse("Cannot access {Key} wen fragment detached");
                if (_childControllers == null)
                {
                    _childControllers = new ChildViewControllersList(Context, ChildFragmentManager);
                }
                return _childControllers;
            }
        }

        protected QodenController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public QodenController()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            _view = new ViewHolder(this);
            InitializeLogger();   
        }
    }

    // Lifecycle
    public partial class QodenController
    {
        public sealed override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("OnCreateView");
            return _view.Value;
        }

        public sealed override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("OnViewCreated (ViewDidLoad)");

            base.OnViewCreated(view, savedInstanceState);

            if (!_view.DidLoad)
            {
                _view.DidLoad = true;
                ViewDidLoad();
            }
        }

        public sealed override void OnResume()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("OnResume (ViewWillAppear)");

            base.OnResume();
            if (!Bindings.Bound)
            {
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
            ViewWillAppear();
        }

        public sealed override void OnPause()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("OnPause (ViewWillDisappear)");

            base.OnPause();
            Bindings.Unbind();
            ViewWillDisappear();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            ((AppCompatActivity) Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(false);
        }
        
        /// <summary>
        /// Override this instead on CreateView
        /// </summary>
        public virtual void LoadView()
        { }
        
        /// <summary>
        /// Override this instead on OnViewCreated
        /// </summary>
        public virtual void ViewDidLoad()
        { }

        /// <summary>
        /// Override this instead on OnResume
        /// </summary>
        protected virtual void ViewWillAppear()
        { }

        /// <summary>
        /// Override this instead on OnPause
        /// </summary>
        protected virtual void ViewWillDisappear()
        { }
        
    }

    // Logger
    public partial class QodenController
    {
        public ILogger Logger { get; set; }

        protected virtual ILogger CreateLogger()
        {
            return Config.LoggerFactory?.CreateLogger(GetType().Name);
        }
        
        private void InitializeLogger()
        {
            Logger = CreateLogger();
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("Created");
        }
    }

    // Toolbar
    public partial class QodenController
    {
        private string _title = "";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (IsPresented)
                {
                    Presenter.Toolbar.TitleView.Text = value;
                    return;
                }
                ((QodenActivity) Activity).Toolbar.TitleView.Text = value;
            }
        }

        private List<MenuItemInfo> _menuItems = new List<MenuItemInfo>();
        public List<MenuItemInfo> MenuItems
        {
            private get => _menuItems;
            set
            {
                _menuItems = value;
                // If Side is left, then change Id to
                // Android.Resource.Id.Home, to use it later as a HomeButton
                for (var i = 0; i < _menuItems.Count; i++)
                {
                    var menuItemInfo = _menuItems[i];
                    if (menuItemInfo.Side == Side.Left)
                    {
                        // Creating a new MenuItemInfo instead of changing Id
                        // is needed because MenuItemInfo is a struct
                        _menuItems[i] = new MenuItemInfo
                        {
                            Id = Android.Resource.Id.Home,
                            Command = menuItemInfo.Command,
                            Icon = menuItemInfo.Icon,
                            Title = menuItemInfo.Title,
                            Side = Side.Left
                        };
                    }
                }
                HasOptionsMenu = _menuItems.Count > 0;
                if (IsPresented)
                {
                    Presenter.CreateAndSetOptionsMenu(_menuItems);
                    return;
                }
                Activity.InvalidateOptionsMenu();
            }
        }
        
        public bool ToolbarVisible
        {
            get
            {
                if (IsPresented)
                    return Presenter.Toolbar?.Visibility == ViewStates.Visible;
                return ((AppCompatActivity) Activity).SupportActionBar?.IsShowing ?? false;
            }
            set
            {
                if (IsPresented)
                {
                    Presenter.Toolbar.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
                    return;
                }
                switch (Activity)
                {
                    case QodenActivity qodenActivity:
                        qodenActivity.ToolbarVisible = value;
                        break;
                    case AppCompatActivity compatActivity when value && !ToolbarVisible:
                        compatActivity.SupportActionBar.Show();
                        break;
                    case AppCompatActivity compatActivity when !value && ToolbarVisible:
                        compatActivity.SupportActionBar.Hide();
                        break;
                }
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            menu.Clear(); 
            foreach (var itemInfo in MenuItems)
            {
                if (itemInfo.Side == Side.Left)
                {
                    ((AppCompatActivity) Activity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    ((AppCompatActivity) Activity).SupportActionBar.SetHomeAsUpIndicator(itemInfo.Icon);
                }
                else
                {
                    var item = menu.Add(Menu.None, itemInfo.Id, Menu.None, itemInfo.Title);
                    item.SetShowAsAction(ShowAsAction.IfRoom);
                    item.SetIcon(itemInfo.Icon);
                }
            }
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            base.OnPrepareOptionsMenu(menu);
            foreach (var itemInfo in MenuItems.Where(menuItem => menuItem.Side == Side.Right))
            {
                var item = menu.FindItem(itemInfo.Id) ?? menu.Add(Menu.None, itemInfo.Id, Menu.None, itemInfo.Title);
                item.SetIcon(itemInfo.Icon);
                item.SetTitle(itemInfo.Title);
                item.SetShowAsAction(ShowAsAction.IfRoom);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var itemInfo = MenuItems.Find(info => info.Id == item.ItemId);
            var command = itemInfo.Command;
            if (command == null)
                return base.OnOptionsItemSelected(item);
            command.Execute();
            return true;
        }
    }

    // navigation
    public partial class QodenController
    {
        public QodenControllerPresenter Presenter => ParentFragment as QodenControllerPresenter;

        public bool IsPresented => Presenter != null;
        
        public void Push(QodenController controller)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Id, controller)
                           .AddToBackStack(controller.GetType().Name)
                           .Commit();
        }

        public void ClearStackAndPush(QodenController controller)
        {
            if (FragmentManager.BackStackEntryCount > 0)
            {
                var id = FragmentManager.GetBackStackEntryAt(0).Id;
                FragmentManager.PopBackStack(id, FragmentManager.PopBackStackInclusive);
            }
            FragmentManager.BeginTransaction()
                           .Replace(Id, controller)
                           .Commit();
        }

        public void Pop() => FragmentManager.PopBackStack();

        public void Present(QodenController controller, Action completionHandler = null, bool withNavigation = false)
        {
            var presenter = QodenControllerPresenter.Wrap(controller, completionHandler, withNavigation);
            presenter.Show(ChildFragmentManager, "presenter");
        }

        public void Dismiss() => ((QodenControllerPresenter)ParentFragment).Dismiss();
    }

    public class QodenController<T> : QodenController where T : View
    {
        public new T View
        {
            get => (T)base.View;
            set => base.View = value;
        }

        public override void LoadView()
        {
            if (Context == null)
            {
                throw new InvalidOperationException("Cannot access View before controller added to parent Activity/Fragment");
            }
            base.View = (T)Activator.CreateInstance(typeof(T), Context);
        }
    }
}
