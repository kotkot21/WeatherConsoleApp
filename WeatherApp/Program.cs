using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace program
{
    class program
    {
        public static async Task Main()
        {
            List<CityWeatherDetails> weatherList = await GetWeather();
            foreach (var WeatherDetail in weatherList)
            {
                Console.WriteLine($"City: {WeatherDetail.CityName}\nTemperature: {WeatherDetail.Temperature}\nHumidty: {WeatherDetail.Humidty}\nWind Speed: {WeatherDetail.WindSpeed} KM/H\n\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" );
            }
        }
        private static async Task<List<CityWeatherDetails>> GetWeather()
        {
            string ApiKey = "4474a4f7eff75726c6779985b2527e3d";
            List<LatitudeAndLongitude> getLatitudeandLongitude = await GetCoordinates();
            List<CityWeatherDetails> weatherList = new List<CityWeatherDetails>();
            CityWeatherDetails cityWeatherDetails;
            foreach (LatitudeAndLongitude x in getLatitudeandLongitude)
            {
                try
                {
                    string WeatherDataURL =
                        $"http://api.openweathermap.org/data/2.5/forecast?lat={x.Latitude}&lon={x.Longitude}&appid={ApiKey}";
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(WeatherDataURL);
                    var json = await httpResponseMessage.Content.ReadAsStringAsync();
                    JObject WeatherDataJObject = JObject.Parse(json);
                    JArray WeatherDataJArray = new JArray();
                    WeatherDataJArray.Add(WeatherDataJObject);
                    cityWeatherDetails = new CityWeatherDetails((string)WeatherDataJArray[0]["city"]["name"],(int) WeatherDataJArray[0]["list"][0]["main"]["temp"] - 273,
                        (int) WeatherDataJArray[0]["list"][0]["main"]["humidity"],(int) WeatherDataJArray[0]["list"][0]["wind"]["speed"]);
                    weatherList.Add(cityWeatherDetails);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.StatusCode);
                }
            }
            return weatherList;
        }
        private static async Task<List<LatitudeAndLongitude>> GetCoordinates()
            {
                List<string> CityName = ReadInputFromUser();
                List<LatitudeAndLongitude> lat = new List<LatitudeAndLongitude>();
                LatitudeAndLongitude coordinates;
                foreach (string city in CityName)
                {
                    
                    string ApiKey = "4474a4f7eff75726c6779985b2527e3d";
                    string GeoCodeUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&appid={ApiKey}";
                    try
                    {
                        HttpClient httpClient = new HttpClient();
                        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(GeoCodeUrl);

                        var json = await httpResponseMessage.Content.ReadAsStringAsync();
                        JArray GeocodeArray = JArray.Parse(json);
                        coordinates = new LatitudeAndLongitude((string)GeocodeArray[0]["lat"],
                            (string)GeocodeArray[0]["lon"]);
                        lat.Add(coordinates);
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine(ex.HttpRequestError);
                    }
                    catch (ArgumentOutOfRangeException x)
                    {
                        Console.WriteLine("This City Does not exist");
                    }
                   
                }

                return lat;
            }

            private static List<string> ReadInputFromUser()
            {
                Console.WriteLine("Enter the city name OR Enter Done to get the Weather for the cities you entered: ");
                string input = Console.ReadLine();
                List<string> CityName = new List<string>();
                while (true)
                {
                    if (input.Equals("done", StringComparison.OrdinalIgnoreCase))
                    {
                        return CityName;
                    }
                    else
                    {
                        CityName.Add(input);
                        Console.WriteLine("Enter the city name OR Enter Done to get the Weather for the cities you entered: ");
                        input = Console.ReadLine();
                    }
                }
            }
        }
    class LatitudeAndLongitude
        {
            public string Latitude { get; set; }
            public string Longitude { get; set; }

            public LatitudeAndLongitude(string lat, string lon)
            {
                Latitude = lat;
                Longitude = lon;
            }
        }
    class CityWeatherDetails
    {
        public string CityName { get; set; }
        public int Temperature { get; set; }
        public int Humidty { get; set; }
        public int WindSpeed { get; set; }

        public CityWeatherDetails(string name, int temperature, int humidty, int windSpeed)
        {
            CityName = name;
            Temperature = temperature;
            Humidty = humidty;
            WindSpeed = windSpeed;
        }
    }
}

    
