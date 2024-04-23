let datatable;

$(document).ready(function(){
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        //seccion de ajax para el plugin
        "ajax": { "url": "/Admin/Categoria/ObtenerTodos" },
        "columns": [
            { "data": "nombre", "width": "20%" },
            { "data": "descripcion", "width": "40%" },
            {
                "data": "estado",
                "render": function (data) {
                    if (data == true) {
                        return "Activo";
                    }
                    else {
                        return "Inactivo";
                    }
                }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Categoria/Upsert/${data}" class="btn btn-success text-white" style = "cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick=Delete("/Admin/Categoria/Delete/${data}") class = "btn btn-danger text-white" style = "cursor:pointer">
                                <i class="bi bi-trash3-fill"></i>
                            </a>
                        </div>
                    `;
                }, "width": "20%"
            }
        ], "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json",
            "search": "Buscar:",
        }
    });
}


function Delete(url) {
    swal({
        title: "¿Estas seguro de Eliminar la Bodega?",
        text: "Este registro no sera Recuperado",
        icon: "warning",
        buttons: true,
        dangerMode:true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "Post",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });


}