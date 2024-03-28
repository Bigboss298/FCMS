using FCMS.FileManager;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using Mapster;
using MapsterMapper;
using System.Linq.Expressions;

namespace FCMS.Implementations.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IFileManager fileManager, IMapper mapper)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
            _mapper = mapper;
        }
        public async Task<BaseResponse<ProductDto>> CreateAsync(CreateProductRequestModel model)
        {
            var checkProduct = await _productRepository.GetAll<Product>();
            foreach (var item in checkProduct)
            {
                if(item.Name == model.Name && item.FarmerId == model.FarmerId)
                {
                    return new BaseResponse<ProductDto>
                    {
                        Message = $"Farmer {item.Farmer.User.FirstName} You already have this product leased already kindly update its quantity",
                        Status = false,
                    };
                }
                
            }
            var newProduct = model.Adapt<Product>();
            var uploadPictures = await _fileManager.ListOfFilesToSystem(model.Images);
            if(!uploadPictures.Status)
            {
                return new BaseResponse<ProductDto>
                {
                    Message = $"{uploadPictures.Message}",
                    Status = false,
                };
            }
            foreach (var item in uploadPictures.Datas)
            {
                var productImage = new
                {
                    ImageReference = item.Name,
                    ProductId = newProduct.Id,
                };
                
                newProduct.ProductImages.Add(productImage.Adapt<ProductImages>());
            }
            _productRepository.Insert(newProduct);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<ProductDto>
            {
                Message = "Product Added successfully",
                Status = true,
            };
        }

        public async Task<bool> DeleteAsync(string productId)
        {
            var imageToDelete = await _productRepository.Get<Product>(x => x.Id == productId);
            if(imageToDelete is null)
            {
                return false;
            }
            _productRepository.Delete<Product>(imageToDelete);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public IReadOnlyList<ProductDto> GetAsync(string param)
        {
            var product = _productRepository.QueryWhere<Product>(x => x.FarmerId == param || x.Name == param || x.Id == param);
            if(!product.Any())
            {
                throw new NotFoundException("Product not Found!!!");
            }
            return product.Adapt<IReadOnlyList<ProductDto>>();
        }

        public async Task<BaseResponse<ProductDto>> GetByIdAsync(string productId)
        {
            var product = await _productRepository.Get<Product>(x => x.Id == productId);
            if(product  is null)
            {
                return new BaseResponse<ProductDto>
                {
                    Message = "Product not found!!!",
                    Status = false,
                };
            }
            return new BaseResponse<ProductDto>
            {
                Message = "Product found!!!",
                Status = true,
                Data = product.Adapt<ProductDto>(),
            };
        }

        public async Task<IReadOnlyList<ProductDto>> GetProductsAsync()
        {
            var products = await _productRepository.GetAll<Product>();
            if(!products.Any())
            {
                return new List<ProductDto>();
            }

            return products.Adapt<IReadOnlyList<ProductDto>>(); 
        }

        public async Task<BaseResponse<ProductDto>> UpdateAsync(UpdateProductRequestModel model, string id)
        {
            var productToUpdate = await _productRepository.Get<Product>(x => x.Id == id);
            if(productToUpdate is null)
            {
                throw new NotFoundException("Product doesnt exist or has been deleted");
            }
            else
            {
                _mapper.Map(model, productToUpdate);
                _productRepository.Update<Product>(productToUpdate);
                await _unitOfWork.SaveChangesAsync();
                return new BaseResponse<ProductDto>
                {
                    Message = "Products updated succesfully!!!",
                    Status = true,
                    Data = productToUpdate.Adapt<ProductDto>(),
                };
            }
        }
    }
}
