using Microsoft.AspNetCore.Mvc;

namespace FIT_Api_Example.Helper
{
    [ApiController]
    public abstract class BaseEndpoint<TRequest, TResponse> : ControllerBase
    {
        public abstract Task<TResponse> Obradi(TRequest request);
    }
}
