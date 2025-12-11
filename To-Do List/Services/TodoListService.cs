using AutoMapper;
using System.Security.Claims;
using To_Do_List.Models;
using To_Do_List.Models.DTO;
using To_Do_List.Repositories;

namespace To_Do_List.Services
{
    public class TodoListService : ITodoListService
    {
        private readonly ITodoListRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;

        public TodoListService(ITodoListRepository repository, IMapper mapper, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _mapper = mapper;
            _accessor = accessor;
        }

        private int GetCurrentUserId()
        {
            var claim = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Пользователь не аутентифицирован");
            return int.Parse(claim);
        }

        private string GetCurrentRole()
        {
            return _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "User";
        }

        public IEnumerable<TodoListDTO> GetAll()
        {
            var role = GetCurrentRole();
            var query = _repository.GetAll();

            if (role != "Admin")
            {
                int userId = GetCurrentUserId();
                query = query.Where(l => l.UserId == userId);
            }

            return _mapper.Map<IEnumerable<TodoListDTO>>(query.ToList());
        }

        public TodoListDTO? GetById(int id)
        {
            var list = _repository.GetById(id);
            if (list == null) return null;

            var role = GetCurrentRole();
            if (role != "Admin" && list.UserId != GetCurrentUserId())
                throw new UnauthorizedAccessException("Нет прав доступа к списку задач");

            return _mapper.Map<TodoListDTO>(list);
        }

        public TodoListDTO Create(CreateTodoListDTO dto)
        {
            var list = _mapper.Map<TodoList>(dto);
            list.UserId = GetCurrentUserId();         
            list.CreatedAt = DateTime.UtcNow;
            return _mapper.Map<TodoListDTO>(_repository.Create(list));
        }

        public TodoListDTO? Update(int id, CreateTodoListDTO dto)
        {
            var existing = _repository.GetById(id) ?? throw new KeyNotFoundException("Список не найден");

            var role = GetCurrentRole();
            if (role != "Admin" && existing.UserId != GetCurrentUserId())
                throw new UnauthorizedAccessException("Нет прав на изменение чужого списка");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            return _mapper.Map<TodoListDTO>(_repository.Update(existing));
        }

        public bool Delete(int id)
        {
            var list = _repository.GetById(id);
            if (list == null) return false;

            var role = GetCurrentRole();
            if (role != "Admin" && list.UserId != GetCurrentUserId())
                throw new UnauthorizedAccessException("Нет прав на удаление чужого списка");

            return _repository.Delete(id);
        }
    }
}