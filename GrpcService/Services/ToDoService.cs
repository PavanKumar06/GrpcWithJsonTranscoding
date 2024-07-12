using Grpc.Core;
using GrpcService.Data;
using GrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Services
{
    public class ToDoService : ToDo.ToDoBase
    {
        private readonly AppDbContext _dbContext;

        public ToDoService(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if (request == null || String.IsNullOrEmpty(request.Title) || String.IsNullOrEmpty(request.Description)) 
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Give valid arguments"));
            }

            var todoItem = new ToDoItem { Title = request.Title, Description = request.Description };

            await _dbContext.AddAsync(todoItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateToDoResponse
            {
                Id = todoItem.Id
            });
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if (request == null || request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Give a valid Id"));
            }

            var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(r => r.Id == request.Id);

            if (todoItem != null)
            {
                return await Task.FromResult(new ReadToDoResponse
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    TodoStatus = todoItem.ToDoStatus
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id: {request.Id}"));
        }

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();

            var todoItems = await _dbContext.ToDoItems.ToListAsync();

            foreach (var todoItem in todoItems) 
            {
                response.Todo.Add(new ReadToDoResponse
                {
                    Id = todoItem.Id,
                    Title = todoItem.Title,
                    Description = todoItem.Description,
                    TodoStatus = todoItem.ToDoStatus
                });
            }

            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if(request == null || request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Give a valid Id"));
            }

            var todoItem = _dbContext.ToDoItems.FirstOrDefault(r => r.Id == request.Id);

            if (todoItem != null)
            {
                todoItem.Title = !string.IsNullOrEmpty(request.Title) ? request.Title : todoItem.Title;
                todoItem.Description = !string.IsNullOrEmpty(request.Description) ? request.Description : todoItem.Description;
                todoItem.ToDoStatus = !string.IsNullOrEmpty(request.TodoStatus) ? request.TodoStatus : todoItem.ToDoStatus;

                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new UpdateToDoResponse
                {
                    Id = request.Id
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id: {request.Id}"));
        }

        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if(request == null || request.Id <= 0) 
            { 
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Give a valid Id")); 
            }

            var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(r => r.Id == request.Id);

            if (todoItem != null)
            {
                _dbContext.Remove(todoItem);
                await _dbContext.SaveChangesAsync();

                return await Task.FromResult(new DeleteToDoResponse 
                { 
                    Id = request.Id 
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id: {request.Id}"));
        }
    }
}
