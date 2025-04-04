namespace JurMaps.Model
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public List<MapCountry>? MapCountries { get; set; }



        public Country(string countryName) 
        {
            CountryName = countryName;
        }    
    }
}
