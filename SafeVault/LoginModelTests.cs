using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SafeVault.Models;
using Xunit;

public class LoginModelTests
{
    [Fact]
    public async Task Register_SuccessfulRegistration_AssignsUserRole()
    {
        // Arrange
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);
        roleManagerMock.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(true);

        var model = new LoginModel(userManagerMock.Object, roleManagerMock.Object)
        {
            Register = new LoginModel.RegisterModel
            {
                Username = "testuser",
                Password = "Test@1234"
            }
        };

        // Act
        var result = await model.OnPostRegisterAsync();

        // Assert
        Assert.Equal("Registration successful. Please log in.", model.RegisterMessage);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task Register_FailedRegistration_ShowsErrorMessage()
    {
        // Arrange
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));
        roleManagerMock.Setup(x => x.RoleExistsAsync("User")).ReturnsAsync(true);

        var model = new LoginModel(userManagerMock.Object, roleManagerMock.Object)
        {
            Register = new LoginModel.RegisterModel
            {
                Username = "testuser",
                Password = "123"
            }
        };

        // Act
        var result = await model.OnPostRegisterAsync();

        // Assert
        Assert.Contains("Registration failed", model.RegisterMessage);
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task Login_InvalidModelState_ReturnsPage()
    {
        // Arrange
        var userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        var model = new LoginModel(userManagerMock.Object, roleManagerMock.Object);
        model.ModelState.AddModelError("Input.Username", "Required");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }
}
