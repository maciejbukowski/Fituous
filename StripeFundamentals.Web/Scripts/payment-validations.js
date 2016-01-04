jQuery(function ($) {
    $('.cc-number').payment('formatCardNumber');

    $('.cc-number').bind('change paste blur', function (e) {
        $('.cc-number').toggleInputError(!$.payment.validateCardNumber($('.cc-number').val()));
        refreshValidationMessage();
    });

    function refreshValidationMessage() {
        $('.validation').removeClass('text-danger text-success');
        $('.validation').addClass($('.has-error').length ? 'text-danger' : 'text-success');
    }

    $.fn.toggleInputError = function (erred) {
        this.closest('.form-group').toggleClass('has-error', erred);
        return this;
    };

    $(".cc-number").bind("change paste keypress", function () {
        $(".card-images img").removeClass("disabled");

        var cardType = $.payment.cardType($('.cc-number').val());
        if (!cardType) {
            return;
        }

        var selector = ".card-images img[alt!='cardType']";
        selector = selector.replace("cardType", cardType);
        $(selector).addClass("disabled");


    });

    $('.cc-exp').payment('formatCardExpiry');
    $('.cc-exp').bind('change paste blur', function (e) {
        $('.cc-exp').toggleInputError(!$.payment.validateCardExpiry($('.cc-exp').payment('cardExpiryVal')));
        refreshValidationMessage();
    });


    $('.cc-cvc').payment('formatCardCVC');

    $('.cc-cvc').bind('change paste blur submit', function (e) {
        var cardType = $.payment.cardType($('.cc-number').val());
        $('.cc-cvc').toggleInputError(!$.payment.validateCardCVC($('.cc-cvc').val(), cardType));
        refreshValidationMessage();
    });



    
});
