using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        //Metodo get del Upsert
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca"),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto")
            };
            if(id == null)
            {
                //crear nuevo producto
                return View(productoVM);
            }
            else
            {
                //actualizar al producto
                productoVM.Producto = await _unidadTrabajo.Producto.obtener(id.GetValueOrDefault());
                if(productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }


        #region API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Upsert(ProductoVM productoVM)
        {
            if(ModelState.IsValid)
            {
                // Definimos la variable para obtener los archivos desde el formulario
                // En este caso es solo la imagen
                var files = HttpContext.Request.Form.Files;
                // Definir una variable para construir la ruta del directorio WWWRoot
                string webRootPath = _webHostEnvironment.WebRootPath;
                if(productoVM.Producto.Id == 0)
                {
                    //Crear un nuevo producto
                    //Definimo la ruta completa de donde se guardara la imagen
                    string upload = webRootPath + DS.ImagenRuta;
                    //Creamos un id unico de la imagen
                    string fileName = Guid.NewGuid().ToString();
                    //Crear la variable con el tipo de archivo de imagen (Extencion)
                    string extension = Path.GetExtension(files[0].FileName);
                    //Habilitemos el manejo del streaming de los archivos
                    using (var fileStream = new FileStream(
                        Path.Combine(upload,fileName + extension),
                        FileMode.Create
                        ))
                    {
                        //Copiamos el archivo de la memoria del navegador a la 
                        //carpeta del servidor
                        files[0].CopyTo(fileStream);
                    }
                    // Asignamos el nombre de la imagen del producto hacia su registro
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    // Preparamos al modelo para ser guardado
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    // Actualizar el producto existente
                    // Hacemos la consulta del registro a modificar
                    var objProducto = await _unidadTrabajo.Producto
                        .ObtenerPrimero(p => p.Id == productoVM.Producto.Id
                        , isTracking:false);
                    // Saber si el usuario eligio otra imagen
                    if(files.Count > 0)
                    {
                        //Definimo la ruta completa de donde se guardara la imagen
                        string upload = webRootPath + DS.ImagenRuta;
                        //Creamos un id unico de la imagen
                        string fileName = Guid.NewGuid().ToString();
                        //Crear la variable con el tipo de archivo de imagen (Extencion)
                        string extension = Path.GetExtension(files[0].FileName);
                        //Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        //Verificamos si la imagen existe
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(
                            Path.Combine(upload, fileName + extension),
                            FileMode.Create
                            ))
                            {
                                //Copiamos el archivo de la memoria del navegador a la 
                                //carpeta del servidor
                                files[0].CopyTo(fileStream);
                            }
                        //Asignamos la nueva imagen al registro del producto
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }
                    // Si no se escogio otra imagen
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                TempData[DS.Exitosa] = "Producto Registrado con Exito";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            // Si el modelState es Invalido
            TempData[DS.Error] = "Algo anda Mal";
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto");
            return View(productoVM);
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades:"Categoria,Marca");

            return Json(new {data = todos});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDb = await _unidadTrabajo.Producto.obtener(id);
            if (productoDb == null)
            {
                return Json(new { success = false, message = "Error al Borrar Producto" });
            }
            //Eliminamos la imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorfile = Path.Combine(upload, productoDb.ImagenUrl);
            if (System.IO.File.Exists(anteriorfile))
            {
                System.IO.File.Delete(anteriorfile);
            }
            _unidadTrabajo.Producto.Remover(productoDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto Eliminada con Exito" });
        }


        [ActionName("ValidarSerie")]
        public async Task<IActionResult>ValidarSerie(string serie,int id = 0)
        {
            bool valor=false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();
            if(id == 0)
            {
                valor = lista.Any(b=>b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b=>b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim() && b.Id !=id);
            }

            if (valor)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }



        #endregion
    }
}
