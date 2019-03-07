namespace ScraperLib.DomainModels
{
    public class Details
    {
        public string Type { get; set; }
        public string SurfaceType { get; set; }
        public string Vegetation { get; set; }
        public string Shape { get; set; }
        public double AverageTemperature { get; set; }
        public double MaxSalinity { get; set; }
        public double MinSalinity { get; set; }
        public string Wind { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
    }
}