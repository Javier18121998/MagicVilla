using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla_Web.Models.ViewModel
{
    public class NumeroVillaUpdateViewModel
    {
        public NumeroVillaUpdateViewModel()
        {
            NumeroVilla = new NumeroVillaUpdateDto();
        }

        public NumeroVillaUpdateDto NumeroVilla { get; set; }
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}