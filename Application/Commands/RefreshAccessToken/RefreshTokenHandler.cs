using Application.Abstractions.JWT;
using Application.Responses;
using Domain.Abstractions;
using MediatR;

namespace Application.Commands.RefreshAccessToken;

public class RefreshAccessTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtGenerator refreshTokenGenerator)
    : IRequestHandler<RefreshAccessTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await unitOfWork.RefreshTokens.FindByTokenAsync(request.RefreshToken, cancellationToken);
        if (refreshToken is null)
            return RefreshTokenResult.Failure("Refresh token not found");

        if (refreshToken.ExpiresAt < DateTime.UtcNow)
            return RefreshTokenResult.Failure("Refresh token has expired");

        if (refreshToken.User is null)
            return RefreshTokenResult.Failure("User not found");

        //! this is temporary, because user cannot be verified until NotificationService is done
        //if (!refreshToken.User.IsVerified)
        //    return RefreshTokenResult.Failure("User is not verified");

        if (refreshToken.User.IsDeleted)
            return RefreshTokenResult.Failure("User is deleted");

        var accessToken = refreshTokenGenerator.GenerateAccessToken(refreshToken.User);

        return RefreshTokenResult.Success(accessToken);
    }
}
