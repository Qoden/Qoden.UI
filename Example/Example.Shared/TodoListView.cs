using System;
using System.Drawing;
using Example.Model;
using Qoden.Binding;
using Qoden.UI;

namespace Example
{
    public partial class TodoListView : QodenView
    {
        [View]
        public QListView TodoList { get; private set; }
        [View]
        public QButton LoadButton { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();
            LoadButton.SetText("Load Todos");
            LoadButton.SetBackgroundColor(new RGB(255, 0, 0));
            TodoList.SetBackgroundColor(new RGB(100, 100, 100));
#if __ANDROID__
            this.SetPadding(new EdgeInset(16, 0, 16, 0));
#endif
        }

        protected override void OnLayout(LayoutBuilder layout)
        {
            var todo = layout.View(TodoList)
                    .Left(0).Right(0).Top(20).Bottom(150);
            layout.View(LoadButton)
                    .Left(10).Right(10).Below(todo.LayoutBounds, 5).Bottom(5);
        }

        public PlainListBinding<TodoRecord> Todos(ObservableList<TodoRecord> records)
        {
            var adapter = new PlainListBinding<TodoRecord>()
            {
                DataSource = records,
                ViewFactory = TodoCell
            };
            TodoList.SetContent(adapter);
            return adapter;
        }

        private void TodoCell(ListItemContext<TodoRecord> ctx)
        {
            var cell = (TodoListViewItem)ctx.View ?? ctx.CreateView<TodoListViewItem>();
            cell.TitleLabel.SetText(ctx.Item.Title);
            ctx.Result = cell;
        }
    }
}
