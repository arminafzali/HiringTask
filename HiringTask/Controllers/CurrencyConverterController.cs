using HiringTask.Dtos;
using HiringTask.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HiringTask.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _currencyConverter;

        public CurrencyConverterController(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

        [HttpGet("Clear")]
        public void Clear()
        {
            _currencyConverter.ClearConfiguration();
        }
        [HttpPost("UpdateConfiguration")]
        public void UpdateConfiguration(UpdateConfigurationDto dto)
        {
            _currencyConverter.UpdateConfiguration(dto.ConversionRates);
        }
        [HttpGet("Convert")]
        public double Convert(string fromCurrency,string toCurrency,double amount)
        {
            return _currencyConverter.Convert(fromCurrency,toCurrency,amount);
        }
    }
}
