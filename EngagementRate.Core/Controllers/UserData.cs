using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EngagementRate.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserData : ControllerBase
    {
        [HttpGet("UserInfo/{userName}")]
        public  JsonResult UserInfo(string userName)
        {
            string result;
            try
            {

                using (var myWebClient = Factor($"https://i.instagram.com/api/v1/users/web_profile_info/?username={userName}"))
                {
                    var jsonResponse = myWebClient.GetAsync($"https://i.instagram.com/api/v1/users/web_profile_info/?username={userName}").Result;
                    result = jsonResponse.Content.ReadAsStringAsync().Result;
                    //var temp = JsonConvert.DeserializeObject<object>(result);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return  new JsonResult(JsonConvert.DeserializeObject<object>(result));
        }
        
        [HttpGet("PostsInfo/{query_hash}")]
        public JsonResult UserPostsInfo(string query_hash)
        {
            string result;
            try
            {
                var queryString = query_hash.Split("&variables=");
                var param = Uri.EscapeDataString(queryString[1]);
                var url = $"https://www.instagram.com/graphql/query/?query_hash={queryString[0] + "&variables=" + param}";
                using (var myWebClient = Factor(url))
                {
                    var jsonResponse = myWebClient.GetAsync(url).Result;
                    result = jsonResponse.Content.ReadAsStringAsync().Result;
                    //var temp = JsonConvert.DeserializeObject<object>(result);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new JsonResult(JsonConvert.DeserializeObject<object>(result));
        }

        public class AvatarViewModel
        {
            public string UserName { get; set; }
            public string UrlImage { get; set; }
        }
        [HttpPost("GetAvatar")]
        public JsonResult GetAvatar(AvatarViewModel param)
        {
            string result;
            Image img;
            try
            {
                var url = Uri.UnescapeDataString(param.UrlImage);
                using (var myWebClient = Factor(url))
                {
                    var jsonResponse = myWebClient.GetAsync(url).Result;
                    var stream = jsonResponse.Content.ReadAsStream();

                    img = Image.FromStream(stream);

                    img.Save(@"D:\Projects\EngagementRate1\EngagementRate\EngagementRate\src\assets\avatar\" + $"{param.UserName}.jpg", ImageFormat.Jpeg);
                    //var temp = JsonConvert.DeserializeObject<object>(result);
                    //D:\Projects\EngagementRate1\EngagementRate\EngagementRate.Core\Controllers\
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new JsonResult(true);
        }
        private static HttpClient Factor(string url)
        {

            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }) { BaseAddress = new Uri(url) };
            client.Timeout = TimeSpan.FromMinutes(20);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Instagram 9.5.2 (iPhone7,2; iPhone OS 9_3_3; en_US; en-US; scale=2.00; 750x1334) AppleWebKit/420+");
            client.DefaultRequestHeaders.Add("Cookie", "mid=YriLOgALAAH4XbpV9YTzsDyZz1l2; ig_did=035068CF-9DC3-4A8F-AB46-02AC4858DD56; ig_nrcb=1; csrftoken=q4Ir27nihfXPJs9TZOBPKJSPEU0NGXMd; ds_user_id=53691035409; sessionid=53691035409%3A62X2H71RhJYwxS%3A4; datr=5M25Yim57Nqc1lhdRU_bPDbZ; dpr=1.350000023841858; rur=\"RVA\\05453691035409\\0541688136097:01f77b37f0360deffd042fe737afd11388cf4a884c98935fca3f9f8f73f75e7f094d0c0e\"; csrftoken=3LYfBcB0rxQbN4iphZCKvJ0KkP5Fz3uf; ds_user_id=53691035409; ig_did=F790ADF4-7467-45D2-8D4E-F1FF6C2CADD6; ig_nrcb=1; mid=YrrgFAAEAAGfuLandoPurb0TVpF4; rur=\"RVA\\05453691035409\\0541688136254:01f7cb0240f415b9d04eeb6fe547abd532561f28bc2f3786d5c5c49657870cbd09505ad2\"; csrftoken=3LYfBcB0rxQbN4iphZCKvJ0KkP5Fz3uf; ds_user_id=53691035409; rur=\"RVA\\05453691035409\\0541688136392:01f7d8800266c4d35da28e5a589d1e64f81ea0bae4eef5cfd1be15093806ee1e46db5e83\"");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("deflate"));
            return client;
        }
        private static HttpClient Factor2(string url)
        {

            var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }) { BaseAddress = new Uri(url) };
            client.Timeout = TimeSpan.FromMinutes(20);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Mobile Safari/537.36");
            client.DefaultRequestHeaders.Add("Cookie", "'mid = YraaEgALAAG7tJh9FlCnbvPCyDem; ig_did = 3886E022 - E8D0 - 4627 - 986C - A4E667B2F42B; ig_nrcb = 1; csrftoken = 21CYexjL8O5herfyFfNoItjE4hiD2FNJ; ds_user_id = 53691035409; sessionid = 53691035409 % 3AmvnpHZJjJaM9wd % 3A25; datr = GZu2YmVTKoFGJ1Rr0NhUElHV; rur =\"RVA\\05453691035409\\0541687680653:01f75913381f1c0e02d96094b4a5b68e94eb4705490d3fcdf3c210312a0ad2ea841fe37d\"; csrftoken=21CYexjL8O5herfyFfNoItjE4hiD2FNJ; csrftoken=21CYexjL8O5herfyFfNoItjE4hiD2FNJ; ds_user_id=53691035409; rur=\"ODN\\05453691035409\\0541687759152:01f710bf03a896c27f4928830fb0525ad439b2347e077cb2819fb24a891d77d32e8278f9\"; rur=\"RVA\\05453691035409\\0541687759689:01f76faebb284a69f08c3ef8449bd872f85842aba937dc1cbc261c2f53b02e3413a7c8b3\'");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("deflate"));
            return client;
        }
    }
}
