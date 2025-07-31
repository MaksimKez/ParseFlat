using Application.Dtos;
using Application.Responses;
using MediatR;

namespace Application.Commands.SendVerificationEmail;

public record SendVerificationEmailCommand(SendVerificationEmailRequest Dto) : IRequest<SendVerificationEmailResult>;