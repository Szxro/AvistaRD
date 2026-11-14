using AvistaRD.Application.Contracts;
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
    private readonly ICurrentUserService _currentUserService;

    public RequestLoggingPipelineBehavior(
        ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger, 
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {   
        string requestName = typeof(TRequest).Name;
        
        _logger.LogInformation(
            "Processing the request {RequestName} requested by the user {User}",
            requestName,
            _currentUserService.UserName ?? "System");

        TResponse response = await next(cancellationToken);

        if (response.IsSuccess)
        {
            _logger.LogInformation(
                "Completed the request {RequestName} requested by the user {User}",
                requestName,
                _currentUserService.UserName ?? "System");

            return response;
        }

        _logger.LogWarning(
            "Completed the request {RequestName} requested by the user {User} with an error of type {ErrorName}",
            requestName,
            _currentUserService.UserName ?? "System",
            response.Error.ErrorCode);

        return response;
    }
}