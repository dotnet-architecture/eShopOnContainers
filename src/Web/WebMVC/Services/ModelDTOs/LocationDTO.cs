namespace WebMVC.Services.ModelDTOs
{
    public record LocationDTO
    {
        public double Longitude { get; init; }
        public double Latitude { get; init; }
    }
}
