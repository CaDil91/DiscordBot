using System.Net;
using GoogleService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace DiscordBot.GoogleService;

public class GoogleCustomSearchServiceTests
{
    private const string CUSTOM_SEARCH_GOOGLEAPIS_URL = "https://content-customsearch.googleapis.com/customsearch/v1";

    private readonly Mock<ILogger<GoogleCustomSearchService>> _loggerMock;
    private readonly Mock<IOptions<GoogleOptions>> _googleOptionsMock;

    private readonly HttpResponseMessage _mockedHttpClientsResponseMessage = new(HttpStatusCode.OK)
    {
        Content = new StringContent("Default response content.")
    };

    private readonly GoogleCustomSearchService? _subjectUnderTest;

    public GoogleCustomSearchServiceTests()
    {
        // Setup _googleOptionsMock.
        _googleOptionsMock = new Mock<IOptions<GoogleOptions>>();
        _googleOptionsMock.SetupGet(_ => _.Value).Returns(new GoogleOptions()
        {
            Key = "Google",
            SteamStoreCx = "3533c3e3e23024252"
        });

        // Setup _httpClientFactoryMock.
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => _mockedHttpClientsResponseMessage);

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(CUSTOM_SEARCH_GOOGLEAPIS_URL)
        };
        Mock<IHttpClientFactory> httpClientFactoryMock = new();
        httpClientFactoryMock
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Setup _loggerMock.
        _loggerMock = new Mock<ILogger<GoogleCustomSearchService>>();

        // TODO: Testing a singleton.
        _subjectUnderTest = null;
        
        // Setup _subjectUnderTest.
        _subjectUnderTest = new GoogleCustomSearchService(_googleOptionsMock.Object, httpClientFactoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_LogsError_OnHttpRequestException()
    {
        // Arrange.
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Unexpected exception."))
            .Verifiable();
        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(CUSTOM_SEARCH_GOOGLEAPIS_URL)
        };
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var steamStoreRepository = new GoogleCustomSearchService(_googleOptionsMock.Object,
            httpClientFactoryMock.Object, _loggerMock.Object);

        // Act.
        // Assert.
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            steamStoreRepository.GetCustomSearchResultsAsync("Any query."));
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Request to Google Custom Search API failed."),
                It.IsAny<HttpRequestException>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_LogsCritical_OnUnexpectedException()
    {
        // Arrange.
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Unexpected exception."))
            .Verifiable();
        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(CUSTOM_SEARCH_GOOGLEAPIS_URL)
        };
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var steamStoreRepository = new GoogleCustomSearchService(_googleOptionsMock.Object,
            httpClientFactoryMock.Object, _loggerMock.Object);

        // Act.
        // Assert.
        await Assert.ThrowsAsync<Exception>(() => steamStoreRepository.GetCustomSearchResultsAsync("Any query."));
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Critical),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Failed to send http request."),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_LogsWarning_OnUnsuccessfulStatusCodeResponse()
    {
        // Arrange.
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.BadRequest;

        // Act.
        await _subjectUnderTest?.GetCustomSearchResultsAsync("Any query.")!;

        // Assert.
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!
                    .Contains($"Failed to get results from google. Status Code: {HttpStatusCode.BadRequest}. " +
                              $"Reason: {It.IsAny<string?>()}")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ReturnsEmptyList_OnUnsuccessfulStatusCodeResponse()
    {
        // Arrange.
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.BadRequest;

        // Act.
        List<Uri> result = await _subjectUnderTest!.GetCustomSearchResultsAsync("Any query.").ConfigureAwait(false);

        // Assert.
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ThrowsException_IfFailingToDeserializeResponseAsGoogleResult()
    {
        // Arrange.
        const string sContent = "Bad content.";
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.OK;
        _mockedHttpClientsResponseMessage.Content = new StringContent(sContent);

        // Act.
        await Assert.ThrowsAsync<Newtonsoft.Json.JsonReaderException>(() =>
            _subjectUnderTest!.GetCustomSearchResultsAsync("Any query."));

        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() == "Failed to deserialize google response."),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ThrowsException_IfItemsCantBeRetrievedFromGoogleResult()
    {
        // Arrange.
        const string sContent = "{\"items\": null}";
        const string sQuery = "Any query.";
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.OK;
        _mockedHttpClientsResponseMessage.Content = new StringContent(sContent);

        // Act.
        await Assert.ThrowsAsync<ArgumentException>(() => _subjectUnderTest!.GetCustomSearchResultsAsync(sQuery));
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString() == $"Failed to get items from google search: {sQuery}."),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ReturnsEmptyList_WhenResultContainsNoUris()
    {
        // Arrange
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.OK;
        _mockedHttpClientsResponseMessage.Content = new StringContent(@"{ ""items"": [] }");

        // Act
        List<Uri> result = await _subjectUnderTest!.GetCustomSearchResultsAsync("Any query.").ConfigureAwait(false);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ReturnsEmptyList_WhenResultContainsInvalidUris()
    {
        // Arrange
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.OK;
        _mockedHttpClientsResponseMessage.Content =
            new StringContent(@"{ ""items"": [ { ""link"": ""Invalid uri"" } ] }");

        // Act
        List<Uri> result = await _subjectUnderTest!.GetCustomSearchResultsAsync("Any query.").ConfigureAwait(false);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ReturnsListOfRequestedSize_WhenResultsContainEnoughValidUris()
    {
        // Arrange
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.OK;
        _mockedHttpClientsResponseMessage.Content = new StringContent(
            @"{ ""items"": [ 
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" }            
            ] }");

        // Act
        List<Uri> result = await _subjectUnderTest!.GetCustomSearchResultsAsync("Any query.", 5).ConfigureAwait(false);

        // Assert
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ReturnsAllItsResults_WhenResultsDoesntContainEnoughValidUris()
    {
        // Arrange.
        _mockedHttpClientsResponseMessage.StatusCode = HttpStatusCode.OK;
        _mockedHttpClientsResponseMessage.Content = new StringContent(
            @"{ ""items"": [ 
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },
                { ""link"": ""https://example.com/1"" },         
            ] }");


        // Act.
        List<Uri> result = await _subjectUnderTest!.GetCustomSearchResultsAsync("Any query.", 5).ConfigureAwait(false);

        // Assert.
        Assert.Equal(3, result.Count);
    }
}