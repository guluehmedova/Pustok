$(function () {

    $(document).on("click", ".single-eye-btn", function () {
        console.log("salam burdayam")
        let id = $(this).attr("data-id");

        fetch(`/home/getbook/${id}`)
            .then(response => response.text())
            .then(data => {
                console.log(data)
                $("#detailModal .modal-content").html(data)
            })
        $("#detailModal").modal("show")
    })

    $(document).on("click", ".add-basket", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");
        fetch(url)
            .then(response => response.json())
            .then(data => {
                $(".cart-total .text-number").text(data.basketItems.length);
                $(".cart-total .price").text("£" + data.totalAmount)
                $("#card-products").html("");
                for (var i = 0; i < data.basketItems.length; i++) {
                    console.log("for un icindeyem")
                    let elem = `<div class="cart-product">
                                                    <a href="product-details.html" class="image">
                                                        <img src="/image/products/`+ data.basketItems[i].posterImage + `" alt="">
                                                    </a>
                                                    <div class="content">
                                                        <h3 class="title">
                                                            <a href="product-details.html">
                                                                `+ data.basketItems[i].name + `
                                                            </a>
                                                        </h3>
                                                        <p class="price"><span class="qty"> `+ data.basketItems[i].count + ` ×</span> £ ` + data.basketItems[i].price + `</p>
                                                        <button class="cross-btn"><i class="fas fa-times"></i></button>
                                                    </div>
                                                </div>`
                    $("#card-products").append(elem);
                }
            })
    })


})
