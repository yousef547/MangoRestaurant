
using Mango.Services.Email.Messages;

namespace Mango.Services.Email.Respository
{
    public interface IEmailRepository
    {
        Task SendAndLogEmail(UpdatePaymentResultMessage message);

    }
}
