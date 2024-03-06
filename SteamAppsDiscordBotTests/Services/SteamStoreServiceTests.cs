using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SteamAppsDiscordBot.Services;
using SteamAppsDiscordBot.Services.DTO;

namespace SteamAppsDiscordBotTests.Services;

public class SteamServicesStoreServiceTests
{
    private readonly SteamStoreService _subjectUnderTest;

    private List<Uri> _mockedGoogleSearchRepositorySearchResults;
    private HttpResponseMessage _mockedHttpClientResponseMessage;
    private readonly Mock<ILogger<SteamStoreService>> _loggerMock = new();

    public SteamServicesStoreServiceTests()
    {
        // Set default mock values.
        _mockedGoogleSearchRepositorySearchResults = new List<Uri>();

        // Mocking Http Client Factory, by mocking the HttpMessageHandler, and injecting the mock into a real HttpClient.
        // And then injecting the HttpClient into the HttpClientFactoryMock.
        // Now we use the _mockedHttpClientResponseMessage to set the response message for the HttpClient.
        _mockedHttpClientResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Default content.")
        };
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        
        // Setup the HttpMessageHandlerMock to return the _mockedHttpClientResponseMessage on "SendAsync".
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => _mockedHttpClientResponseMessage);
        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        
        var httpClientFactoryMock = new Mock<IHttpClientFactory> ();
        httpClientFactoryMock
            .Setup(clientFactory => clientFactory.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        Mock<IGoogleSearchService> googleSearchRepositoryMock = new();
        googleSearchRepositoryMock
            .Setup(searchService => searchService.SearchAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(() => _mockedGoogleSearchRepositorySearchResults);

        // Create subject under test.
        _subjectUnderTest = new SteamStoreService(httpClientFactoryMock.Object, _loggerMock.Object, googleSearchRepositoryMock.Object);
    }
}