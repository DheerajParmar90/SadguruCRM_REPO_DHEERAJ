using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using SadguruCRM.Helpers;
using AutoMapper;

[assembly: OwinStartup(typeof(SadguruCRM.Startup))]

namespace SadguruCRM
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.MapSignalR();
            //AutoMapper.Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());

            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
