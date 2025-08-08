using Application.Abstractions.Messaging;
using Application.Responses;
using Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull 
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var isTransactional = request is ITransactionalCommand<TResponse>;

        if (!isTransactional)
        {
            return await next(cancellationToken);
        }

        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Starting transaction for {RequestName}", requestName);

        using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next(cancellationToken);

            if (response is IResult { IsSuccess: false })
            {
                logger.LogWarning("Operation failed for {RequestName}, rolling back transaction", requestName);
                await transaction.RollbackAsync(cancellationToken);
                return response;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Transaction committed successfully for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Transaction rolled back for {RequestName}", requestName);
            throw;
        }
    }

}
