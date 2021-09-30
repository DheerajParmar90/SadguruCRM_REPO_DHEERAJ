using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SadguruCRM.Helpers
{
    public class ModelsMapper
    {


        public IMapper ModelsoMapper()
        {

            var config = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<Source, Dest>();
            });

            IMapper mapper = config.CreateMapper();
            return mapper;
        }  
    }
}