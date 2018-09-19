using FluentAssertions;
using FluentAssertions.Events;
using Identity.DTO;
using MedEasy.Mobile.Core.Apis;
using MedEasy.Mobile.Core.Services;
using MedEasy.Mobile.Core.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Moq.MockBehavior;

namespace MedEasy.Mobile.UnitTests.Core.ViewModels
{
    [UnitTest]
    [Feature("Mobile")]
    public class SignUpViewModelTests : IDisposable
    {
        private readonly ITestOutputHelper _outputHelper;
        private Mock<IAccountsApi> _accountsApiMock;
        private Mock<INavigatorService> _navigationServiceMock;
        private SignUpViewModel _sut;

        public SignUpViewModelTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            _accountsApiMock = new Mock<IAccountsApi>(Strict);
            _navigationServiceMock = new Mock<INavigatorService>(Strict);
            _sut = new SignUpViewModel(_accountsApiMock.Object, _navigationServiceMock.Object);

        }

        public void Dispose()
        {
            _accountsApiMock = null;
            _navigationServiceMock = null;
            _sut = null;

        }

        public static IEnumerable<object[]> SignUpCases
        {
            get
            {
                string[] emails = { string.Empty, " ", "bruce@wayne-enterprise.com" };
                string[] passwords = { string.Empty, " ", "thebatman" };
                string[] confirmPasswords = { string.Empty, " ", "thebatman" };
                string[] usernames = { string.Empty, " ", "thecapedcrusader" };

                return emails.CrossJoin(passwords, usernames, (email, password, username) => (email, password, username))
                    .Where(tuple => string.IsNullOrWhiteSpace(tuple.email)
                        || string.IsNullOrWhiteSpace(tuple.password)
                        || string.IsNullOrWhiteSpace(tuple.username))
                    .CrossJoin(confirmPasswords, (tuple, confirmPassword) => (tuple.email, tuple.username, tuple.password, confirmPassword))
                    .Select(tuple => new object[] { tuple.email, tuple.username, tuple.password, tuple.confirmPassword, false, "Not all required properties are set" })
                    .Concat(new []
                    {
                        new object[] { "dick@wayne-entreprise.com", "robin", "sidekick", "SIDEKICK", false, "Password and confirmPassword don't match" },
                        new object[] { "dick@wayne-entreprise.com", "robin", "sidekick", "sidekick", true, "All required properties are set and passwords match" }
                    });

            }
        }


        [Theory]
        [MemberData(nameof(SignUpCases))]
        public void SignUpCommand_CanExecute(string email, string username, string password,  string confirmPassword, bool expectedCanExecute, string reason)
        {
            // Arrange
            _outputHelper.WriteLine($"input : {new { email, username, password, confirmPassword }.Stringify()}");

            _sut.Email = email;
            _sut.Username = username;
            _sut.Password = password;
            _sut.ConfirmPassword = confirmPassword;


            // Act
            bool canExecute = _sut.SignUpCommand.CanExecute(null);

            // Assert
            canExecute.Should()
                .Be(expectedCanExecute, reason);

        }

        [Fact]
        public async Task SignInCommand_CanExecute_ShouldBeTrue() => _sut.SignInCommand.CanExecute(null)
            .Should().BeTrue();


        [Fact]
        public async Task SignInCommand_NavigatesTo_LoginView()
        {
            // Arrange
            _navigationServiceMock.Setup(mock => mock.NavigateTo<SignInViewModel>())
                .Returns(Task.CompletedTask);

            // Act
            _sut.SignInCommand.Execute(default);

            //Assert
            _navigationServiceMock.Verify(mock => mock.NavigateTo<SignInViewModel>());
        }

        [Fact]
        public async Task SetName_Raise_LoginPropertyChangeEvent()
        {
            // Arrange
            IMonitor<SignUpViewModel> monitor = _sut.Monitor();

            // Act
            _sut.Name = "Joker";

            // Assert

            monitor.Should().RaisePropertyChangeFor(vm => vm.Name);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.ConfirmPassword);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Password);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Email);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Username);
        }

        [Fact]
        public void SetUsername_RaiseUsernamePropertyChangeEvent()
        {
            // Arrange
            IMonitor<SignUpViewModel> monitor = _sut.Monitor();

            // Act
            _sut.Username = "thesmile";

            // Assert
            monitor.Should().RaisePropertyChangeFor(vm => vm.Username);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.ConfirmPassword);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Password);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Email);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Name);
        }

        [Fact]
        public void SetConfirmPassword_RaiseConfirmPasswordPropertyChangeEvent()
        {
            // Arrange
            IMonitor<SignUpViewModel> monitor = _sut.Monitor();

            // Act
            _sut.ConfirmPassword = "thesmile";

            // Assert
            monitor.Should().RaisePropertyChangeFor(vm => vm.ConfirmPassword);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Username);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Password);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Email);
            monitor.Should().NotRaisePropertyChangeFor(vm => vm.Name);
        }

        [Fact]
        public void SignInCommand_GoTo_LoginViewModel()
        {
            // Arrange
            _navigationServiceMock.Setup(mock => mock.NavigateTo<SignInViewModel>())
                .Returns(Task.CompletedTask);

            // Act
            _sut.SignInCommand.Execute(default);

            // Assert
            _navigationServiceMock.Verify(mock => mock.NavigateTo<SignInViewModel>(), Times.Once);

        }

        [Fact]
        public void SignUpCommand_GoTo_HomeViewModel_When_Successfull()
        {
            // Arrange
            _sut.Username = "darkknight";
            _sut.Password = "capedcrusader";
            _sut.ConfirmPassword = _sut.Password;
            _sut.Email = "bruce@wayne-entreprise.com";
            _sut.Name = "Bruce Wayne";

            _accountsApiMock.Setup(mock => mock.SignUp(It.IsAny<NewAccountInfo>(), It.IsAny<CancellationToken>()))
                .Returns((NewAccountInfo newAccountInfo, CancellationToken ct) =>
                {
                    return Task.FromResult(new BearerTokenInfo
                    {
                        AccessToken = $"access-{newAccountInfo.Username}-{newAccountInfo.Password}",
                        RefreshToken = $"refresh-{newAccountInfo.Username}-{newAccountInfo.Password}"
                    });
                });
            _navigationServiceMock.Setup(mock => mock.InsertViewModelBefore<HomeViewModel, SignUpViewModel>());
            _navigationServiceMock.Setup(mock => mock.PopToRootAsync(It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            // Act
            _sut.SignUpCommand.Execute(null);

            // Assert
            _accountsApiMock.Verify(mock => mock.SignUp(It.IsAny<NewAccountInfo>(), It.IsAny<CancellationToken>()), Times.Once);
            _accountsApiMock.Verify(mock => mock.SignUp(It.Is<NewAccountInfo>(input => input.Username == _sut.Username 
                && input.Password == _sut.Password && input.Name == _sut.Name && input.Email == _sut.Email
                && input.ConfirmPassword == _sut.ConfirmPassword), 
                    It.IsAny<CancellationToken>()), Times.Once);
            _navigationServiceMock.Verify(mock => mock.InsertViewModelBefore<HomeViewModel, SignUpViewModel>(), Times.Once);
            _navigationServiceMock.Verify(mock => mock.PopToRootAsync(It.IsAny<bool>()), Times.Once);


        }
    }
}
