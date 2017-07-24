using System;
using Android.App;
using Example.Model;

namespace Example
{
    [Application]
    public class ExampleApplication : Application
    {
        public ExampleApplication(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            var dbFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            ExampleApp.Init(dbFolder).ContinueWith(x => {});
        }
    }
}
