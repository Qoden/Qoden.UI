using System;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Microsoft.Extensions.Logging;
using Qoden.Binding;

namespace Qoden.UI
{
    public abstract partial class QodenActivity : AppCompatActivity, IControllerHost, IViewHost
    {
        public ChildViewControllersList ChildControllers { get; private set; }
        
        ViewHolder _view;
        public Android.Views.View View
        {
            get => _view.Value;
            set => _view.Value = value;
        }
        
        BindingListHolder _bindings;
        public BindingList Bindings
        {
            get => _bindings.Value;
            set { _bindings.Value = value; }
        }

        public void Push(Type type)
        {
            var intent = new Intent(this, type);
            StartActivity(intent);
        }

        public void Pop()
        {
            OnBackPressed();
        }
        
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
        /// Override this instead of OnCreate
        /// </summary>
        public virtual void ViewDidLoad()
        {
        }

        /// <summary>
        /// Override this to create view
        /// </summary>
        public virtual void LoadView()
        {
        }
        
        protected sealed override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Logger = CreateLogger();
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
                Logger.LogInformation("{controller} Created", GetType().Name);

            _view = new ViewHolder(this);

            ChildControllers = new ChildViewControllersList(this, SupportFragmentManager);
            AddContentView(View, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

            Toolbar = new CustomViewToolbar(this, GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            AddContentView(Toolbar, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, GetDefaultToolbarHeight(Theme)));
            SetSupportActionBar(Toolbar);
            ToolbarVisible = false;

            ViewDidLoad();
        }

        protected sealed override void OnResume()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnResume (ViewWillAppear)", GetType().Name);

            base.OnResume();
            if (!Bindings.Bound)
            {
                Bindings.Bind();
                Bindings.UpdateTarget();
            }
            ViewWillAppear();
        }

        protected sealed override void OnPause()
        {
            if (Logger != null && Logger.IsEnabled(LogLevel.Information)) 
                Logger.LogInformation("{controller} OnPause (ViewWillDisappear)", GetType().Name);

            base.OnPause();
            Bindings.Unbind();
            ViewWillDisappear();
        }
    }
    
    public partial class QodenActivity 
    {
        public ILogger Logger { get; set; }

        protected virtual ILogger CreateLogger()
        {
            return Config.LoggerFactory?.CreateLogger(GetType().Name);
        }
    }

    public partial class QodenActivity
    {
        public static int GetDefaultToolbarHeight(Resources.Theme theme)
        {
            var array = theme.ObtainStyledAttributes(new int[] { Resource.Attribute.actionBarSize });
            var height = array.GetLayoutDimension(0, "actionBarSize");
            array.Recycle();
            return height;
        }
        
        public CustomViewToolbar Toolbar { get; private set; }
        
        public event EventHandler<VisibilityChangeEventArgs> OnToolbarVisibilityChange;

        public bool ToolbarVisible 
        {
            get => SupportActionBar?.IsShowing ?? false;
            set
            {
                if(value && !ToolbarVisible) 
                {
                    SupportActionBar.Show();
                } 
                else if(!value && ToolbarVisible)
                {
                    SupportActionBar.Hide();
                }
                OnToolbarVisibilityChange?.Invoke(this, new VisibilityChangeEventArgs(value));
            }
        }
    }
    
    public class QodenActivity<T> : QodenActivity where T : Android.Views.View
    {
        public new T View
        {
            get => (T)base.View;
            set { base.View = value; }
        }

        public override void LoadView()
        {
            base.View = (T)Activator.CreateInstance(typeof(T), this);
        }
    }
}