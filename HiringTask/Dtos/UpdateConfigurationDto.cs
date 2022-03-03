namespace HiringTask.Dtos
{
    public class UpdateConfigurationDto
    {
        public IEnumerable<Tuple<string, string, double>> ConversionRates { get; set; }
    }
}
