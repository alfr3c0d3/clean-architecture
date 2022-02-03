namespace CleanArchitecture.Application.Common.Dtos
{
    public class VehicleMasterDataDto
    {
        public string Vin { get; set; }
        public string VehicleNumber { get; set; }
        public string Make { get; set; }
        public int? ModelNumber { get; set; }
        public int? Year { get; set; }
        public int? CustomerHqNumber { get; set; }
    }
}