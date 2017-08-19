using System;
using Android.Content;
using Android.Support.V4.App;

namespace Qoden.UI
{
    /// <summary>
    /// Implemented by controllers which wants to know about Fragment Manager 
    /// and Context before they attached to parent Activity/Fragment.
    /// </summary>
    /// <remarks>
    /// This interface is used to emulate iOS behavior when you can access 
    /// controlelr view before controller itself attached to any parent.
    /// In case of Android you create view before fragment is attached.
    /// This causes problems with dialogs since they attached right before they 
    /// shows. This makes it impossible to configure dialog view right before it 
    /// is displayed. Configuring view right before it is displayed is a natural 
    /// and very convenient pattern in iOS. This interface is here just to enable 
    /// same pattern in Android as well.
    /// </remarks>
    public interface IDetachedController
    {
        FragmentManager FragmentManager { get; set; }

        Context Context { get; set; }
    }
}
