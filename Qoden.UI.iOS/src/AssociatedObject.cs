using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace Qoden.UI
{
    public enum AssociationPolicy
    {
        ASSIGN = 0,
        RETAIN_NONATOMIC = 1,
        COPY_NONATOMIC = 3,
        RETAIN = 01401,
        COPY = 01403,
    }
    public static class AssociatedObject
    {
        [DllImport(ObjCRuntime.Constants.ObjectiveCLibrary)]
        static extern void objc_setAssociatedObject(IntPtr o, IntPtr key, IntPtr value, AssociationPolicy policy);

        [DllImport(ObjCRuntime.Constants.ObjectiveCLibrary)]
        static extern IntPtr objc_getAssociatedObject(IntPtr o, IntPtr key);

        public static void Set(NSObject o, NSString key, NSObject value, AssociationPolicy policy)
        {
            objc_setAssociatedObject(o.Handle, key.Handle, value.Handle, policy);
        }

        public static NSObject Get(NSObject o, NSString key)
        {
            var obj = objc_getAssociatedObject(o.Handle, key.Handle);
            return Runtime.GetNSObject(obj);
        }
    }
}
