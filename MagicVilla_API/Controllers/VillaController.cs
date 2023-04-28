using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_API.Models.DTOs;
using MagicVilla_API.Datos;
using Microsoft.AspNetCore.JsonPatch;
using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_API.Repositorio.IRepositorio;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaController(ILogger<VillaController> logger, 
                               IVillaRepositorio villaRepo, 
                               IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las villas");
                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();
                _response.Resultado = _mapper.Map<IEnumerable<VillaDTO>>(villaList);
                // return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villaList));
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>(){
                    ex.ToString()
                };
            }
            return _response;
        }
        [HttpGet("{id:int}", Name="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if(id==0)
                {
                    _logger.LogError("Error al traer Villa con Id"+ id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }
                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var villa = await _villaRepo.Obtener(v => v.Id == id);
                if(villa==null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false; 
                    return NotFound(_response);
                }
                _response.Resultado = _mapper.Map<VillaDTO>(villa);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
                // return Ok(_mapper.Map<VillaDTO>(villa));
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() {
                    ex.ToString()
                };
                return _response;            
            }
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // if(VillaStore.villaList.FirstOrDefault(v=>v.Nombre.ToLower() == villaDTO.Nombre.ToLower()) != null)
                // {
                //     ModelState.AddModelError("NombreExiste","La Villa con ese Nombre ya existe!");
                //     return BadRequest(ModelState);
                // }
                if(await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDTO.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste","La Villa con ese Nombre ya existe!");
                    return BadRequest(ModelState);
                }
                if(createDTO == null)
                {
                    return BadRequest(createDTO);
                }
                // villaDTO.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id +1;
                // VillaStore.villaList.Add(villaDTO);
                // New Model Base Villa
                Villa modelo = _mapper.Map<Villa>(createDTO);
                /*Villa modelo = new()
                {
                    Nombre = villaDTO.Nombre,
                    Detalle = villaDTO.Detalle,
                    ImagenUrl = villaDTO.ImagenUrl,
                    Ocupantes = villaDTO.Ocupantes,
                    Tarifa = villaDTO.Tarifa,
                    MetrosCuadrados = villaDTO.MetrosCuadrados,
                    Amenidad = villaDTO.Amenidad
                };*/
                modelo.FechaActualizacion = DateTime.Now;
                modelo.FechaCreacion = DateTime.Now;
                await _villaRepo.Crear(modelo);
                // await _db.SaveChangesAsync();
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = modelo.Id}, _response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>{
                    ex.ToString()
                };
            }
            return _response;
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                if(id==0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                // var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var villa = await _villaRepo.Obtener(v => v.Id == id);
                if(villa==null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                // VillaStore.villaList.Remove(villa);
                // _db.Villas.Remove(villa);
                await _villaRepo.Remover(villa);
                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>(){
                    ex.ToString()
                };
            }
            return BadRequest(_response);
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            // var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            // villa.Nombre =villaDTO.Nombre;
            // villa.Ocupantes = villaDTO.Ocupantes;
            // villa.MetrosCuadrados = villaDTO.MetrosCuadrados;
            Villa modelo = _mapper.Map<Villa>(updateDTO);
            /*Villa modelo = new()
            {
                Id = villaDTO.Id,
                Nombre = villaDTO.Nombre,
                Detalle = villaDTO.Detalle,
                ImagenUrl = villaDTO.ImagenUrl,
                Ocupantes = villaDTO.Ocupantes,
                Tarifa = villaDTO.Tarifa,
                MetrosCuadrados = villaDTO.MetrosCuadrados,
                Amenidad = villaDTO.Amenidad
            };*/
            // _db.Villas.Update(modelo);
            // await _db.SaveChangesAsync();
            await _villaRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDTo)
        {
            if (patchDTo == null || id == 0)
            {
                return BadRequest();
            }
            // var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            // var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            var villa = await _villaRepo.Obtener(v => v.Id == id, tracked:false);
            VillaUpdateDto villaDTO = _mapper.Map<VillaUpdateDto>(villa);
            // VillaUpdateDto villaDTO = new()
            // {
            //     Id = villa.Id,
            //     Nombre = villa.Nombre,
            //     Detalle = villa.Detalle,
            //     ImagenUrl = villa.ImagenUrl,
            //     Ocupantes = villa.Ocupantes,
            //     Tarifa = villa.Tarifa,
            //     MetrosCuadrados = villa.MetrosCuadrados,
            //     Amenidad = villa.Amenidad
            // };
            if(villa == null) return BadRequest();
            patchDTo.ApplyTo(villaDTO, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Villa modelo = _mapper.Map<Villa>(villaDTO);
            // Villa modelo = new()
            // {
            //     Id = villaDTO.Id,
            //     Nombre = villaDTO.Nombre,
            //     Detalle = villaDTO.Detalle,
            //     ImagenUrl = villaDTO.ImagenUrl,
            //     Ocupantes = villaDTO.Ocupantes,
            //     Tarifa = villaDTO.Tarifa,
            //     MetrosCuadrados = villaDTO.MetrosCuadrados,
            //     Amenidad = villaDTO.Amenidad
            // };
            // _db.Villas.Update(modelo);
            // await _db.SaveChangesAsync();
            await _villaRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}