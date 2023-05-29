var MyTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("pending")) {
        OrderTable("pending")
    }
    else if (url.includes("approved")) {       
            OrderTable("approved")       

    }
    else {
            OrderTable("allorders") 
         }

   
})
function OrderTable(status) {


    MyTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/AllOrders?status="+status

        },
        "columns": [
            { "data": "name" },
            { "data": "phoneNumber" },
            { "data": "orderStatus" },
            { "data": "paymentStatus" },
            { "data": "orderTotal" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                        <a href="/Admin/order/OrderDetail/${data}" class="m-2" ><i class="bi bi-pencil-square "></i></a>
                        
                        `;
                }
            }

        ]
    })
}
