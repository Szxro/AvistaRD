using AvistaRD.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AvistaRD.Application.Common.Pipelines;

public class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(
        ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {   
        string requestName = typeof(TRequest).Name;

        // TODO: CREATE SERVICE TO GET THE ACTIVE USER USERNAME 
        string userName = "";
        
        _logger.LogInformation(
            "Processing the request {RequestName} requested by the user {User}",
            requestName,
            userName ?? "System");

        TResponse response = await next(cancellationToken);

        if (response.IsSuccess)
        {
            _logger.LogInformation(
                "Completed the request {RequestName} requested by the user {User}",
                requestName,
                userName ?? "System");

            return response;
        }

        _logger.LogWarning(
            "Completed the request {RequestName} requested by the user {User} with an error of type {ErrorName}",
            requestName,
            userName ?? "System",
            response.Error.ErrorCode);

        return response;
    }
}