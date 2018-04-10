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

        PlatformSwitch platformView;
        public PlatformSwitch PlatformView
        {
            get => platformView;
            set
            {
                platformView = value;
#if __IOS__
                checkedThumbColor = uncheckedThumbColor = PlatformView.ThumbTintColor != null ? PlatformView.ThumbTintColor.ToRGB() : RGB.Clear;
                var @switch = this;
                platformView.AddTarget((object sender, EventArgs e) =>
                {
                    @switch.PlatformView.ThumbTintColor = @switch.Checked ? @switch.CheckedThumbColor.ToColor() : @switch.UncheckedThumbColor.ToColor();
                }, UIKit.UIControlEvent.ValueChanged);
#endif     
            }
        }

        public Switch(object view) : this()
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
                PlatformView.ThumbTintColor = value ? CheckedThumbColor.ToColor() : UncheckedThumbColor.ToColor();
#elif __ANDROID__
                PlatformView.Checked = value;
#endif
            }
        }

#if __IOS__
        RGB checkedThumbColor, uncheckedThumbColor;

        RGB CheckedThumbColor
        {
            get => checkedThumbColor;
            set
            {
                checkedThumbColor = value;
                if (Checked)
                {
                    PlatformView.ThumbTintColor = checkedThumbColor.ToColor();
                }
            }
        }

        RGB UncheckedThumbColor
        {
            get => uncheckedThumbColor;
            set
            {
                uncheckedThumbColor = value;
                if (!Checked)
                {
                    PlatformView.ThumbTintColor = uncheckedThumbColor.ToColor();
                }
            }
        }
#endif

        public void SetThumbColors(RGB @checked, RGB @unchecked)
        {
#if __IOS__
            SetCheckedThumbColor(@checked);
            SetUncheckedThumbColor(@unchecked);
#elif __ANDROID__
            PlatformView.ThumbTintList = new ColorStateList(new int[][]
            {
                new int[] { Android.Resource.Attribute.StateChecked },
                new int[] { -Android.Resource.Attribute.StateChecked },
            }, new int[]
            {
                @checked.IntARGB,
                @unchecked.IntARGB
            });
#endif
        }

        public void SetTrackColors(RGB @checked, RGB @unchecked)
        {
#if __IOS__
            SetCheckedTrackColor(@checked);
            SetUncheckedTrackColor(@unchecked);
#elif __ANDROID__
            PlatformView.TrackTintList = new ColorStateList(new int[][]
            {
                new int[] { Android.Resource.Attribute.StateChecked },
                new int[] { -Android.Resource.Attribute.StateChecked },
            }, new int[]
            {
                @checked.IntARGB,
                @unchecked.IntARGB
            });
#endif
        }

        public void SetCheckedThumbColor(RGB color)
        {
#if __IOS__
            CheckedThumbColor = color;
#elif __ANDROID__
            PlatformView.ThumbTintList = GetSwitchTintColorStateList(color, true, PlatformView.ThumbTintList);
#endif
        }

        public void SetUncheckedThumbColor(RGB color)
        {
#if __IOS__
            UncheckedThumbColor = color;
#elif __ANDROID__
            PlatformView.ThumbTintList = GetSwitchTintColorStateList(color, false, PlatformView.ThumbTintList);
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
