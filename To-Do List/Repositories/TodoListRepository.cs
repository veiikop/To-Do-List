using Microsoft.EntityFrameworkCore;
using To_Do_List.Models;

namespace To_Do_List.Repositories
{
    public class TodoListRepository : ITodoListRepository
    {
        private readonly APIDBContect _context;

        public TodoListRepository(APIDBContect context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает все списки дел с подгруженными задачами
        /// </summary>
        public IEnumerable<TodoList> GetAll()
        {
            return _context.TodoLists
                .Include(tl => tl.Items)
                .ToList();
        }
        // <summary>
        /// Находит список по ID с подгруженными задачами.
        /// </summary>
        public TodoList GetById(int id)
        {
            return _context.TodoLists
                .Include(tl => tl.Items)
                .FirstOrDefault(tl => tl.Id == id);
        }
        /// <summary>
        /// Создаёт новый список дел 
        /// </summary>
        public TodoList Create(TodoList entity)
        {
            _context.TodoLists.Add(entity);
            _context.SaveChanges();
            return entity;
        }
        /// <summary>
        /// Обновляет существующий список дел
        /// </summary>
        public TodoList Update(TodoList entity)
        {
            _context.TodoLists.Update(entity);
            _context.SaveChanges();
            return entity;
        }
        /// <summary>
        /// Удаляет список по ID
        /// </summary>
        public bool Delete(int id)
        {
            var list = GetById(id);
            if (list == null) return false;

            _context.TodoLists.Remove(list);
            _context.SaveChanges();
            return true;
        }
        /// <summary>
        /// Проверяет существование списка по ID
        /// </summary>
        public bool Exists(int id)
        {
            return _context.TodoLists.Any(tl => tl.Id == id);
        }
    }
}
