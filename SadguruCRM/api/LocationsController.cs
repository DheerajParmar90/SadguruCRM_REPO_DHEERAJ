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
    public class LocationsController : ApiController
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        // GET api/<controller>
        public IEnumerable<LocationModel> Get()
        {
            //return new string[] { "value1", "value2" };
            //AutoMapper.CreateMap<LearningMVC.User, LearningMVC.Models.User>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Location, LocationModel>();
            });

            IMapper mapper = config.CreateMapper();

            List<LocationModel> locations = new List<LocationModel>();
            
            //var locationsDATA = ;
            foreach (var location in db.Locations)
            {
                LocationModel loc = mapper.Map<LocationModel>(location);
                //AutoMapper.Mapper.Map<Location, LocationModel>(location, loc);
                //LocationModel loc = new LocationModel();
                //loc.LocationID = location.LocationID;
                //loc.LocationName = location.LocationName;

                locations.Add(loc);
            }
            return locations;
        }

        // GET api/<controller>/5
        public LocationModel Get(int id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Location, LocationModel>();
            });

            IMapper mapper = config.CreateMapper();
            LocationModel loc = mapper.Map<LocationModel>(db.Locations.Find(id));
            return loc;
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