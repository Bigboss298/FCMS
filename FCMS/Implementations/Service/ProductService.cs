using FCMS.FileManager;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using Mapster;
using MapsterMapper;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace FCMS.Implementations.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;
        private readonly IMapper _mapper;
        private readonly IProductImageRepository _productImagesRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IProductImageRepository _productImageRepository;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IFileManager fileManager, IMapper mapper, IProductImageRepository productImagesRepository, IWebHostEnvironment hostingEnvironment, IProductImageRepository productImageRepository)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
            _mapper = mapper;
            _productImagesRepository = productImagesRepository;
            _hostingEnvironment = hostingEnvironment;
            _productImageRepository = productImageRepository;
        }
        public async Task<BaseResponse<ProductDto>> CreateAsync(CreateProductRequestModel model)
        {
            var checkProduct = await _productRepository.GetAll<Product>();
            foreach (var item in checkProduct)
            {
                if(item.Name == model.Name && item.FarmerId == model.FarmerId)
                {
                    throw new BadRequestException($"Farmer {item.Farmer.User.FirstName} You already have this product leased already kindly update its quantity");
                }
                
            }
            var newProduct = model.Adapt<Product>();
            var uploadPictures = await _fileManager.ListOfFilesToSystem(model.Images);

           
            if(!uploadPictures.Status)
            {
                throw new BadRequestException($"{uploadPictures.Message}");
            }
            
            foreach (var item in uploadPictures.Datas)
            {
                ProductImages productImage = new()
                {
                    ImageReference = item.Name,
                    ProductId = newProduct.Id,
                };
                _productImagesRepository.Insert<ProductImages>(productImage);
            }
            _productRepository.Insert<Product>(newProduct);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<ProductDto>
            {
                Message = "Product Added successfully",
                Status = true,
            };
        }

        public async Task<bool> DeleteAsync(string productId)
        {
            var produtToDelete = await _productRepository.Get<Product>(x => x.Id == productId);
            var imageToDelete = await _productImageRepository.Get(x => x.ProductId == productId);
            foreach (var item in imageToDelete)
            {
                var previousPicturePath = Path.Combine(_hostingEnvironment.WebRootPath, "Documents", item.ImageReference);
                if (File.Exists(previousPicturePath))
                {
                    File.Delete(previousPicturePath);
                    _productImageRepository.Delete<ProductImages>(item);
                }
            }
            if (produtToDelete is null)
            {
                return false;
            }
           
            _productRepository.Delete<Product>(produtToDelete);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<ProductDto>> GetAsync(string param)
        
        {
            var products = _productRepository.QueryWhere<Product>(x => x.FarmerId == param || x.Name == param || x.Id == param || x.Farmer.User.Address.State == param || x.Farmer.User.Address.City == param || x.Farmer.User.FirstName == param);
            if(!products.Any())
            {
                throw new NotFoundException("Product not Found!!!");
            }

            var listOfProducts = new List<ProductDto>();
            foreach (var product in products)
            {
                var productImage = await _productImagesRepository.GetAll(x => x.ProductId == product.Id);


                var productToAdd = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    ImageUrls = productImage.Select(i => i.ImageReference).ToList(),
                    FarmerId = product.FarmerId,
                    Farmer = new Farmer
                    {
                        UserEmail = product.Farmer.UserEmail,
                        UserId = product.Farmer.UserId,
                        User = new User
                        {
                            FirstName = product.Farmer.User.FirstName,
                            LastName = product.Farmer.User.LastName,
                            Email = product.Farmer.User.Email,
                            PhoneNumber = product.Farmer.User.PhoneNumber,
                            ProfilePicture = product.Farmer.User.ProfilePicture,
                            Gender = product.Farmer.User.Gender,
                            Role = product.Farmer.User.Role,
                            Address = new Address
                            {
                                Country = product.Farmer.User.Address.Country,
                                State = product.Farmer.User.Address.State,
                                City = product.Farmer.User.Address.City,
                                Language = product.Farmer.User.Address.Language,
                            },
                        }
                    }
                };

                listOfProducts.Add(productToAdd);

            }

            return listOfProducts;
        }

        public async Task<BaseResponse<ProductDto>> GetByIdAsync(string productId)
        {
            var product = await _productRepository.Get(x => x.Id == productId);
            if(product  is null)
            {
                throw new NotFoundException("No such product!!!");
            }
            var productImage = await _productImagesRepository.GetAll(x => x.ProductId == product.Id);
            return new BaseResponse<ProductDto>
            {
                Message = "Product found!!!",
                Status = true,
                Data = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    ImageUrls = productImage.Select(i => i.ImageReference).ToList(),
                    FarmerId = product.FarmerId,
                    Farmer = new Farmer
                    {
                        UserEmail = product.Farmer.UserEmail,
                        UserId = product.Farmer.UserId,
                        User = new User
                        {
                            FirstName = product.Farmer.User.FirstName,
                            LastName = product.Farmer.User.LastName,
                            Email = product.Farmer.User.Email,
                            PhoneNumber = product.Farmer.User.PhoneNumber,
                            ProfilePicture = product.Farmer.User.ProfilePicture,
                            Gender = product.Farmer.User.Gender,
                            Role = product.Farmer.User.Role,
                            Address = new Address
                            {
                                Country = product.Farmer.User.Address.Country,
                                State = product.Farmer.User.Address.State,
                                City = product.Farmer.User.Address.City,
                                Language = product.Farmer.User.Address.Language,
                            },
                        }
                    }
                }
            };
        }

            public async Task<IReadOnlyList<ProductDto>> GetProductsAsync()
            {
                var products = await _productRepository.GetAll();
                if(!products.Any())
                {
                    return new List<ProductDto>();
                }

                var listOfProducts = new List<ProductDto>();
                foreach (var product in products)
                {
                    var productImage = await _productImagesRepository.GetAll(x => x.ProductId == product.Id);


                    var productToAdd = new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        ImageUrls = productImage.Select(i => i.ImageReference).ToList(),
                        FarmerId = product.FarmerId,
                        Farmer = new Farmer
                        {
                            UserEmail = product.Farmer.UserEmail,
                            UserId = product.Farmer.UserId,
                            User = new User
                            {
                                FirstName = product.Farmer.User.FirstName,
                                LastName = product.Farmer.User.LastName,
                                Email = product.Farmer.User.Email,
                                PhoneNumber = product.Farmer.User.PhoneNumber,
                                ProfilePicture = product.Farmer.User.ProfilePicture,
                                Gender = product.Farmer.User.Gender,
                                Role = product.Farmer.User.Role,
                                Address = new Address
                                {
                                    Country = product.Farmer.User.Address.Country,
                                    State = product.Farmer.User.Address.State,
                                    City = product.Farmer.User.Address.City,
                                    Language = product.Farmer.User.Address.Language,
                                },
                            }
                        }
                    };

                    listOfProducts.Add(productToAdd);

                }

            return listOfProducts;
               
            }

        public async Task<IEnumerable<ProductDto>> GetProductsByAny(string param)
        {
            var products = await _productRepository.GetByAny(x => x.Name == param || x.Farmer.User.FirstName == param || x.FarmerId == param || x.Farmer.UserId == param);
            if (!products.Any())
            {
                throw new NotFoundException("Product not Found!!!");
            }
            var listOfProducts = new List<ProductDto>();
            foreach (var product in products)
            {
                var productImage = await _productImagesRepository.GetAll(x => x.ProductId == product.Id);


                var productToAdd = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    Price = product.Price,
                    ImageUrls = productImage.Select(i => i.ImageReference).ToList(),
                    FarmerId = product.FarmerId,
                    Farmer = new Farmer
                    {
                        UserEmail = product.Farmer.UserEmail,
                        UserId = product.Farmer.UserId,
                        User = new User
                        {
                            FirstName = product.Farmer.User.FirstName,
                            LastName = product.Farmer.User.LastName,
                            Email = product.Farmer.User.Email,
                            PhoneNumber = product.Farmer.User.PhoneNumber,
                            ProfilePicture = product.Farmer.User.ProfilePicture,
                            Gender = product.Farmer.User.Gender,
                            Role = product.Farmer.User.Role,
                            Address = new Address
                            {
                                Country = product.Farmer.User.Address.Country,
                                State = product.Farmer.User.Address.State,
                                City = product.Farmer.User.Address.City,
                                Language = product.Farmer.User.Address.Language,
                            },
                        }
                    }
                };

                listOfProducts.Add(productToAdd);
            }
            return listOfProducts;
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


