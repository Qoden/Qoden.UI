using System;
using Android.Views;

namespace Qoden.UI
{
    public struct ViewHolder
    {
        IViewHost _host;
        View _view;

        public ViewHolder(IViewHost host) : this()
        {            
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            this._view = null;
        }

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
                        Console.WriteLine(e);
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