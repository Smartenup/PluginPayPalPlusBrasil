var PaymentInfoPayPal = {

    init: function () {

        if (window.addEventListener) {
            window.removeEventListener('message', PaymentInfoPayPal.messageListener);
            window.addEventListener('message', PaymentInfoPayPal.messageListener, false);
            return true;
        }

        if (window.attachEvent) {
            window.detachEvent("message", PaymentInfoPayPal.messageListener);
            window.attachEvent("message", PaymentInfoPayPal.messageListener);
            return true;
        }

        return false;
    },

    doContinue: function () {

        if (Checkout.loadWaiting != false) return;

        Checkout.setLoadWaiting('payment-info');
        
        ppp.doContinue();
    },

    save: function () {

        Checkout.setLoadWaiting(false);

        PaymentInfoPayPal.returnContinueClick();
        PaymentInfoPayPal.returnBackClick();

        PaymentInfo.save();


    },

    back: function () {

        PaymentInfoPayPal.returnContinueClick();
        PaymentInfoPayPal.returnBackClick();

        Checkout.back();
        return false;
    },

    returnContinueClick: function () {

        $("#paymentoInfoConfirm").prop('onclick', null);
        $("#paymentoInfoConfirm").off("click");
        $("#paymentoInfoConfirm").on("click", PaymentInfo.Save);
    },

    changeContinueClick: function () {

        $("#paymentoInfoConfirm").prop('onclick', null);
        $("#paymentoInfoConfirm").off("click");
        $("#paymentoInfoConfirm").on("click", PaymentInfoPayPal.doContinue);
    },

    returnBackClick: function () {

        $("#paymentoInfoBack").prop('onclick', null);
        $("#paymentoInfoBack").off("click");
        $("#paymentoInfoBack").on("click", Checkout.back);

    },
    changeBackClick: function () {

        $("#paymentoInfoBack").prop('onclick', null);
        $("#paymentoInfoBack").off("click");
        $("#paymentoInfoBack").on("click", PaymentInfoPayPal.back);
    },

    messageListener: function (event) {

        try {

            var message = JSON.parse(event.data);


            if (typeof message['cause'] !== 'undefined') { //iFrame error handling

                Checkout.setLoadWaiting(false);

                ppplusError = message['cause'].replace(/['"]+/g, ""); //log & attach this error into the order if possible

                // <<Insert Code Here>>

                $.ajax({
                    cache: false,
                    type: "PUT",
                    url: "/Plugins/PaymentPayPalPlusBrasil/LogError",
                    data: { "ppplusError": ppplusError },
                    success: function () { },
                    error: function (xhr, ajaxOptions, thrownError) {
                        alert('Falha ao inserir log de retorno.');
                    }
                });

                switch (ppplusError) {

                    case "INTERNAL_SERVICE_ERROR": //javascript fallthrough
                    case "SOCKET_HANG_UP": //javascript fallthrough
                    case "socket hang up": //javascript fallthrough
                    case "connect ECONNREFUSED": //javascript fallthrough
                    case "connect ETIMEDOUT": //javascript fallthrough
                    case "UNKNOWN_INTERNAL_ERROR": //javascript fallthrough
                    case "fiWalletLifecycle_unknown_error": //javascript fallthrough
                    case "Failed to decrypt term info": //javascript fallthrough
                    case "INTERNAL_SERVER_ERROR":
                        //Internal error, reload the iFrame and ask the costumer to try again, if the problem persists check your integration and/or contact your PayPal POC.
                        // <<Insert Code Here>>
                        alert("Ocorreu um erro inesperado, por favor tente novamente.");
                        break;

                    case "RISK_N_DECLINE": //javascript fallthrough
                    case "NO_VALID_FUNDING_SOURCE_OR_RISK_REFUSED": //javascript fallthrough
                    case "TRY_ANOTHER_CARD": //javascript fallthrough
                    case "NO_VALID_FUNDING_INSTRUMENT":
                        //Payment declined by risk, inform the customer to contact PayPal or offer Express Checkout payment solution.
                        // <<Insert Code Here>>
                        alert("Seu pagamento não foi aprovado. Por favor utilize outro cartão, caso o problema persista entre em contato com o PayPal (0800-047-4482).");
                        break;

                    case "INVALID_OR_EXPIRED_TOKEN":
                        alert("A sua sessão expirou, por favor tente novamente."); //pt_BR
                        break;
                    case "CHECK_ENTRY":
                        alert("Por favor revise os dados de Cartão de Crédito inseridos."); //pt_BR
                        break;
                    default: //unknown error & reload payment flow
                        alert("Ocorreu um erro inesperado, por favor tente novamente."); //pt_BR

                }

            }

            if (message['action'] == 'checkout') { //PPPlus session approved, do logic here

                $("#ReturnPaymentPayPal").val(JSON.stringify(message))

                PaymentInfoPayPal.save();
            }

            if (message['action'] == 'enableContinueButton') {

                PaymentInfoPayPal.changeContinueClick();
                PaymentInfoPayPal.changeBackClick();

                Checkout.setLoadWaiting(false);
            }
            if (message['action'] == 'loaded') {

                PaymentInfoPayPal.changeContinueClick();
                PaymentInfoPayPal.changeBackClick();

                $(ppplusDisplayDiv).show("fast");

            }

        } catch (e) { //treat exceptions here

            // <<Insert Code Here>>

        }

    }
};


PaymentInfoPayPal.init();