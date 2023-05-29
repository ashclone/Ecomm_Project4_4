var MyTable;
$(document).ready(function () {
    MyTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/AllProducts"

        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "description", "width": "20%" },
            { "data": "price", "width": "10%" },
            { "data": "cateogory.name", "width": "20%" },

            {
                "data": "imageUrl",
                "render": function (data) {
                    return `
                            <div class="rounded-circle" width="40" height="40">
                        <img src=${data} class="rounded-end" width="70" height="70" />
                    </div>
                            `;
                }

            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                        <a href="/Admin/product/CreateUpdate/${data}" class="m-2" ><i class="bi bi-pencil-square "></i></a>
                        <a onclick=Delete("/Admin/product/Delete/${data}") ><i class="bi bi-trash-fill"></i></a>
                        `;
                }
            }

        ]
    })
})
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: 'Deleted!',
                            text:'data Deleted Successfully ',
                            icon:'success'
                            }
                        )
                        MyTable.ajax.reload();
                    }
                    else {
                        Swal.fire({
                            title:'Cancelled',
                            text: "Error while Deleting data in DataBase",
                            icon:'error'
                            }
                        )
                    }
                }
            })

        }
    })

}