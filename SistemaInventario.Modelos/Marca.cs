using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Marca
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="El Campo Nombre es Requerido")]
        [MaxLength(60,ErrorMessage ="El Campo Nombre no es mas de 60 Caracteres")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El Campo Descripcion es Requerido")]
        [MaxLength(100, ErrorMessage = "El Campo Descripcion no es mas de 100 Caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El Campo Estado es Requerido")]
        public bool Estado { get; set; }
    }
}
