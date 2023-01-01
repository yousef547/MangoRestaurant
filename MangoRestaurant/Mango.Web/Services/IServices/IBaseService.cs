using Mango.Web.Model;
using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface IBaseService:IDisposable
    {
        ResponseDto responsDto { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
