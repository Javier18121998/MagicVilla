using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_API.Models.DTOs;

namespace MagicVilla_API.Datos
{
    public class VillaStore
    {
       public static List<VillaDTO> villaList = new List<VillaDTO>
       {
            new VillaDTO{Id=1, Nombre="Vista a la Piscina", Ocupantes=3, MetrosCuadrados=50},
            new VillaDTO{Id=2, Nombre="Vista a la Playa", Ocupantes=4, MetrosCuadrados=80}
       };
    }
}