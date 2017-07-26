using System;
using Qoden.UI;

namespace Example
{
    public interface ILabeledField
    {
        QTextView Label { get; }
        QEditText Text { get; }
        float LabelWidth { get; set; }
    }

    internal static class LabeledFieldUtil
    {
        public static void LayoutLabeledField(this ILabeledField field, LayoutBuilder layout)
        {
            var label = layout.View(field.Label)
                 .Left(20).AutoHeight().Width(Pixel.Val(field.LabelWidth)).CenterVertically();
            layout.View(field.Text)
                .After(label.LayoutBounds, 9.5f).AutoHeight().Right(0).CenterVertically();
        }
    }
}
