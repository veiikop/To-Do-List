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

        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems
                .Include(ti => ti.TodoList)
                .ToList();
        }

        public TodoItem GetById(int id)
        {
            return _context.TodoItems
                .Include(ti => ti.TodoList)
                .FirstOrDefault(ti => ti.Id == id);
        }

        public TodoItem Create(TodoItem entity)
        {
            _context.TodoItems.Add(entity);
            _context.SaveChanges();         
            return entity;
        }

        public TodoItem Update(TodoItem entity)
        {
            _context.TodoItems.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public bool Delete(int id)
        {
            var item = GetById(id);
            if (item == null) return false;

            _context.TodoItems.Remove(item);
            _context.SaveChanges();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.TodoItems.Any(ti => ti.Id == id);
        }

        public IEnumerable<TodoItem> GetItemsByListId(int todoListId)
        {
            return _context.TodoItems
                .Include(ti => ti.TodoList)
                .Where(ti => ti.TodoListId == todoListId)
                .ToList();
        }
    }
}
