﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;

namespace InternetData
{
    public class Weather
    {
        protected static readonly string key = "4814ff966292830a43761b77aefe1545";
        protected static readonly double longitude = -71.1071909;
        protected static readonly double latitude = 42.340993;

        /// <summary>
        /// Gets weather forcast for the given longitude and latitude.
        /// Default values are replaced by Weather.longitude and Weather.latitude.
        /// </summary>
        /// <param name="longitude">default: Weather.longitude</param>
        /// <param name="latitude">default: Weather.latitude</param>
        /// <returns></returns>
        public static WeatherForcast GetWeatherForcast(double longitude = 500, double latitude = 500)
        {
            // if the default values for latitude and longitude are left (which are not
            // valid values!!!)  Take the values declared statically in this class.
            if (longitude == 500) longitude = Weather.longitude;
            if (latitude == 500) latitude = Weather.latitude;


            // HttpClient is required for sending an API request to the internet.
            HttpClient client = new HttpClient();

            // Initialize an HttpRequestMessage, it will GET data from the api.
            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Get,
                string.Format("https://api.darksky.net/forecast/{0}/{1},{2}", key, latitude, longitude));
            //  The URL formatted in the line above is formatted according to the darksky.net api documentation.
            //  The "key" is generated by the account I created at DarkSky.


            // Send the request and wait for a response.
            // This might take some time~ (probably on the order of 100s of milliseconds...)
            HttpResponseMessage response = client.SendAsync(request).Result;

            // Prepare the DataContract serializer to read the WeatherForecast object from the response.
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WeatherForcast));

            // If the response is not successful, return an empty forecast...
            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Not Successful:  {0}", response.StatusCode);
                return new WeatherForcast();
            }

            // Read the object from the response content and convert it into an
            // instance of WeatherForecast and return it to who asked for it.
            return (WeatherForcast)serializer.ReadObject(response.Content.ReadAsStreamAsync().Result);
        }


    }

    /// <summary>
    /// The WeatherForcast class is modelled after the root object returned by
    /// the DarkSky api.
    ///
    /// the [DataContract] decoration indicates that this will be a class that we
    /// want to convert to and from JSON.  It is necessary for us to use the
    /// DataContractJsonSerializer in the GetWeatherForecast() method.
    /// </summary>
    [DataContract]
    public class WeatherForcast
    {
        [DataMember]
        public double latitude;

        [DataMember]
        public double longitude;

        [DataMember]
        public string timezone;

        /// <summary>
        /// This ForecastData does not necessarily contain all possible properties
        /// some properties may be empty or "null" but that's ok~
        /// </summary>
        [DataMember]
        public ForecastData currently;

        [DataMember]
        public ForecastCollection minutely;

        [DataMember]
        public ForecastCollection hourly;

        [DataMember]
        public ForecastCollection daily;

        [DataMember]
        public List<WeatherAlert> alerts;


        /// <summary>
        /// Quick override for the ToString() method.  Prints the current forecast
        /// date and time, summary, and daily high/low temperatures.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1} - {2}\nHigh:  {3}F ({4})\nLow:  {5}F ({6})",
                currently.Time.ToLongDateString(), currently.Time.ToShortTimeString(), currently.summary,
                daily.data[0].temperatureHigh, daily.data[0].HighTemperatureTime.ToShortTimeString(),
                daily.data[0].temperatureLow, daily.data[0].LowTemperatureTime.ToShortTimeString());
        }
    }

    /// <summary>
    /// Alerts returned by the weather api have this form.
    /// </summary>
    [DataContract]
    public class WeatherAlert
    {
        [DataMember]
        public string title;

        [DataMember]
        public List<string> regions;

        [DataMember]
        public string severity;

        [DataMember]
        public long time;

        [DataMember]
        public long expires;

        [DataMember]
        public string description;

        [DataMember]
        public string uri;


        /// <summary>
        /// This converts the Epcoh Time returned by the API into a DateTime object
        /// in US/Eastern (GMT-5) timezone.
        ///
        /// These are decorated with [IgnoreDataMember] because they are /not/ part of the API.
        /// </summary>
        [IgnoreDataMember]
        public DateTime Time => DateTimeOffset.FromUnixTimeSeconds(time).UtcDateTime.AddHours(-5);

        [IgnoreDataMember]
        public DateTime Expires => DateTimeOffset.FromUnixTimeSeconds(expires).UtcDateTime.AddHours(-5);
    }

    /// <summary>
    /// a ForcastCollection is used in the API as the minutely, hourly and daily
    /// fields consist of a collection of ForcastData objects with a general summary.
    /// </summary>
    [DataContract]
    public class ForecastCollection
    {
        [DataMember]
        public string summary;

        [DataMember]
        public string icon;

        [DataMember]
        public List<ForecastData> data;
    }

    /// <summary>
    /// This is an extraordinary object which may contain many or only a few of the
    /// listed properties.  Note, that "time" is listed as Required because all
    /// ForecastData objects will contain a timestamp.
    /// </summary>
    [DataContract]
    public class ForecastData
    {
        [DataMember(IsRequired = true)]
        public long time;

        [IgnoreDataMember]
        public DateTime Time => DateTimeOffset.FromUnixTimeSeconds(time).UtcDateTime.AddHours(-5);

        [DataMember]
        public string summary;

        [DataMember]
        public string icon;

        [DataMember]
        public double percipIntensity;

        [DataMember]
        public double pecipProbability;

        [DataMember]
        public double temperatue;

        [DataMember]
        public double apparentTemperature;

        [DataMember]
        public double dewPoint;

        [DataMember]
        public double humidity;

        [DataMember]
        public double pressure;

        [DataMember]
        public double windSpeed;

        [DataMember]
        public double windGust;

        [DataMember]
        public double windBearing;

        [DataMember]
        public double cloudCover;

        [DataMember]
        public double visibility;

        [DataMember]
        public double ozone;

        [DataMember]
        public double moonPhase;

        [DataMember]
        public double temperatureHigh;

        [DataMember]
        public long temperatureHighTime;

        [IgnoreDataMember]
        public DateTime HighTemperatureTime => DateTimeOffset.FromUnixTimeSeconds(temperatureHighTime).UtcDateTime.AddHours(-5);

        [DataMember]
        public double temperatureLow;

        [DataMember]
        public long temperatureLowTime;

        [IgnoreDataMember]
        public DateTime LowTemperatureTime => DateTimeOffset.FromUnixTimeSeconds(temperatureLowTime).UtcDateTime.AddHours(-5);

        // There are more things we could add.......
    }
        
}
