using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Microsoft.Extensions.Logging;
using Qoden.Binding;
using Qoden.Validation;

namespace Qoden.UI
{
    public class QodenController : Fragment, IControllerHost, IViewHost
    {
        public ILogger Logger { get; set; }

        ViewHolder _view;

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
            Logger = CreateLogger();
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("Created");
        }

        protected virtual ILogger CreateLogger()
        {
            return Config.LoggerFactory?.CreateLogger(GetType().Name);
        }

        BindingListHolder _bindings;
        public BindingList Bindings
        {
            get => _bindings.Value;
            set => _bindings.Value = value;
        }

        public sealed override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("OnCreateView");
            return _view.Value;
        }

        public sealed override void OnViewCreated(Android.Views.View view, Bundle savedInstanceState)
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

        public new View View
        {
            get => _view.Value;
            set => _view.Value = value;
        }

        public string Title
        {
            get => Activity?.Title;
            set => Activity.Title = value;
        }

        List<MenuItemInfo> menuItems = new List<MenuItemInfo>();
        public List<MenuItemInfo> MenuItems 
        {
            private get => menuItems;
            set
            {
                menuItems = value;
                HasOptionsMenu = menuItems.Count > 0;
                Activity.InvalidateOptionsMenu();
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            foreach(var itemInfo in MenuItems)
            {
                var item = menu.Add(Menu.None, itemInfo.Id, Menu.None, itemInfo.Title);
                item.SetShowAsAction(ShowAsAction.IfRoom);
                item.SetIcon(itemInfo.Icon);
            }
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            base.OnPrepareOptionsMenu(menu);
            foreach (var itemInfo in MenuItems)
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
            if(command != null) 
            {
                command.Execute();
                return true;
            }
            return base.OnOptionsItemSelected(item);
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

        /// <summary>
        /// Override this instead on OnViewCreated
        /// </summary>
        public virtual void ViewDidLoad()
        {
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

        public void ClearStackAndPush(QodenController controller)
        {
            if (FragmentManager.BackStackEntryCount > 0)
            {
                var id = FragmentManager.GetBackStackEntryAt(0).Id;
                FragmentManager.PopBackStackImmediate(id, FragmentManager.PopBackStackInclusive);
            }
            Push(controller);
        }

        public void Push(QodenController controller)
        {
            FragmentManager.BeginTransaction()
                           .Replace(Id, controller)
                           .AddToBackStack(controller.Tag)
                           .Commit();
        }

        public void Pop() => FragmentManager.PopBackStack();

        public void Present(QodenController controller) => Push(controller);

        public void Dismiss() => Pop();

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

        /// <summary>
        /// Override this instead on CreateView
        /// </summary>
        public virtual void LoadView()
        {
        }
    }

    public class QodenController<T> : QodenController where T : Android.Views.View
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

    public struct MenuItemInfo
    {
        public string Title { get; set; }
        public Drawable Icon { get; set; }
        public int Id { get; set; }
        public Command Command { get; set; }
    }
}
