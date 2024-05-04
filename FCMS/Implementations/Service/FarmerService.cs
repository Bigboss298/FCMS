using FCMS.FileManager;
using FCMS.Gateway.EmailService;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Model.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Abstractions;
using MySqlX.XDevAPI;
using Paystack.Net.SDK;
using Paystack.Net.SDK.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FCMS.Implementations.Service
{
    public class FarmerService : IFarmerService
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;
        private readonly IMailService _mailServices;
        private readonly IAddressRepository _addressRepository;
        private static HttpClient _client;
        private readonly string _secretKey;
        private readonly IPaymentDetails _paymentDetails;


        public FarmerService(IConfiguration configuration, IFarmerRepository farmerRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IFileManager fileManager, IAddressRepository addressRepository, IPaymentDetails paymentDetails,IMailService mailService)
        {
            _farmerRepository = farmerRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
            _addressRepository = addressRepository;
            _secretKey = configuration["Paystack:ApiKey"];
            _client = new HttpClient();
            _paymentDetails = paymentDetails;
            _mailServices = mailService;
        }

        public async Task<BaseResponse<FarmerDto>> CreateAsync(CreateFarmerRequestModel model)
        {

            if (model is null) throw new BadRequestException($"Model can't be null");
            var checkFarmer = await _userRepository.Get<User>(x => x.Email == model.Email);
            if(checkFarmer != null)
            {
                throw new Exception($"User with the Email {model.Email} already exists");
            }

            var newUser = model.Adapt<User>();
            var newAddress = model.Adapt<Address>();
            var newPaymentDetails = model.Adapt<PaymentDetails>();
            var newFarmer = new Farmer
            {
                UserEmail = newUser.Email,
                UserId = newUser.Id,
                User = newUser,
            };

            var farmerImage = await _fileManager.UploadFileToSystem(model.ProfilePicture);
            if(!farmerImage.Status)
            {
                throw new Exception($"{farmerImage.Message}");
            };

            var recipientCode = await CreateTransferRecipient(_secretKey, model.AccountName, model.AccountNumber, model.BankCode, "NGN");

            var mailRequest = new MailRequestDto
            {
                Subject = "Welcome to FCMS Farmer to Customer Management System!",
                ToEmail = newUser.Email,
                ToName = newUser.FirstName,
                HtmlContent = $@"
                        <html>
                            <body>
                                <h2>Dear {newUser.FirstName},</h2>
                                <p>Welcome to our Farmer to Customer Management System! Get ready for streamlined farm management, easy sales, real-time insights and happy customers. We're here to support you every step of the way.</p>
                                <p>Happy farming!</p>
                                <p>Best regards,<br/>Management<br/>FCMS</p>
                            </body>
                        </html>"
            };




            newUser.ProfilePicture = farmerImage.Data.Name;
            newUser.Farmer = newFarmer;
            newFarmer.User = newUser;
            newFarmer.UserId = newUser.Id;
            newFarmer.PaymentDetailId = newPaymentDetails.Id;
            newFarmer.PaymentDetails = newPaymentDetails;
            newUser.Address = newAddress;
            newAddress.UserId = newUser.Id;
            newPaymentDetails.Name = model.AccountName;
            newPaymentDetails.Type = Model.Enum.BankType.Nuban;
            newPaymentDetails.Recipient_Code = recipientCode;
            newPaymentDetails.Currency = "NGN";
            newPaymentDetails.Farmer = newFarmer;
            newPaymentDetails.FarmerId = newFarmer.Id;




            _farmerRepository.Insert<Farmer>(newFarmer);
            _userRepository.Insert<User>(newUser);
            _addressRepository.Insert<Address>(newAddress);
            _paymentDetails.Insert<PaymentDetails>(newPaymentDetails);
            _mailServices.SendEmailAsync(mailRequest);


            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<FarmerDto>
            {
                Message = "Farmer registered Successfully",
                Status = true
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var farmerToDelete = await _farmerRepository.Get<Farmer>(x => x.Id == id);
            var userToDelete = await _userRepository.Get<User>(x => x.Farmer.Id == id);
            if(farmerToDelete is null || userToDelete is null)
            {
                throw new Exception("User doesnt exits!!!");
            }
            _farmerRepository.Delete<Farmer>(farmerToDelete);
            _userRepository.Delete<User>(userToDelete);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<BaseResponse<FarmerDto>> GetAsync(string id)
        {
            var farmer = await _farmerRepository.Get(x => x.Id == id  || x.UserId == id);
            if(farmer  is null)
            {
                throw new NotFoundException("Farmer not Found!!!");
            }
            return new BaseResponse<FarmerDto>
            {
                Message = $"Farmer with the Id {id} has been found successfully",
                Status = true,
                Data = new FarmerDto
                {
                    FarmerId = farmer.Id,
                    UserEmail = farmer.UserEmail,
                    UserId = farmer.User.Id,
                    User = new User
                    {
                        Id = farmer.Id,
                        FirstName = farmer.User.FirstName,
                        LastName = farmer.User.LastName,
                        Email = farmer.User.Email,
                        Password = farmer.User.Password,
                        PhoneNumber = farmer.User.PhoneNumber,
                        ProfilePicture = farmer.User.ProfilePicture,
                        Gender = farmer.User.Gender,
                        Role = farmer.User.Role,
                        Address = new Address()
                        {
                            Country = farmer.User.Address.Country,
                            City = farmer.User.Address.City,
                            State = farmer.User.Address.State,
                            Language = farmer.User.Address.Language,
                            UserId = farmer.User.Id
                        }
                    },
                },
            };
        }

        public async Task<IReadOnlyList<FarmerDto>> GetFarmersAsync()
        {
            var farmers = await _farmerRepository.GetAll();
            if(!farmers.Any())
            {
                return new List<FarmerDto>();
            }
            return farmers.Select(x => new FarmerDto
            {
                FarmerId = x.Id,
                UserEmail = x.UserEmail,
                UserId = x.UserId,
                User = new User()
                {
                    Id = x.User.Id,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    PhoneNumber = x.User.PhoneNumber,
                    ProfilePicture = x.User.ProfilePicture,
                    Gender = x.User.Gender,
                    Role = x.User.Role,
                    Address = new Address()
                    {
                        Country = x.User.Address.Country,
                        City = x.User.Address.City,
                        State = x.User.Address.State,
                        Language = x.User.Address.Language,
                        UserId = x.User.Id
                    }
                },
            }).ToList();
        }

        public async Task<BaseResponse<FarmerDto>> UpdateAsync(string id, UpdateFarmerRequestModel model)
        {
            var farmer = await _farmerRepository.Get<Farmer>(x => x.Id == id);
            if(farmer is null)
            {
                return new BaseResponse<FarmerDto>
                {
                    Message = $"No farmer with the Id {id} exist",
                    Status = false,
                };
            }
            farmer.UserEmail = model.UserEmail;
            _farmerRepository.Update<Farmer>(farmer);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponse<FarmerDto>
            {
                Message = $"Farmer {farmer.User.FirstName} profile Updated Succesfully",
                Status = true,
            };

        }


        //private async Task<string> GetRecipientCode(PaymentDetailsDto model)
        //{
        //    var url = "https://api.paystack.co/transferrecipient";
        //    var authorization = "Bearer " + _secretKey;
        //    var contentType = "application/json";
        //    var data = new
        //    {
        //        type = "nuban",
        //        name = model.AccountName,
        //        account_number = model.AccountNumber,
        //        bank_code = model.BankCode,
        //        currency = "NGN",
        //    };

        //    using (var httpClient = new HttpClient())
        //    {
        //        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization", "Bearer sk_test_81ea41faa2918934deb1efb644a4b94217ebdf48");

        //        var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        //        var content = new StringContent(json, Encoding.UTF8, contentType);

        //        HttpResponseMessage response = await httpClient.PostAsync(url, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var resultJson = await response.Content.ReadAsStringAsync();
        //            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<RecipientData>(resultJson);
        //            return result.recipient_code;
        //        }
        //        else
        //        {
        //            throw new BadRequestException("Recipient Code could not be generated");
        //        }
        //    }
        //}
        public static async Task<string> CreateTransferRecipient(string secretKey, string recipientName, string accountNumber, string bankCode, string currency)
        {
            string url = "https://api.paystack.co/transferrecipient";
            string authorization = $"Bearer {secretKey}";
            string contentType = "application/json";

            string jsonData = $@"
        {{
          ""type"": ""nuban"",
          ""name"": ""{recipientName}"",
          ""account_number"": ""{accountNumber}"",
          ""bank_code"": ""{bankCode}"",
          ""currency"": ""{currency}""
        }}";

            var content = new StringContent(jsonData, Encoding.UTF8, contentType);

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", authorization);

            HttpResponseMessage response = await _client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                JsonElement root = jsonDoc.RootElement;
                JsonElement recipientCodeElement = root.GetProperty("data").GetProperty("recipient_code");
                string recipientCode = recipientCodeElement.GetString();
                return recipientCode;
            }
            else
            {
                throw new BadRequestException(response.ReasonPhrase);
            }
        }
    }
}
