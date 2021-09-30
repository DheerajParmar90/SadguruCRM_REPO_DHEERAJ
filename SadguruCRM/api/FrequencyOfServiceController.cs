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
    public class FrequencyOfServiceController : ApiController
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        // GET api/<controller>
        public IEnumerable<FrequencyOfServiceModel> Get()
        {
            //return new string[] { "value1", "value2" };
            //AutoMapper.CreateMap<LearningMVC.User, LearningMVC.Models.User>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FrequencyOfService, FrequencyOfServiceModel>();
            });

            IMapper mapper = config.CreateMapper();

            List<FrequencyOfServiceModel> frequencies = new List<FrequencyOfServiceModel>();
            
            //var locationsDATA = ;
            foreach (var freq in db.FrequencyOfServices)
            {
                FrequencyOfServiceModel f = mapper.Map<FrequencyOfServiceModel>(freq);
                //AutoMapper.Mapper.Map<Location, LocationModel>(location, loc);
                //LocationModel loc = new LocationModel();
                //loc.LocationID = location.LocationID;
                //loc.LocationName = location.LocationName;

                frequencies.Add(f);
            }
            return frequencies;
        }

        // GET api/<controller>/5
        public FrequencyOfServiceModel Get(int id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FrequencyOfService, FrequencyOfServiceModel>();
            });

            IMapper mapper = config.CreateMapper();
            FrequencyOfServiceModel f = mapper.Map<FrequencyOfServiceModel>(db.FrequencyOfServices.Find(id));
            return f;
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