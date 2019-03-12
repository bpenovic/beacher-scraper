namespace ScraperLib
{
    public class Statics
    {
        public static readonly string Type = "Type of beach";
        public static readonly string SurfaceType = "Prevalence beach surface type";
        public static readonly string Vegetation = "Vegetation";
        public static readonly string AverageTemperature = "Average sea temperature";
        public static readonly string MinSalinity = "Salinity - min";
        public static readonly string MaxSalinity = "Salinity - max";
        public static readonly string Wind = "Prevailing wind";
        public static readonly string Length = "Beach length";
        public static readonly string Width = "Beach width";
        public static readonly string Shape = "Beach shape";
        public static readonly int Srid = 4326;
        public static readonly int QualitySyncWeeks = 2;
    }

    public class Parameters
    {
        public static readonly string Season = "psez";
        public static readonly string Year = "p_god";
        public static readonly string Language = "p_jezik";
        public static readonly string Cycle = "p_ciklus";
        public static readonly string View = "p_prikaz";
        public static readonly string CycleView = "p_cprikaz";
        public static readonly string Filter = "p_filter";
    }

    public class Endpoints
    {
        public static readonly string Markers = "http://baltazar.izor.hr/plazepub/kakvoca_prikaz_xml9";
        public static readonly string Details = "http://baltazar.izor.hr/plazepub/profil_plaze";
        public static readonly string Quality = "http://baltazar.izor.hr/plazepub/kakvoca_ispitivanja9";
    }
}