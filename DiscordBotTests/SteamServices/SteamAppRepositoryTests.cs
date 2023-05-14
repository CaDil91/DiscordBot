using Microsoft.Extensions.Logging;
using Moq;
using SteamServices;

namespace DiscordBot.SteamServices;

public class SteamAppRepositoryTests
{
    private readonly SteamAppRepository _subjectUnderTest;

    public SteamAppRepositoryTests()
    {
        // Create mocks
        Mock<IHttpClientFactory> httpClientFactoryMock = new();
        httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient()
        {
            BaseAddress = new Uri("https://api.steampowered.com")
        });
        
        Mock<ILogger<SteamAppRepository>> loggerMock = new();
        
        _subjectUnderTest = new SteamAppRepository(httpClientFactoryMock.Object, loggerMock.Object);
    }
    
    [Fact]
    public async Task GetAsync_WithAnInvalidSearchTerm_ReturnsEmptyListOfSteamApps()
    {
        // Arrange. Create Guid to ensure that the search term is invalid.
        var searchTerm = Guid.NewGuid().ToString();
        
        // Act.
        List<SteamApp> steamApps = await _subjectUnderTest.GetAsync(searchTerm);
        
        // Assert.
        Assert.Empty(steamApps);
    }
    
    [InlineData("a")]
    [InlineData("cs")]
    [InlineData("halo")]
    [Theory]
    public async Task GetAsync_WithValidSearchTerm_ReturnsSteamApps(string searchTerm)
    {
        // Arrange.
        
        // Act.
        List<SteamApp> steamApps = await _subjectUnderTest.GetAsync(searchTerm);
        
        // Assert.
        Assert.NotEmpty(steamApps);
    }
    
    [Fact]
    public void TestFilterAppsJson_ReturnsEmptyList_WhenJsonAppsIsEmpty()
    {
        // Arrange
        const string jsonApps = "";
        const string searchTerm = "app";
        const int iMaxReturnCount = 10;

        // Act
        IEnumerable<string> result = _subjectUnderTest.FilterAppsJson(jsonApps, searchTerm, iMaxReturnCount);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void TestFilterAppsJson_RemovesAppsThatDoNotContainWholeSearchTerm()
    {
        // Arrange
        const string jsonApps = @"{""applist"":{""apps"":[{""name"":""apple""},{""name"":""banana""},{""name"":""grapefruit""}]}}";
        const string searchTerm = "apple";
        const int iMaxReturnCount = 10;

        // Act
        List<string> result = _subjectUnderTest.FilterAppsJson(jsonApps, searchTerm, iMaxReturnCount).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("{\r\n  \"name\": \"apple\"\r\n}", result.First());
    }

    [Fact]
    public void TestFilterAppsJson_ReturnsBestMatchedAppNames()
    {
        /*// Arrange
        var jsonApps = @"{""applist"":{""apps"":[{""name"":""apple""},{""name"":""banana""},{""name"":""grapefruit""}]}}";
        var searchTerm = "apl";
        var iMaxReturnCount = 2;

        // Mock FuzzySharp's PartialRatio method
        var mockFuzz = new Mock<IFuzz>();
        mockFuzz.Setup(f => f.PartialRatio(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((s1, s2) => s1.StartsWith(s2) ? 100 : 0);

        // Inject the mocked FuzzySharp instance
        var myClass = new MyClass(mockFuzz.Object);

        // Act
        var result = myClass.FilterAppsJson(jsonApps, searchTerm, iMaxReturnCount);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("apple", result.First());
        Assert.Equal("grapefruit", result.Last());*/
    }
}