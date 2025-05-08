using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Healthmanagment.todo
{
    public class TodoListViewModel
    {
        public ObservableCollection<TodoItem> TodoItems { get; set; }

        public TodoListViewModel()
        {
            TodoItems = new ObservableCollection<TodoItem>
        {
            new TodoItem { Title = "Einkaufen", SubTasks = { new SubTask { Description = "Milch kaufen" }, new SubTask { Description = "Brot kaufen" } } },
            new TodoItem { Title = "Projekt abschlie?en", SubTasks = { new SubTask { Description = "Dokumentation schreiben" }, new SubTask { Description = "Code review machen" } } }
        };
        }
    }
}

