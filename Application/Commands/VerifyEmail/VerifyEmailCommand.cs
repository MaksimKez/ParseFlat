using Application.Responses;
using MediatR;

namespace Application.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<VerifyEmailResult>;