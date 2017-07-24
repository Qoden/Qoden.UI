using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace Example.Model
{
    public class ExampleApp
    {
        private static string _dbFullPath;

        public static async Task<bool> Init(string dbFolder)
        {
            var fileName = "todos.db";
            _dbFullPath = Path.Combine(dbFolder, fileName);
            try
            {
                SQLiteAsyncConnection.ResetPool();
                var db = new SQLiteAsyncConnection(_dbFullPath);
                await db.CreateTableAsync<TodoRecord>();
                var todosCount = await db.Table<TodoRecord>().CountAsync();
                if (todosCount == 0)
                {
                    await db.InsertAllAsync(new[]{
                            new TodoRecord() { Title = "New Todo 1" },
                            new TodoRecord() { Title = "New Todo 2" },
                            new TodoRecord() { Title = "New Todo 3" }
                        });
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        internal static SQLiteAsyncConnection NewDbConnection()
        {
            return new SQLiteAsyncConnection(_dbFullPath);
        }
    }
}
