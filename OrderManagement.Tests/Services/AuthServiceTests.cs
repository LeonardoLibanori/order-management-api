using FluentAssertions;
using Moq;
using OrderManagement.Application.DTOs.Auth;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Tests.Helpers;
using Xunit;

namespace OrderManagement.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authService = new AuthService(_userRepositoryMock.Object, JwtConfigHelper.GetConfiguration());
    }

    // ─── Register ────────────────────────────────────────────

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Password = "123456"
        };

        _userRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Email.Should().Be(dto.Email.ToLower());
        result.Data.Role.Should().Be("User");
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Password = "123456"
        };

        _userRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("E-mail já cadastrado.");
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    // ─── Login ───────────────────────────────────────────────

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "joao@email.com",
            Password = "123456"
        };

        var user = new User
        {
            Id = 1,
            Name = "João Silva",
            Email = "joao@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "joao@email.com",
            Password = "senha_errada"
        };

        var user = new User
        {
            Id = 1,
            Email = "joao@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "User"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("E-mail ou senha inválidos.");
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ShouldReturnFailure()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Email = "naoexiste@email.com",
            Password = "123456"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("E-mail ou senha inválidos.");
    }
}