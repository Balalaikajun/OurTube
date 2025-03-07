using AutoMapper;
using OurTube.Application.DTOs.Views;
using OurTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Mapping
{
    public class ViewProfile:Profile
    {
        public ViewProfile()
        {
            CreateMap<View, ViewGetDTO>();
        }

    }
}
