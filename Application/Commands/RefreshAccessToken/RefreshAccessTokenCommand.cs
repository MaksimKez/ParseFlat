using Application.Responses;
using MediatR;

namespace Application.Commands.RefreshAccessToken;

public record RefreshAccessTokenCommand(string RefreshToken) : IRequest<RefreshTokenResult>;
