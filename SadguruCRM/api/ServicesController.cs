using AutoMapper;
using SadguruCRM.Models;
using SadguruCRM.ViewModels.SimplifiedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SadguruCRM.api
{
    public class ServicesController : ApiController
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        // GET api/<controller>
        public IEnumerable<ServiceModel> Get()
        {
            //return new string[] { "value1", "value2" };
            //AutoMapper.CreateMap<LearningMVC.User, LearningMVC.Models.User>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Service, ServiceModel>();
            });

            IMapper mapper = config.CreateMapper();

            List<ServiceModel> list = new List<ServiceModel>();
            
            //var locationsDATA = ;
            foreach (var ele in db.Services)
            {
                ServiceModel single = mapper.Map<ServiceModel>(ele);
                //AutoMapper.Mapper.Map<Location, LocationModel>(location, loc);
                //LocationModel loc = new LocationModel();
                //loc.LocationID = location.LocationID;
                //loc.LocationName = location.LocationName;

                list.Add(single);
            }
            return list;
        }

        // GET api/<controller>/5
        public ServiceModel Get(int id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Service, ServiceModel>();
            });

            IMapper mapper = config.CreateMapper();
            ServiceModel single = mapper.Map<ServiceModel>(db.Services.Find(id));
            return single;
            //return "value";
        }

        //// POST api/<controller>
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}