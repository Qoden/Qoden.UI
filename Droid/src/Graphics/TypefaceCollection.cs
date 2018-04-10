using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Android.Graphics;

namespace Qoden.UI
{
    public class TypefaceCollection
    {
        struct Key
        {
            public string Name;
            public FontStyle Style;

            public Key(string name, FontStyle style)
            {
                Name = name;
                Style = style;
            }
        }

        static ConcurrentDictionary<Key, Typeface> _cache = new ConcurrentDictionary<Key, Typeface>();

        public static void Add(string name, FontStyle style, Typeface typeface)
        {
            _cache.AddOrUpdate(new Key(name, style), typeface, (arg1, arg2) => arg2);
        }

        public static Typeface Get(string name, FontStyle style = FontStyle.Unknown)
        {
            Typeface face;
            if (!_cache.TryGetValue(new Key(name, style), out face))
            {
                face = Typeface.Create(name, (TypefaceStyle)style);
                if (face != null)
                {
                    Add(name, style, face);
                }
            }
            return face;
        }

        public static string GetFontFamily(Typeface tf)
        {
            var systemTypefaceMapField = tf.Class.GetDeclaredField("sSystemFontMap");
            systemTypefaceMapField.Accessible = true;
            var systemTypefaceMap = (System.Collections.IDictionary) systemTypefaceMapField.Get(tf);
            foreach (var entry in systemTypefaceMap)
            {
                var kv = (DictionaryEntry)entry;
                if (kv.Value == tf)
                    return kv.Key as string;
            }
            return null;
        }
    }
}
