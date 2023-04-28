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
    public class NumeroVillaController : ControllerBase
    {
        private readonly ILogger<NumeroVillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numeroRepo;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public NumeroVillaController(ILogger<NumeroVillaController> logger, 
                                     INumeroVillaRepositorio numeroRepo, 
                                     IVillaRepositorio villaRepo, 
                                     IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _numeroRepo = numeroRepo;
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
                _logger.LogInformation("Obtener Numeros villas");
                IEnumerable<NumeroVilla> numeroVillaList = await _numeroRepo.ObtenerTodos(incluirPropiedades:"Villa");
                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numeroVillaList);
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
        [HttpGet("{id:int}", Name="GetNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
        {
            try
            {
                if(id==0)
                {
                    _logger.LogError("Error al traer Numero Villa con Id"+ id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
                }
                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id, incluirPropiedades:"Villa");
                if(numeroVilla==null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false; 
                    return NotFound(_response);
                }
                _response.Resultado = _mapper.Map<NumeroVillaDto>(numeroVilla);
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
        public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto createDTO)
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
                if(await _numeroRepo.Obtener(v=>v.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("NombreExiste","El numero de Villa ya existe!");
                    return BadRequest(ModelState);
                }
                if( await _villaRepo.Obtener(v => v.Id == createDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ClaveForanea","El Id de la Villa no existe!");
                    return BadRequest(ModelState);
                }
                if(createDTO == null)
                {
                    return BadRequest(createDTO);
                }
                // villaDTO.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id +1;
                // VillaStore.villaList.Add(villaDTO);
                // New Model Base Villa
                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDTO);
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
                await _numeroRepo.Crear(modelo);
                // await _db.SaveChangesAsync();
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo}, _response);
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
        public async Task<IActionResult> DeleteNumeroVilla(int id)
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
                var numeroVilla = await _numeroRepo.Obtener(v => v.VillaNo == id);
                if(numeroVilla==null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                // VillaStore.villaList.Remove(villa);
                // _db.Villas.Remove(villa);
                await _numeroRepo.Remover(numeroVilla);
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
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDTO)
        {
            if (updateDTO == null || id != updateDTO.VillaNo)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            if(await _villaRepo.Obtener(v => v.Id == updateDTO.VillaId) == null)
            {
                ModelState.AddModelError("ClaveForanea", "El Id de la Villa no Existe!");
                return BadRequest(ModelState);
            }
            // var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            // villa.Nombre =villaDTO.Nombre;
            // villa.Ocupantes = villaDTO.Ocupantes;
            // villa.MetrosCuadrados = villaDTO.MetrosCuadrados;
            NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDTO);
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
            await _numeroRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}