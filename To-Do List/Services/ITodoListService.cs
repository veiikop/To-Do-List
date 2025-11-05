using To_Do_List.Models.DTO;

namespace To_Do_List.Services
{
    public interface ITodoListService
    {
        IEnumerable<TodoListDTO> GetAll();
        TodoListDTO? GetById(int id);
        TodoListDTO Create(CreateTodoListDTO dto);
        TodoListDTO? Update(int id, CreateTodoListDTO dto);
        bool Delete(int id);
    }
}