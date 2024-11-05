using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using MyProject.Controllers;
using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMyProject.Controllers
{
    public class PlayersControllerTests
    {

        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly ILogger<PlayersController> _logger;
        private readonly HttpClient _httpClient;

        public PlayersControllerTests()
        {
            _mockMemoryCache = new Mock<IMemoryCache>();
            _logger = new NullLogger<PlayersController>(); 
            _httpClient = new HttpClient();
        }

        [Fact]
        public async Task GetPlayers_ReturnsOkResult_WithPlayersList()
        {
            // Arrange
            var mockPlayers = new List<Player>
            {
                new Player { Id = 1, FirstName = "Novak", LastName = "Djokovic" },
                new Player { Id = 2, FirstName = "Roger", LastName = "Federer" }
            };

            _mockMemoryCache
                            .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                            .Returns((object key, out object cachedValue) =>
                            {
                                cachedValue = mockPlayers;
                                return true;
                            });

            var controller = new PlayersController(_httpClient, _mockMemoryCache.Object, _logger);

            // Act
            var result = await controller.GetPlayers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Player>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetPlayerById_PlayerExists_ReturnsOkResult_WithPlayer()
        {
            // Arrange
            var playerId = 1;
            var mockPlayers = new List<Player>
            {
                new Player { Id = 1, FirstName = "Novak", LastName = "Djokovic" },
                new Player { Id = 2, FirstName = "Roger", LastName = "Federer" }
            };

            _mockMemoryCache
                           .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                           .Returns((object key, out object cachedValue) =>
                           {
                               cachedValue = mockPlayers;
                               return true;
                           });

            var controller = new PlayersController(_httpClient, _mockMemoryCache.Object, _logger);

            // Act
            var result = await controller.GetPlayerById(playerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var player = Assert.IsType<Player>(okResult.Value);
            Assert.Equal(playerId, player.Id);
        }

        [Fact]
        public async Task GetPlayerById_PlayerDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var playerId = 99;
            var mockPlayers = new List<Player>
            {
                new Player { Id = 1, FirstName = "Novak", LastName = "Djokovic" },
                new Player { Id = 2, FirstName = "Roger", LastName = "Federer" }
            };

            _mockMemoryCache
                           .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                           .Returns((object key, out object cachedValue) =>
                           {
                               cachedValue = mockPlayers;
                               return true;
                           });

            var controller = new PlayersController(_httpClient, _mockMemoryCache.Object, _logger);

            // Act
            var result = await controller.GetPlayerById(playerId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

