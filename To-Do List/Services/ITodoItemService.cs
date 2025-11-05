using To_Do_List.Models.DTO;

namespace To_Do_List.Services
{
    public interface ITodoItemService
    {
        IEnumerable<TodoItemDTO> GetAll();
        TodoItemDTO? GetById(int id);
        IEnumerable<TodoItemDTO> GetItemsByListId(int todoListId);
        TodoItemDTO Create(CreateTodoItemDTO dto);
        TodoItemDTO? Update(int id, UpdateTodoItemDTO dto);
        bool Delete(int id);
    }
}