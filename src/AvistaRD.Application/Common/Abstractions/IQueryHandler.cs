using AvistaRD.SharedKernel;
using MediatR;

namespace AvistaRD.Application.Common.Abstractions;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{ }