using LoreVault.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoreVault.Domain.Interfaces
{
    public interface IWeatherForecastRepository
    {
        WeatherForecast[] GetForecasts();
    }
}
