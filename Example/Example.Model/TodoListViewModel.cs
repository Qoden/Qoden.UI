using System;
using System.Linq;
using System.Threading.Tasks;
using Qoden.Binding;

namespace Example.Model
{
    public class TodoListViewModel : DataContext
    {
        public TodoListViewModel()
        {
            LoadTodos = new AsyncCommand()
            {
                Action = OnLoadTodos,
                CanExecute = (arg) => !LoadTodos.IsRunning,
            };
            Todos = new ObservableList<TodoRecord>();
        }

        private async Task OnLoadTodos(object arg)
        {
            var conn = ExampleApp.NewDbConnection();
            var table = conn.Table<TodoRecord>();
            Todos.Reset(await table.ToListAsync());
        }

        public ObservableList<TodoRecord> Todos { get; private set; }

        public AsyncCommand LoadTodos { get; private set; }
    }
}
