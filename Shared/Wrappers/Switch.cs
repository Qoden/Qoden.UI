using System;
#if __ANDROID__
using PlatformSwitch = Android.Support.V7.Widget.SwitchCompat;
using Android.Content.Res;
#elif __IOS__
using PlatformSwitch = UIKit.UISwitch;
#endif
namespace Qoden.UI.Wrappers
{
    public struct Switch
    {
        public static implicit operator PlatformSwitch(Switch view) { return view.PlatformView; }
        public PlatformSwitch PlatformView { get; set; }

        public Switch(object view)
        {
            PlatformView = (PlatformSwitch)view;
        }

        public bool Checked
        {
#if __IOS__
            get => PlatformView.On;
#elif __ANDROID__
            get => PlatformView.Checked;
#endif
            set
            {
#if __IOS__
                PlatformView.On = value;
#elif __ANDROID__
                PlatformView.Checked = value;
#endif
            }
        }

        public void SetThumbColor(RGB color)
        {
#if __IOS__
            PlatformView.ThumbTintColor = color.ToColor();
#elif __ANDROID__
            PlatformView.ThumbTintList = ColorStateList.ValueOf(color.ToColor());
#endif
        }

        public void SetCheckedTrackColor(RGB color)
        {
#if __IOS__
            PlatformView.OnTintColor = color.ToColor();
#elif __ANDROID__
            PlatformView.TrackTintList = GetSwitchTintColorStateList(color, true, PlatformView.TrackTintList);
#endif
        }

        public void SetUncheckedTrackColor(RGB color)
        {
#if __IOS__
            PlatformView.TintColor = color.ToColor();
#elif __ANDROID__
            PlatformView.TrackTintList = GetSwitchTintColorStateList(color, false, PlatformView.TrackTintList);
#endif
        }

#if __ANDROID__
        static ColorStateList GetSwitchTintColorStateList(RGB color, bool isChecked, ColorStateList oldTintList = null)
        {
            ColorStateList tintList;

            var modifiedState = isChecked ? Android.Resource.Attribute.StateChecked : -Android.Resource.Attribute.StateChecked;

            if (oldTintList == null)
            {
                tintList = new ColorStateList(new int[][]
                {
                    new int[] { modifiedState },
                }, new int[]
                {
                    color.IntARGB
                });
            }
            else
            {
                var colorToKeep = oldTintList.GetColorForState(new int[] { -modifiedState }, RGB.Clear.ToColor());
                tintList = new ColorStateList(new int[][]
                {
                    new int[] { modifiedState },
                    new int[] { -modifiedState },
                }, new int[]
                {
                    color.IntARGB,
                    colorToKeep
                });
            }
            return tintList;
        }
#endif

        public View AsView() { return new View() { PlatformView = PlatformView }; }
    }

    public static class SwitchExtensions
    {
        public static Switch Switch(this ViewBuilder b, bool addSubview = true)
        {
#if __IOS__
            var view = new Switch() { PlatformView = new PlatformSwitch() };
#elif __ANDROID__
            var view = new Switch() { PlatformView = new PlatformSwitch(b.Context) };
#endif
            if (addSubview) b.AddSubview(view.PlatformView);
            return view;

        }

        public static Switch AsSwitch(this PlatformSwitch @switch)
        {
            return new Switch() { PlatformView = @switch };
        }
    }
}
