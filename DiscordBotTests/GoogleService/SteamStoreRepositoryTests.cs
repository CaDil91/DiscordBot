using System.Net;
using Azure.Security.KeyVault.Secrets;
using GoogleService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace DiscordBot.GoogleService;

public class SteamStoreRepositoryTests
{
    private const string CUSTOM_SEARCH_GOOGLEAPIS_URL = "https://content-customsearch.googleapis.com/customsearch/v1";
    
    private readonly Mock<ILogger<SteamStoreRepository>> _loggerMock;
    private readonly Mock<IOptions<GoogleOptions>> _googleOptionsMock;
    private readonly Mock<SecretClient> _secretClientMock;
    
    private HttpResponseMessage _httpResponseMessage = new(HttpStatusCode.OK)
    {
        Content = new StringContent("Default response content.")
    };
    
    private readonly SteamStoreRepository _subjectUnderTest;

    public SteamStoreRepositoryTests()
    {
        // Setup _googleOptionsMock.
        _googleOptionsMock = new Mock<IOptions<GoogleOptions>>();
        _googleOptionsMock.SetupGet(_ => _.Value).Returns(new GoogleOptions()
        {
            Token = "Google",
            SteamStoreCx = "3533c3e3e23024252"
        });

        // Setup _httpClientFactoryMock.
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => _httpResponseMessage);
        
        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(CUSTOM_SEARCH_GOOGLEAPIS_URL)
        };
        
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(_ => _.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Setup _secretClientMock.
        _secretClientMock = new Mock<SecretClient>();

        // Setup _loggerMock.
        _loggerMock = new Mock<ILogger<SteamStoreRepository>>();

        // Setup _subjectUnderTest.
        _subjectUnderTest = new SteamStoreRepository(_googleOptionsMock.Object, httpClientFactoryMock.Object,
            _secretClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_LogsError_OnHttpError()
    {
        // Arrange.
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Failed to send http request."))
            .Verifiable();
        
        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(CUSTOM_SEARCH_GOOGLEAPIS_URL)
        };
        
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var steamStoreRepository = new SteamStoreRepository(_googleOptionsMock.Object, httpClientFactoryMock.Object,
            _secretClientMock.Object, _loggerMock.Object);

        // Act.
        await steamStoreRepository.GetCustomSearchResultsAsync("query");

        // Assert.
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Failed to send http request."),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ReturnsEmptyList_OnHttpError()
    {
        // Arrange.
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new Exception("Failed to send http request."))
            .Verifiable();
        
        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(CUSTOM_SEARCH_GOOGLEAPIS_URL)
        };
        
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var steamStoreRepository = new SteamStoreRepository(_googleOptionsMock.Object, httpClientFactoryMock.Object,
            _secretClientMock.Object, _loggerMock.Object);
        
        // Act.
        List<string> customSearchResults = await steamStoreRepository.GetCustomSearchResultsAsync("halo");

        // Assert.
        Assert.Empty(customSearchResults);
    }

    [Fact]
    public async Task GetCustomSearchResultsAsync_ReturnsListOfResults()
    {
        // Arrange.
        _httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"items\": [{\"link\": \"https://www.halowaypoint.com/en-us/games/halo-5-guardians\"}]}")
        };

        // Act.
        List<string> customSearchResults = await _subjectUnderTest.GetCustomSearchResultsAsync("halo");

        // Assert.
        Assert.NotEmpty(customSearchResults);
        Assert.Equal("https://www.halowaypoint.com/en-us/games/halo-5-guardians", customSearchResults[0]);
    }

    /*
    [Fact]
    public async Task SetKeyAsync_ReturnsFalseOnFailure()
    {
        // Arrange. TODO: mock in constructor.
        _secretClientMock.Setup(x => x.SetSecretAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception("Any Exception"));
        
        _subjectUnderTest = new SteamStoreRepository(_googleOptionsMock.Object, _httpClientFactoryMock.Object, 
            _secretClientMock.Object, _loggerMock.Object);

        // Act.
        bool result = await _subjectUnderTest.SetKeyAsync("INVALID ARGUMENT");

        // Assert.
        Assert.False(result);
        _loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString() == "Failed to get secret from vault."),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }*/
}