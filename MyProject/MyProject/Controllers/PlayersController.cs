using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyProject.Utility;

namespace MyProject.Controllers
{
    [ApiController]
    [Route("MyProject/[controller]/[Action]")]
    public class PlayersController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<PlayersController> _logger;


        public PlayersController(HttpClient httpClient, IMemoryCache memoryCache, ILogger<PlayersController> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            try
            {
                _logger.LogInformation("Fetching players from cache or external source.");
                var players = await Utils.GetPlayersFromCacheAsync(_httpClient, _memoryCache);

                // Sort the players by ID
                var sortedPlayers = players?.OrderBy(p => p.Id).ToList();
                _logger.LogInformation($"Successfully fetched {sortedPlayers?.Count} players.");

                return Ok(sortedPlayers);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error occured when getting all players, Message: {ex.Message} , Trace {ex.StackTrace}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerById(int id)
        {

            try
            {
                _logger.LogInformation("Fetching player with ID {Id}.", id);
                var players = await Utils.GetPlayersFromCacheAsync(_httpClient, _memoryCache);

                // Find the player with ID
                var player = players?.FirstOrDefault(p => p.Id == id);

                if (player == null)
                {
                    _logger.LogWarning($"Player with ID {id} not found.");
                    return NotFound($"Player with ID {id} not found.");
                }
                _logger.LogInformation($"Player with ID {id} retrieved successfully.");
                return Ok(player);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error occured when getting palyer with Id {id}, Message: {ex.Message}, Trace : {ex.StackTrace}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerById(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete player with ID {id}.");
                var players = await Utils.GetPlayersFromCacheAsync(_httpClient, _memoryCache);

                // Find the player with ID
                var player = players?.FirstOrDefault(p => p.Id == id);

                if (player == null)
                {
                    _logger.LogWarning($"Attempted to delete non-existent player with ID {id}.");
                    return NotFound($"Player with ID {id} not found.");
                }

                //remove the player from the memory list
                players?.Remove(player);
                _memoryCache.Set("Key##@0", players);

                _logger.LogInformation($"Player with ID {id} has been deleted.");
                return Ok($"Player with ID {id} has been deleted.");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error occured when deleting player with Id {id} from the list, Message: {ex.Message}, Trace : {ex.StackTrace}");
            }

        }


    }
}
