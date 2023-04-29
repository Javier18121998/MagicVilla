﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using AutoMapper;
using MagicVilla_Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_Web.Services.IServices;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTOs;
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
        //Get MEthood cause dont have any reference MEthood this methood calls the view of Villa
        public async Task<IActionResult> CrearVilla()
        {
            return View();
        }
        //This methood in reference of his pair methood is posting the data information
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVilla(VillaCreateDto modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.Crear<APIResponse>(modelo);
                if (response != null && response.IsExitoso)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(modelo);
        }
        
        public async Task<IActionResult> ActualizarVilla(int villaId)
        {
            var response = await _villaService.Obtener<APIResponse>(villaId);
            if (response != null && response.IsExitoso)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Resultado));
                return View(_mapper.Map<VillaUpdateDto>(model));
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarVilla(VillaUpdateDto modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.Actualizar<APIResponse>(modelo);
                if (response != null && response.IsExitoso)
                {
                    TempData["exitoso"] = "Villa Actualizada Exitosamente";
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(modelo);
        }
        public async Task<IActionResult> RemoverVilla(int villaId)
        {
            var response = await _villaService.Obtener<APIResponse>(villaId);
            if (response != null && response.IsExitoso)
            {
                VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Resultado));
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverVilla(VillaUpdateDto modelo)
        {
            
            var response = await _villaService.Remover<APIResponse>(modelo.Id);
            if (response != null && response.IsExitoso)
            {
                TempData["exitoso"] = "Villa Removida Exitosamente";
                return RedirectToAction(nameof(IndexVilla));
            }
            TempData["error"] = "Ocurro un Error al Remover";
            return View(modelo);
        }
    }
}
