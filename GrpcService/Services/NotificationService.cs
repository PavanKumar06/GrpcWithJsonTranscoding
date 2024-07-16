using Grpc.Core;
using GrpcService.Data;
using GrpcService.Models;

namespace GrpcService.Services
{
    public class NotificationService : Notification.NotificationBase
    {
        private readonly ToDo.ToDoClient _toDoClient;
        private readonly AppDbContext _dbContext;

        public NotificationService(ToDo.ToDoClient toDoClient, AppDbContext dbContext)
        {
            _toDoClient = toDoClient;
            _dbContext = dbContext;
        }

        public override async Task<CreateNotificationResponse> CreateNotification(CreateNotificationRequest request, ServerCallContext context)
        {
            if (request == null || request.TodoId <= 0 || String.IsNullOrEmpty(request.Notification))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Give valid arguments"));
            }

            var toDoRequest = new ReadToDoRequest { Id = request.TodoId };
            var toDoResponse = await _toDoClient.ReadToDoAsync(toDoRequest);

            if (toDoResponse == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Give valid ToDo Id"));
            }

            var notificationItem = new NotificationItem { ToDoTitle = toDoResponse.Title, Notification = request.Notification};

            await _dbContext.AddAsync(notificationItem);
            await _dbContext.SaveChangesAsync();

            return await Task.FromResult(new CreateNotificationResponse
            {
                Id = notificationItem.Id
            });
        }
    }
}
