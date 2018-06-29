using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syzoj.Api.Filters
{
    public class RecaptchaValidationAttribute : ActionFilterAttribute
    {
        private readonly HttpClient httpClient;

        public RecaptchaValidationAttribute()
        {
            // TODO: 应该使用依赖注入吗？还是每个请求构造新的 httpClient？
            this.httpClient = new HttpClient();
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            const string recapchaValidationEndpoint = "https://www.google.com/recaptcha/api/siteverify";
            var response = context.HttpContext.Request.Form["g-recaptcha-response"];
            var request = new HttpRequestMessage(HttpMethod.Post, recapchaValidationEndpoint);
            var paramaters = new Dictionary<string, string>();
            paramaters["secret"] = "SECRETKEY";
            paramaters["response"] = response;
            // TODO: 怎样获取 remote IP ？
            // paramaters["remoteip"] = remoteIp;
            request.Content = new FormUrlEncodedContent(paramaters);

            var resp = await httpClient.SendAsync(request);
            resp.EnsureSuccessStatusCode();

            var responseStream = await resp.Content.ReadAsStreamAsync();
            var serializer = new DataContractJsonSerializer(typeof(RecaptchaResponse));
            var responseJson = (RecaptchaResponse)serializer.ReadObject(responseStream);

            if (!responseJson.Success)
            {
                // TODO: 替换为真正的返回值
                context.Result = new JsonResult(new { status = 1 });
                return;
            }

            await next();
        }
    }

    [DataContract]
    class RecaptchaResponse
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "challenge_ts")]
        public DateTime ChallengeTimeStamp { get; set; }

        [DataMember(Name = "hostname")]
        public string Hostname { get; set; }

        [DataMember(Name = "error-codes")]
        public List<string> ErrorCodes { get; set; }

    }
}