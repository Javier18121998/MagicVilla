using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModel
{
    public class NumeroVillaViewModel
    {
        public NumeroVillaViewModel()
        {
            NumeroVilla = new NumeroVillaCreateDto();
        }

        public NumeroVillaCreateDto NumeroVilla { get; set; }
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}