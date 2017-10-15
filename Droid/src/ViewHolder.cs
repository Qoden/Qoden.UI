using System;
using Android.Views;

namespace Qoden.UI
{
    struct ViewHolder
    {
        IViewHost _host;
        Android.Views.View _view;

        public ViewHolder(IViewHost host)
        {            
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            this._view = null;
            DidLoad = false;
        }

        public bool DidLoad { get; set; }

        public View Value
        {
            get
            {
                if (_view == null)
                {
                    try
                    {
                        _host.LoadView();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{_host.GetType()} failed to create view: " + e);
                        throw;
                    }
                }
                if (_view == null)
                {
                    throw new InvalidOperationException("Controller does not have view.");
                }
                return _view;
            }
            set
            {
                if (_view == null)
                {
                    _view = value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot change loaded view");
                }
            }
        }
    }
}