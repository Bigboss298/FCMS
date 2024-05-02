using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using System.Linq.Expressions;

namespace FCMS.Interfaces.Service
{
    public interface IProductService
    {
        Task<BaseResponse<ProductDto>> GetByIdAsync(string productId);
        Task<IReadOnlyList<ProductDto>> GetAsync(string param);
        Task<IReadOnlyList<ProductDto>> GetProductsAsync();
        Task<BaseResponse<ProductDto>> CreateAsync(CreateProductRequestModel model);
        Task<BaseResponse<ProductDto>> UpdateAsync(UpdateProductRequestModel model, string id);
        Task<bool> DeleteAsync(string productId);

    }
}
