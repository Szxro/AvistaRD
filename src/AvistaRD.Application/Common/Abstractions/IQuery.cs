using AvistaRD.SharedKernel;
using MediatR;

namespace AvistaRD.Application.Common.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }