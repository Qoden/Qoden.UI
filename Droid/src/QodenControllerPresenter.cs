using System;
using Android.Content;
using Android.OS;
using Android.Views;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace Qoden.UI
{
    public sealed class QodenControllerPresenter : DialogFragment
    {
        public static QodenControllerPresenter Wrap(QodenController controller, Action completionHandler) => 
            new QodenControllerPresenter { _viewController = controller, _completionHandler = completionHandler };

        private QodenController _viewController;
        private Action _completionHandler;
        private QodenControllerPresenter()
        {
            SetStyle(StyleNoTitle, Resource.Style.FullscreenDialog);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            ChildFragmentManager.BeginTransaction()
                                .Add(_viewController, "child")
                                .Commit();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) => 
            _viewController.View;

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            _completionHandler?.Invoke();
        }
    }
}
