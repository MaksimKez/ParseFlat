namespace Application.BusinessLogic.Auth.Commands.RegisterUser;

public class RegisterUserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
}