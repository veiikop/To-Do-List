using Microsoft.EntityFrameworkCore;
using To_Do_List.Models;

namespace To_Do_List.Repositories
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly APIDBContect _context;

        public TodoItemRepository(APIDBContect context)
        {
            _context = context;
        }
        /// <summary>
        /// Возвращает все задачи с подгруженным списком
        /// </summary>
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems
                .Include(ti => ti.TodoList)
                .ToList();
        }
        /// <summary>
        /// Находит задачу по ID с подгруженным списком
        /// </summary>
        public TodoItem GetById(int id)
        {
            return _context.TodoItems
                .Include(ti => ti.TodoList)
                .FirstOrDefault(ti => ti.Id == id);
        }
        /// <summary>
        /// Создаёт новую задачу
        /// </summary>
        public TodoItem Create(TodoItem entity)
        {
            _context.TodoItems.Add(entity);
            _context.SaveChanges();         
            return entity;
        }
        /// <summary>
        /// Обновляет существующую задачу
        /// </summary>
        public TodoItem Update(TodoItem entity)
        {
            _context.TodoItems.Update(entity);
            _context.SaveChanges();
            return entity;
        }
        /// <summary>
        /// Удаляет задачу по ID
        /// </summary>
        public bool Delete(int id)
        {
            var item = GetById(id);
            if (item == null) return false;

            _context.TodoItems.Remove(item);
            _context.SaveChanges();
            return true;
        }
        /// <summary>
        /// Проверяет существование задачи по ID
        /// </summary>
        public bool Exists(int id)
        {
            return _context.TodoItems.Any(ti => ti.Id == id);
        }
        /// <summary>
        /// Возвращает все задачи конкретного списка
        /// Фильтрует по TodoListId + подгружает список
        /// </summary>
        public IEnumerable<TodoItem> GetItemsByListId(int todoListId)
        {
            return _context.TodoItems
                .Include(ti => ti.TodoList)
                .Where(ti => ti.TodoListId == todoListId)
                .ToList();
        }
    }
}
