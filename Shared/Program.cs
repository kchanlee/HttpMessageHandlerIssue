using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            try
            {
                var cert = FindCertificateByThumbprint("1A7201E21CA93CAFED9FDCD1736716F606A22651");

                using (var handler = HttpMessageHandlerFactory.Create(cert))
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://release.moscnuat.com");
                    client.DefaultRequestHeaders.Host = "release.moscnuat.com";

                    var postString = "{\"phone\":\"13599999999\",\"names\":[{\"name\":\"test\"}],\"pageSize\":100,\"pageNum\":0}";

                    using (var postContent = new StringContent(postString, Encoding.UTF8, "application/json"))
                    using (var response = await client.PostAsync("/searchorder_wechat", postContent))
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        Console.WriteLine(responseContent);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            try
            {
                store.Open(OpenFlags.ReadOnly);
                var col = store.Certificates.Find(X509FindType.FindByThumbprint, findValue, false);
                return col[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
