using LostInSpace.WebApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		private readonly ILogger<GameController> logger;

		public GameController(ILogger<GameController> logger)
		{
			this.logger = logger;
		}

		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{

		}
	}
}
