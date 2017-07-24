using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Qoden.UI;

namespace Example
{
    public partial class TodoListView
    {
        public TodoListView(Context context) : base(context)
        {
            if (LinkerTrick.False)
            {
#pragma warning disable RECS0026 // Possible unassigned object created by 'new'
                new ListView(null);
                new Button(null);
#pragma warning restore RECS0026 // Possible unassigned object created by 'new'
            }
        }
    }
}
