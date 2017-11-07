using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Util;

namespace Qoden.UI
{
    public static class ImageFactory
    {
        static Type DrawableClass { get; set; }
        static Context ApplicationContext { get; set; }

        public static void Init(Type drawableClass, Context context)
        {
            DrawableClass = drawableClass;
            ApplicationContext = context;
        }

        public static Bitmap GetBitmap(this Resources resource, string name) => BitmapFactory.DecodeResource(resource, IdFromTitle(name, DrawableClass));

        public static Task<Bitmap> GetBitmapAsync(this Resources resource, string name) => BitmapFactory.DecodeResourceAsync(resource, IdFromTitle(name, DrawableClass));

        public static Drawable GetDrawable(string name)
        {
            int id = IdFromTitle(name, DrawableClass);
            if (id == 0)
            {
                Log.Warn("Could not load image named: {0}", name);
                return null;
            }
            return ContextCompat.GetDrawable(ApplicationContext, id);
        }

        public static int GetDrawableIdByName(string name) => IdFromTitle(name, DrawableClass);

        static int IdFromTitle(string title, Type type) => GetId(type, title);

        static int GetId(Type type, string memberName)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type), "ImageFactory is not initialized");
            object value = type.GetFields().FirstOrDefault(p => p.Name == memberName)?.GetValue(type)
                ?? type.GetProperties().FirstOrDefault(p => p.Name == memberName)?.GetValue(type);
            if (value is int)
                return (int)value;
            return 0;
        }
    }
}
