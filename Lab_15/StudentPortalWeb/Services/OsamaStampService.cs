namespace StudentPortalWeb.Services
{
    public class OsamaStampService : IOsamaStampService
    {
        public string Owner => "Osama Aboud";
        public string Stamp { get; }

        public OsamaStampService()
        {
            Stamp = Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}