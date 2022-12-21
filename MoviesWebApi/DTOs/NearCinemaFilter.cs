using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class NearCinemaFilter
    {
        [Range(-90, 90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }
        public double distanceInKMs = 10;
        public double maximunDistanceInKMs = 50;
        public double DistanceInKMs { 
            get=>distanceInKMs; 
            set{
                distanceInKMs = (value > maximunDistanceInKMs) ? maximunDistanceInKMs : value;
            }
        }
    }
}
