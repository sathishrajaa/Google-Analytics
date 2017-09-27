using System;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Google.Apis.Analytics.v3.Data;
using System.Reflection;

namespace GoogleAnalyticsApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Default Password
            const string password = "notasecret";

            // found https://console.developers.google.com
            //Replace the below Service Account Email with your Service Account Email
            const string serviceAccountEmail = "something@analyticsproject-123456.iam.gserviceaccount.com";
            
            // Downloaded from https://console.developers.google.com
            var keyFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\AnalyticsProject-45fa102e87ca.p12";

            // Scopes for view and manage your Google Analytics data
            string[] scopes = new string[] { AnalyticsService.Scope.Analytics };

            //loading the Key file
            var certificate = new X509Certificate2(keyFilePath, password, X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes
            }.FromCertificate(certificate));

            try
            {
                var service = GetService(credential);
                
                var totalAppUsers = GetTotalAppUsers(service);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to access the Google Analytics Service" + ex);
            }
        }

        private static GaData GetTotalAppUsers(AnalyticsService service)
        {
            //Replace ga:123456789 with your Google Analytics application unique ID
            DataResource.GaResource.GetRequest request = service.Data.Ga.Get("ga:123456789", "2017-05-01", "2017-07-05", "ga:screenViews");
            request.Dimensions = "ga:dimension1";
            request.Filters = "ga:screenName==Calendar";
            GaData result = request.Execute();
            return result;
        }

        private static AnalyticsService GetService(ServiceAccountCredential credential)
        {
            var service = new AnalyticsService();
            try
            {
                service = new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Analytics API Sample",
                });
                return service;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create a new Analytics Service" + e);
            }
        }
    }
}
