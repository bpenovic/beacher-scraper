namespace ScraperLib
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public DataEndpoints DataEndpoints { get; set; }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public class DataEndpoints
    {
        public string Markers { get; set; }
        public string MarkerDetails { get; set; }
        public string MarkerQuality { get; set; }
    }
}
