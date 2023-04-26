using AutoMapper;
using MagicVilla_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_Web.Services.IServices;
using MagicVilla_Web.Models;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> villaList = new ();
            var response = await _villaService.ObtenerTodos<APIResponse>();
            if(response != null && response.IsExitoso){
                villaList = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Resultado));
            }
            return View(villaList);
        }
    }
}
