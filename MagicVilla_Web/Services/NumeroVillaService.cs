using System.Xml;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicVilla_Web.Services.IServices;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTOs;
using MagicVilla_Utilidad;

namespace MagicVilla_Web.Services
{
    public class NumeroVillaService : BaseService, INumeroVillaService
    {
        public readonly IHttpClientFactory _httpClient;
        private string _villaUrl;
        public NumeroVillaService(IHttpClientFactory httpClient, IConfiguration configuration) :base(httpClient)
        {
            _httpClient = httpClient;
            _villaUrl = configuration.GetValue<string>("ServicesUrls:API_URL");
        }
        public Task<T> Actualizar<T>(NumeroVillaUpdateDto dto)
        {
            return SendAsync<T>(new APIRequest(){
                APITipo = DS.APITipo.PUT,
                Datos = dto,
                Url = _villaUrl+"/api/NumeroVilla/"+dto.VillaNo
            });
        }
        public Task<T> Crear<T>(NumeroVillaCreateDto dto)
        {
            return SendAsync<T>(new APIRequest(){
                APITipo = DS.APITipo.POST,
                Datos = dto,
                Url = _villaUrl+"/api/NumeroVilla"
            });
        }
        public Task<T> Obtener<T>(int id)
        {
            return SendAsync<T>(new APIRequest(){
                APITipo = DS.APITipo.GET,
                Url = _villaUrl+"/api/NumeroVilla/"+id
            });
        }
        public Task<T> ObtenerTodos<T>()
        {
            return SendAsync<T>(new APIRequest(){
                APITipo = DS.APITipo.GET,
                Url = _villaUrl+"/api/NumeroVilla"
            });        
        }
        public Task<T> Remover<T>(int id)
        {
            return SendAsync<T>(new APIRequest(){
                APITipo = DS.APITipo.DELETE,
                Url = _villaUrl+"/api/NumeroVilla/" + id
            }); 
        }
    }
}