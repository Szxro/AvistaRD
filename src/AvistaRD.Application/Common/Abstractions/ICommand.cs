using AvistaRD.SharedKernel;
using MediatR;

namespace AvistaRD.Application.Common.Abstractions;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand { }

public interface ICommand : IRequest<Result>, IBaseCommand { }

public interface IBaseCommand { }