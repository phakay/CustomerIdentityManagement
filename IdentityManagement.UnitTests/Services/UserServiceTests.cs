using IdentityManagement.API.Core.Infrastructure;
using IdentityManagement.API.Core.Models;
using IdentityManagement.API.Core.Repositories;
using IdentityManagement.API.Core.Security;
using IdentityManagement.API.Core.Security.Models;
using IdentityManagement.API.Services;
using Moq;
using System.Linq.Expressions;

namespace IdentityManagement.UnitTests.Services
{
    public class UserServiceTests
    {
        private UserService userSvc;
        private Mock<IUserRepository> mockUserRepo;
        private Mock<ILgaRepository> mockLgaRepo;
        private Mock<IPasswordService> mockPasswordSvc;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ISecurityProvider> mockSecurityProvider;
        private Mock<ISmsService> mockSmsService;
        private User fakeUser;
        
        [SetUp]
        public void Setup()
        {
            mockUserRepo = new Mock<IUserRepository>();
            mockLgaRepo = new Mock<ILgaRepository>();
            mockPasswordSvc = new Mock<IPasswordService>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockSecurityProvider = new Mock<ISecurityProvider>();
            mockSmsService = new Mock<ISmsService>();

            userSvc = new UserService(
                mockUserRepo.Object,
                mockPasswordSvc.Object, 
                mockLgaRepo.Object, 
                mockSecurityProvider.Object, 
                mockUnitOfWork.Object, 
                mockSmsService.Object);

            fakeUser = new User 
            { 
                FirstName = "test-firstname", 
                LastName = "test-lastname",
                Username = "test-username"
            };
        }

        [Test]
        public async Task CreateUserAsync_ValidRequest_ShouldCompleteSuccessfully()
        {
            SetupHappyPath_CreateUserAsync();

            var result = await userSvc.CreateUserAsync(fakeUser, "test-password");

            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.Value, Is.Not.Null);
            Assert.That(result.Value.PasswordHash, Is.Not.Null);
            Assert.That(result.Value.IsPhoneNumberVerified, Is.False);

            mockSecurityProvider.Verify(x => x.SetSecurityStampAsync(It.IsAny<User>()), Times.Once);
            mockSecurityProvider.Verify(x => x.GenerateOtp(It.IsAny<User>()), Times.Once);
            mockUserRepo.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
            mockSmsService.Verify(x => x.SendAync(It.IsAny<SecurityMessage>()), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_UsernameAlreadyExists_ShouldReturnFailure()
        {
            SetupHappyPath_CreateUserAsync();

            mockUserRepo.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);

            var result = await userSvc.CreateUserAsync(fakeUser, "test-password");

            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("username exists"));
        }

        [Test]
        public async Task CreateUserAsync_InvalidLga_ShouldReturnFailure()
        {
            SetupHappyPath_CreateUserAsync();

            mockLgaRepo.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Lga, bool>>>())).ReturnsAsync(false);

            var result = await userSvc.CreateUserAsync(fakeUser, "test-password");

            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("lga is invalid"));
        }


        [Test]
        public async Task CreateUserAsync_LgaIsInvalid_ShouldReturnFailure()
        {
            SetupHappyPath_CreateUserAsync();

            mockUserRepo.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);

            var result = await userSvc.CreateUserAsync(fakeUser, "test-password");

            Assert.That(result.IsSuccessful, Is.False);
        }

        [Test]
        public async Task ProcessPhoneNumberVerification_ValidRequest_ShouldReturnSuccessfully()
        {
            SetupHappyPath_ProcessPhoneNumberVerification();

            var result = await userSvc.ProcessPhoneNumberVerification("test-user-email", "test-otp");

            Assert.That(result.IsSuccessful, Is.True);

            mockSecurityProvider.Verify(x => x.SetSecurityStampAsync(It.IsAny<User>()), Times.Once);
            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task ProcessPhoneNumberVerification_PhonenumberAlreadyVerified_ShouldReturnSuccessfully()
        {
            SetupHappyPath_ProcessPhoneNumberVerification();

            fakeUser.IsPhoneNumberVerified = true;

            var result = await userSvc.ProcessPhoneNumberVerification("test-user-email", "test-otp");

            Assert.That(result.IsSuccessful, Is.True);

            mockSecurityProvider.Verify(x => x.SetSecurityStampAsync(It.IsAny<User>()), Times.Never);
            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ProcessPhoneNumberVerificatio_UserEmailDoesNotExist_ShouldReturnFailure()
        {
            SetupHappyPath_ProcessPhoneNumberVerification();

            mockUserRepo.Setup(x => x.FindByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            var result = await userSvc.ProcessPhoneNumberVerification("test-user-email", "test-otp");

            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("user email does not exist"));
        }

        [Test]
        public async Task ProcessPhoneNumberVerificatio_InvalidOtp_ShouldReturnFailure()
        {
            SetupHappyPath_ProcessPhoneNumberVerification();

            mockSecurityProvider.Setup(x => x.ValidateOtp(It.IsAny<User>(), It.IsAny<string>())).Returns(false);

            var result = await userSvc.ProcessPhoneNumberVerification("test-user-email", "test-otp");

            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("invalid otp"));
        }

        [Test]
        public async Task InitiatePhoneNumberVerification_ShouldReturnSuccessfully()
        {
            mockUserRepo.Setup(x => x.FindByUsernameAsync(It.IsAny<string>())).ReturnsAsync(fakeUser);

            var result = await userSvc.InitiatePhoneNumberVerification("test-user-email");

            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(fakeUser.IsPhoneNumberVerified, Is.False);

            mockSmsService.Verify(x => x.SendAync(It.IsAny<SecurityMessage>()), Times.Once);
            mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task InitiatePhoneNumberVerification_UserEmailDoesNotExist_ShouldReturnFailure()
        {
            var result = await userSvc.InitiatePhoneNumberVerification("test-user-email");

            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Empty);
        }

        [Test]
        public async Task GetByUserIdAsync_ShouldReturnSuccessfully()
        {
            mockUserRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(fakeUser);

            var result = await userSvc.GetByUserIdAsync(It.IsAny<int>());

            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.Value, Is.Not.Null);
        }


        [Test]
        public async Task GetByUserIdAsync_UserIdDoesNotExist_ShouldReturnFailure()
        {
            var result = await userSvc.GetByUserIdAsync(It.IsAny<int>());

            Assert.That(result.IsSuccessful, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Empty);
        }

        #region Helper Methods
        private void SetupHappyPath_ProcessPhoneNumberVerification()
        {
            fakeUser.IsPhoneNumberVerified = false;
            mockUserRepo.Setup(x => x.FindByUsernameAsync(It.IsAny<string>())).ReturnsAsync(fakeUser);

            mockSecurityProvider.Setup(x => x.ValidateOtp(It.IsAny<User>(), It.IsAny<string>())).Returns(true);
        }

        private void SetupHappyPath_CreateUserAsync()
        {
            mockUserRepo.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

            mockLgaRepo.Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Lga, bool>>>())).ReturnsAsync(true);

            mockPasswordSvc.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("test-hashed-password");
        }
        #endregion
    }
}