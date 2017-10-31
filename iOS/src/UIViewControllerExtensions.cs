using System;
using UIKit;

namespace Qoden.UI
{
    public static class UIViewControllerExtensions
    {
        public static UIViewController GetVisibleViewController(this UIViewController controller) 
        { 
            if (controller == null) 
                controller = UIApplication.SharedApplication.KeyWindow.RootViewController; 
 
            if (controller?.NavigationController?.VisibleViewController != null) 
                return controller.NavigationController.VisibleViewController; 
 
            if (controller.IsViewLoaded && controller.View?.Window != null) 
                return controller; 
            else 
            { 
                foreach (var childViewController in controller.ChildViewControllers) 
                { 
                    var foundVisibleViewController = GetVisibleViewController(childViewController); 
                    if (foundVisibleViewController == null) 
                        continue; 
 
                    return foundVisibleViewController; 
                } 
            } 
            return controller; 
        } 
    }
}
