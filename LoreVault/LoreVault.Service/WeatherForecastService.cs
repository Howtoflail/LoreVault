using LoreVault.Domain.Interfaces;
using LoreVault.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoreVault.Service
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IWeatherForecastRepository _repository;

        public WeatherForecastService(IWeatherForecastRepository repository)
        {
            _repository = repository;
        }

        public List<WeatherForecast> ProcessFTemperature()
        {
            var forecasts = _repository.GetForecasts();
            var processed = new List<WeatherForecast>();
            foreach (var forecast in forecasts)
            {
                forecast.TemperatureF = 32 + (int)(forecast.TemperatureC / 0.5556);
                processed.Add(forecast);
            }

            return processed;
        }
    }
}
