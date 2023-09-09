using System.Threading.Tasks;

namespace EWS.Domain.Abstraction.Common;

public interface IHostNotificationService
{
    Task NotificationAsync();
}
