using FluentValidation;
using RentCarServer.Application.Services;
using RentCarServer.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TS.MediatR;
using TS.Result;

namespace RentCarServer.Application.Auth;

public sealed record LoginCommand(
    string EmailOrUsername,
    string Password) : IRequest<Result<string>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(p => p.EmailOrUsername).NotEmpty().WithMessage("Geçerli bir mail ya da kullanıcı adı giriniz");
        RuleFor(p => p.Password).NotEmpty().WithMessage("Geçerli bir şifre giriniz");
    }
}


public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(p => p.Email.Value == request.EmailOrUsername || p.UserName.Value == request.EmailOrUsername);

        if (user is null)
        {
            return Result<string>.Failure("Kullanıcı adı ya da şifre yanlış");
        }

        var token = jwtProvider.CreateToken(user);
        return "token";
    }

}