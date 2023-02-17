let deleteBtns = document.querySelectorAll(".delete-button");



deleteBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();

    let url = btn.getAttribute("href");
    let id = btn.getAttribute("data-id");
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
                .then(response => {
                    if (response.status == 200) {
                        window.location.reload(true);
                    } else {
                        alert(`${id}li data yoxdur`)
                    }
                })
        }
    })

}))

let deletebtnimages = document.querySelectorAll(".delete-image-button");

deletebtnimages.forEach(btn => btn.addEventListener("click", function (e) {
    btn.parentElement.remove()
}))

let deletebtnimage = document.querySelectorAll(".delete-image-btn");
deletebtnimage.forEach(btn => btn.addEventListener("click", function (e) {
    btn.parentElement.remove()
}))
