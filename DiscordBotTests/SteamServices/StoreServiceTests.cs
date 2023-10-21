using System.Net;
using GoogleService;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using SteamServices.DTOs;
using SteamServices.Services;

namespace DiscordBot.SteamServices;

public class SteamServicesStoreServiceTests
{
    private readonly SteamStoreService _subjectUnderTest;

    private List<Uri> _mockedGoogleSearchRepositorySearchResults;
    private HttpResponseMessage _mockedHttpClientResponseMessage;

    public SteamServicesStoreServiceTests()
    {
        // Set default mock values.
        _mockedGoogleSearchRepositorySearchResults = new List<Uri>();

        // Create mocks
        _mockedHttpClientResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Default content.")
        };
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => _mockedHttpClientResponseMessage);
        var httpClient = new HttpClient(httpMessageHandlerMock.Object);
        Mock<IHttpClientFactory> httpClientFactoryMock = new();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        Mock<ILogger<SteamStoreService>> loggerMock = new();

        Mock<IGoogleSearchRepository> googleSearchRepositoryMock = new();
        googleSearchRepositoryMock.Setup(_ => _.GetCustomSearchResultsAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(() => _mockedGoogleSearchRepositorySearchResults);

        // Create subject under test.
        _subjectUnderTest = new SteamStoreService(httpClientFactoryMock.Object, loggerMock.Object,
            googleSearchRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAppsAsync_WithNoSearchResults_ReturnsEmptyList()
    {
        // Arrange
        _mockedGoogleSearchRepositorySearchResults = new List<Uri>();

        // Act
        List<SteamApp> steamApps = await _subjectUnderTest.GetAppsAsync();

        // Assert
        Assert.Empty(steamApps);
    }

    [Fact]
    public async Task GetAppsAsync_WithInvalidSearchUris_ReturnsEmptyList()
    {
        // Arrange
        _mockedGoogleSearchRepositorySearchResults = new List<Uri>
        {
            new("https://store.steampowered.com/app/"),
            new("nope"),
            new("https://store.steampowered.com/"),
        };

        // Act
        List<SteamApp> steamApps = await _subjectUnderTest.GetAppsAsync();

        // Assert
        Assert.Empty(steamApps);
    }

    [Fact]
    public async Task GetAppsAsync_WithValidSearchUris_ReturnsSteamApps()
    {
        // Arrange
        _mockedGoogleSearchRepositorySearchResults = new List<Uri>
        {
            new("https://store.steampowered.com/app/10"),
            new("https://store.steampowered.com/app/10"),
            new("https://store.steampowered.com/app/10"),
        };

        // Act
        List<SteamApp> steamApps = await _subjectUnderTest.GetAppsAsync();

        // Assert
        Assert.NotEmpty(steamApps);
    }

    [InlineData("-1234")]
    [InlineData("0")]
    [InlineData("Text and stuff. Well no, just text.")]
    [Theory]
    public async Task GetSteamAppAsync_WithAnInvalidAppId_ReturnsNull(string appId)
    {
        // Arrange
        // Act
        SteamApp? steamApp = await _subjectUnderTest.GetSteamAppAsync(appId);

        // Assert
        Assert.Null(steamApp);
    }

    [Fact]
    public async Task GetSteamAppAsync_WithNonJsonResponse_ReturnsNull()
    {
        // Arrange
        _mockedHttpClientResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Not json.")
        };

        // Act
        SteamApp? steamApp = await _subjectUnderTest.GetSteamAppAsync("10");

        // Assert
        Assert.Null(steamApp);
    }

    [Fact]
    public async Task GetSteamAppAsync_WithEmptyJsonResponse_ReturnsNull()
    {
        // Arrange
        _mockedHttpClientResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{}")
        };

        // Act
        SteamApp? steamApp = await _subjectUnderTest.GetSteamAppAsync("Any value.");

        // Assert
        Assert.Null(steamApp);
    }

    [Fact]
    public async Task GetSteamAppSync_WithValidJsonResponse_ReturnsSteamAppWithExpectedValues()
    {
        // Arrange
        _mockedHttpClientResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(await File.ReadAllTextAsync("../../../TestData/SteamAppResponse.json"))
         };
        
        // Act
        SteamApp? steamApp = await _subjectUnderTest.GetSteamAppAsync("10");
        
        // Assert
    }
    
    [Fact]
    public async Task QueryApiAsync_WithExpectedHttpResponse_ReturnsExpectedJson()
    {
        // Arrange
        _mockedHttpClientResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(await File.ReadAllTextAsync("../../../TestData/SteamAppResponse.json"))
        };
        
        // Act
        string? json = await _subjectUnderTest.QueryApiAsync(new Uri("https://store.steampowered.com/api/appdetails?appids=10"));
        
        // Assert
        Assert.Equal(await File.ReadAllTextAsync("../../../TestData/SteamAppResponse.json"), json);
    }
}