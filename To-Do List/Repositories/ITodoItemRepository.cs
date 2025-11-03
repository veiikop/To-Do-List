using To_Do_List.Models;

namespace To_Do_List.Repositories
{
    public interface ITodoItemRepository : IRepository<TodoItem>
    {
        IEnumerable<TodoItem> GetItemsByListId(int todoListId);
    }
}
