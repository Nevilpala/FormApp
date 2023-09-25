//function DeleteConfirm(e) {
//    //console.log(e);
//    //e.preventDefault();
//    var bool = false;
//    swal({
//        title: "Are you sure?",
//        text: "You Want to Delete !",
//        icon: "warning",
//        buttons: true,
//        dangerMode: true,
//    }).then((willDelete) => {
//            if (willDelete) {
//                swal(
//                    "Successfully deleted!", {
//                    icon: "success"
//                })
//                return true;

//            } else {
//                //swal("Your imaginary file is safe!");
//                return false;

//            }

//        });
  
//    console.log("bool : " + bool);
//}
function ajaxformpartialview(url, data, type) {
    if (type == "search") {
        url = url + "?&filter=true";
        $.ajax({
            url: url, // Replace with your controller action URL
            type: "POST", // or "GET" if you prefer
            data: formData,
            success: function (data) {
                // Update the div with the response from the controller
                $("#partialViewContainer").html(data);
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    else {
        $.ajax({
            url: url, // Replace with your controller action URL
            type: "POST", // or "GET" if you prefer
            data: formData,
            success: function (data) {
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
     
}
//function formSubmit(e) {
//    event.preventDefault();
//    var formData = $(this).serialize();
//    var url = $(this).attr("action");
//    ajaxformpartialview(url, formData);
//}
$(document).ready(function () {
    $(".formPartialViewSubmit").submit(function (event) {
        event.preventDefault(); 
        var formData = $(this).serialize();
        var url = $(this).attr("action");
        ajaxformpartialview(url, formData);

    });
    //$(".formDeleteSubmit").submit(function (event) {
    //    event.preventDefault(); // Prevent the default form submission
    //    console.log($(this).attr('action'));
    //    // Serialize the form data
    //    var formData = $(this).serialize();

       
    //});
    //$("table").DataTable({ searching: false,paging: false, info: false });
    $(".DeleteConfirmSweetAlert").on('click', function (e) {

        //console.log(e);
        e.preventDefault();
        swal({
            title: "Are you sure?",
            text: "You Want to Delete !",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                //console.log(willDelete)
                if (willDelete) {
                    swal(
                        "Successfully deleted!", {
                            icon: "success",
                            buttons: false,
                    })
                    setTimeout(() => {
                        $(this).parent().submit();
                    },1000)

                } else {
                    return false;

                    //swal("Your imaginary file is safe!");
                }

            });
    });
});