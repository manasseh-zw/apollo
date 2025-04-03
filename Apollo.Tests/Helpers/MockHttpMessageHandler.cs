using System.Net;
using System.Text.Json;

namespace Apollo.Tests.Helpers;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(_handler(request));
    }

    public static MockHttpMessageHandler CreateMockHandler<T>(
        T responseContent,
        HttpStatusCode statusCode = HttpStatusCode.OK
    )
    {
        return new MockHttpMessageHandler(req => new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(JsonSerializer.Serialize(responseContent)),
        });
    }
}
