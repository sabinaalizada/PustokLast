let adtoBasketBtns = document.querySelectorAll(".add-to-basket")

adtoBasketBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();

    let url = btn.getAttribute("href");

    fetch(url)
        .then(response => {
            if (response.status == 200) {
                alert("Baskete add olundu")
                window.location.reload(true);
            } else {
                alert("Error")
                window.location.reload(true);
            }
        })
}))


let decreasebtn = document.querySelectorAll(".decrease-count");

decreasebtn.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();

    let url = btn.getAttribute("href");

    fetch(url)
        .then(response => {
            if (response.status == 200) {
                alert("Azaldi")
                window.location.reload();
            } else {
                alert("Error")
                window.location.reload(true);
            }
        })
}))
