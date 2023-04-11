using System;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Models.DTOs
{
    public class VillaDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }
        public int Ocupantes { get; set; }
        public int MetrosCuadrados { get; set; }
    }
}