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
    public class CitiesController : ApiController
    {
        private SadguruCRMEntities db = new SadguruCRMEntities();
        // GET api/<controller>
        public IEnumerable<CityModel> Get()
        {
            //return new string[] { "value1", "value2" };
            //AutoMapper.CreateMap<LearningMVC.User, LearningMVC.Models.User>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<City, CityModel>();
            });

            IMapper mapper = config.CreateMapper();

            List<CityModel> cities = new List<CityModel>();
            
            //var locationsDATA = ;
            foreach (var city in db.Cities)
            {
                CityModel c = mapper.Map<CityModel>(city);
                //AutoMapper.Mapper.Map<Location, LocationModel>(location, loc);
                //LocationModel loc = new LocationModel();
                //loc.LocationID = location.LocationID;
                //loc.LocationName = location.LocationName;

                cities.Add(c);
            }
            return cities;
        }

        // GET api/<controller>/5
        public CityModel Get(int id)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<City, CityModel>();
            });

            IMapper mapper = config.CreateMapper();
            CityModel c = mapper.Map<CityModel>(db.Cities.Find(id));
            return c;
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