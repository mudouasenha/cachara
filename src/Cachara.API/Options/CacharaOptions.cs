namespace Cachara.API.Options
{
    public class CacharaOptions
    {
        public string Name { get; set; }
        
        public string SqlDb { get; set; }
        public string JobsSqlDb { get; set; }
        
        public CacharaExporterOptions CacharaExporter { get; set; }
    }

    public class CacharaExporterOptions
    {
        public string Url { get; set; }
        
        public string Token { get; set; }
    }
}
