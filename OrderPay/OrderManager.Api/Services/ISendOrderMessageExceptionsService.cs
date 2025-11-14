namespace OrderManager.Api.Services
{
    public interface ISendOrderMessageExceptionsService
    {
        public Task ReprocessOrders();
    }
}
