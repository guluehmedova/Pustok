$(function (e) {
    $(document).on("click", ".delete-btn", function (e) {
        e.preventDefault();
        console.log("salam")
        let url = $(this).attr("href")
        let datax = $(this).attr("data-x")
        let id = $(this).attr("data-id")
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
                fetch(url)
                    .then(response => response.json())
                    .then(data => {
                        
                    })
            }
            else {

            }
        })
    })
})