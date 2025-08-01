using MediatR;

namespace Application.Abstractions.Messaging;

public interface ITransactionalCommand<TResponse> : IRequest<TResponse>;