using Core;

namespace OrderManager.Api.Services;

public interface IOrderMessageRepository
{
    Task SendOrderMessage(OrderMessage mensagem);
}
