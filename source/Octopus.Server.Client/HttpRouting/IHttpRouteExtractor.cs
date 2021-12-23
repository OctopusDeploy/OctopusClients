using System;
using System.Net.Http;
using Octopus.Server.MessageContracts.Base;

namespace Octopus.Client.HttpRouting
{
    public interface IHttpRouteExtractor
    {
        HttpMethod ExtractHttpMethod<TCommand, TResponse>(ICommand<TCommand, TResponse> command)
            where TCommand : ICommand<TCommand, TResponse>
            where TResponse : IResponse;

        HttpMethod ExtractHttpMethod<TRequest, TResponse>(IRequest<TRequest, TResponse> request)
            where TRequest : IRequest<TRequest, TResponse>
            where TResponse : IResponse;

        Uri ExtractHttpRoute<TCommand, TResponse>(ICommand<TCommand, TResponse> command)
            where TCommand : ICommand<TCommand, TResponse>
            where TResponse : IResponse;

        Uri ExtractHttpRoute<TRequest, TResponse>(IRequest<TRequest, TResponse> request)
            where TRequest : IRequest<TRequest, TResponse>
            where TResponse : IResponse;
    }
}