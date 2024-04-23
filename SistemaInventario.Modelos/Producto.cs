using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El Número de Serie es Requerido")]
        [MaxLength(60, ErrorMessage = "Máximo 60 Caracteres")]
        public string NumeroSerie { get; set; }
        [Required(ErrorMessage = "La Descripcion es Requerido")]
        [MaxLength(100, ErrorMessage = "Máximo 100 Caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El Precio es Requerido")]
        public double Precio { get; set; }
        [Required(ErrorMessage = "El Costo es Requerido")]
        public double Costo { get; set; }
        public string ImagenUrl { get; set; }
        [Required(ErrorMessage = "El Estado es Requerido")]
        public bool Estado { get; set; }
        //Realizaremos la relacion con la tabla categoria y con la marca
        [Required(ErrorMessage = "La Categoria es Requerida")]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }
        [Required(ErrorMessage = "La Marca es Requerida")]
        public int MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }
        //Recursividad al Padre
        public int? PadreId { get; set; }
        public virtual Producto Padre { get; set; }
    }
}
