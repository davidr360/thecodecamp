using Microsoft.Web.Http;
using Microsoft.Web.Http.Versioning;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TheCodeCamp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            // HACK: Microsoft.AspNet.WebApi.Versioning on Nuget, will add versioning for your API

            config.AddApiVersioning(cfg =>
            {
                cfg.DefaultApiVersion = new ApiVersion(1, 0);
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ReportApiVersions = true;

                // HACK: If you don't set this, the default version reader is QueryStringApiVersionReader
                cfg.ApiVersionReader = new HeaderApiVersionReader("X-Version");

                // HACK: You can specify multiple version methods as per below
                //cfg.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader(), new HeaderApiVersionReader("X-Version"));
            });

            // HACK: Have to manually add this contract resolver to sort out the camel casing
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();       

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
