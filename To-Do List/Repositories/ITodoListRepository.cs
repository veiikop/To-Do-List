using To_Do_List.Models;

namespace To_Do_List.Repositories
{
    // Наследует все CRUD-операции из IRepository<TodoList>
    public interface ITodoListRepository : IRepository<TodoList>
    {

    }
}
