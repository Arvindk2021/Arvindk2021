var SAMG = SAMG || {};

SAMG.customerAuth = (function () {

    var playMediaElement = null;

    var init = function (url, language) {
        registerLoginFormSubmit();

        streamPayments.init(url, language);
        streamPayments.getSessionState(null, SAMG.customerAuth.sessionUpdateCallback);
    };

    var showLogin = function (e) {
        playMediaElement = null;
        if (e && e != null) playMediaElement = e;

        $("#customer-login-modal").modal("show");
    };

    var logout = function () {
        streamPayments.doLogout(null,

                //callback
                function () {
                    location.reload();
                });
    };

    var playMedia = function (e) {
        playMediaElement = e;
        var entryId = playMediaElement.closest(".kWidgetIframeContainer-wrapper").attr('data-entryid');
        streamPayments.getKSession({ entryId: entryId }, SAMG.customerAuth.playMediaCallback);
    };

    var checkIfLoggedIn = function () {
        var sessionData = streamPayments.getSessionState(null, function (data) {
            return data && data.CurrentCustomerSession;
        });
    };

    var isEmailAddressValid = function (email, addToMailingList) {
        streamPayments.isEmailAddressValid({ emailaddress: email['emailaddress'] }, function (data) {
            if (data) {
                streamPayments.isEmailAddressRegistered({ emailaddress: email['emailaddress'] }, function (isRegistered) {
                    if (isRegistered) {
                        $("#customer-login-modal").attr("data-url", $(".package-option.selected").attr("data-package-url") + "&emailaddress=" + $("#Email").val());
                        $(".email-response").show();
                        showLogin();
                    }
                    else {
                        if (addToMailingList) {
                            $("#mc_mv_EMAIL").val($("#Email").val());
                            $("#mc_signup_form").submit();
                        }
                        window.location = $(".package-option.selected").attr("data-package-url") + "&emailaddress=" + $("#Email").val();
                    }
                });
            }
            else {
                $(".email-response").show();
            }


        });
    };

    var doContactSubmission = function (contactform, callback) {
        streamPayments.doContactSubmission(contactform, callback);
    };

    var getSubscriptionPackageList = function (options, callback) {
        streamPayments.getSubscriptionPackageList(options, callback);
    };

    //*** Callback functions ***//

    var loginCallback = function (data) {

        if (Object.keys(data.ModelErrors).length == 0) {
            if ($("body").hasClass("page-template-register-template")) {
                var url = registerUrl + "step2/" + window.location.search.replace("?package=", "");
                window.location = url + (url.indexOf("?") > -1 ? "&" : "?") + "emailaddress=" + data.CurrentCustomerSession.CustomerEmailAddress;
            }
            else
                $("#customer-login-modal").modal("hide");
        }
        else {

            errorMessage = [];
            $("#modal_login_form input[data-val='true']").each(function (index) {
                var self = $(this);
                self.closest('.form-group').removeClass('has-error');
                self.parent().find('.field-error').remove();
                if (data.ModelErrors[self.attr('name')] !== undefined) {
                    errorMessage.push(data.ModelErrors[self.attr('name')]);
                    $(this).closest('.form-group').addClass('has-error');
                } else {
                    $.each(data.ModelErrors, function (key, value) {
                        if (jQuery.inArray(value, errorMessage) === -1) {
                            errorMessage.push(value);
                        }

                    });
                }

            });
            if (errorMessage != null) {
                var html = "";
                //remove dublicate errors from array
                var uniqueErrorMessage = [];
                $.each(errorMessage, function (i, el) {
                    if ($.inArray(el, uniqueErrorMessage) === -1) uniqueErrorMessage.push(el);
                });

                //use unique errors in array
                $(uniqueErrorMessage).each(function () {
                    html += "<p class=' help-block field-error'>" + this + "</p>";
                });
                $("#modal_login_error").html(html);
            }
        }
    };

    var playMediaCallback = function (data) {
        console.log(data.Status);
        if (data.Status == -1) {
            var container = playMediaElement.closest(".kWidgetIframeContainer-wrapper");
            kWidget.embed({
                "targetId": container.first().attr('id'),
                "wid": container.attr('data-wid'),
                "entry_id": container.attr('data-entryid'),
                "uiconf_id": container.attr('data-uiconfid'),
                "width": "100%",
                "height": "100%",
                "flashvars": {
                    "streamerType": "auto",
                    "autoPlay": "true",
                    "ks": data.KSession
                },
                "captureClickEventForiOS": true
            });

            setPlayerAspectRatio();
            return;
        }

        var category = $('#category_name').val();
        if ((data.Status == 1 && category == 'exclusive') || (data.Status == 1 && category == 'free')) {
            var container = playMediaElement.closest(".kWidgetIframeContainer-wrapper");
            kWidget.embed({
                "targetId": container.first().attr('id'),
                "wid": container.attr('data-wid'),
                "entry_id": container.attr('data-entryid'),
                "uiconf_id": container.attr('data-uiconfid'),
                "width": "100%",
                "height": "100%",
                "flashvars": {
                    "streamerType": "auto",
                    "autoPlay": "true"
                },
                "captureClickEventForiOS": true
            });

            setPlayerAspectRatio();
            return;
        }
        if (data.Status == 0 && category == 'free') {
            var container = playMediaElement.closest(".kWidgetIframeContainer-wrapper");
            kWidget.embed({
                "targetId": container.first().attr('id'),
                "wid": container.attr('data-wid'),
                "entry_id": container.attr('data-entryid'),
                "uiconf_id": container.attr('data-uiconfid'),
                "width": "100%",
                "height": "100%",
                "flashvars": {
                    "streamerType": "auto",
                    "autoPlay": "true"
                },
                "captureClickEventForiOS": true
            });

            setPlayerAspectRatio();
            return;
        }

        if (data.Status == 0 && category != 'free') {
            showLogin(playMediaElement);
            return;
        }

        $(".customer-ksession-modal-status").hide();
        $("#customer-ksession-modal-status-1").show();
        $("#customer-ksession-modal").modal("show");
    };

    var sessionUpdateCallback = function (sessionData) {
        if (sessionData.CurrentCustomerSessionStatus == -1) {
            $("body").removeClass("customer-unauthenticated");
            $("body").addClass("customer-authenticated");

            if ($("body").hasClass("home")) {
                $(".hide-unauthenticated").show();
                $("footer").show();
                fitToPage();
            }
            else {
                $("header .hide-unauthenticated").show();
            }

            $(".logged-in-user").html(sessionData.CurrentCustomerSession["CustomerFirstName"] + " " + sessionData.CurrentCustomerSession["CustomerLastName"]);
            return;
        }

        $("body").addClass("customer-unauthenticated");
        $("body").removeClass("customer-authenticated");

        if ($("body").hasClass("home")) {
            $(".hide-authenticated").show();
            $("footer").show();
            fitToPage();
            return;
        }

        $("header .hide-authenticated").show();
    };

    //*** End of Callback functions ***//


    //*** Private functions ***//

    var registerLoginFormSubmit = function () {
        $("#modal_login_form").submit(function (e) {
            e.preventDefault();
            streamPayments.doLogin($("#modal_login_form").serialize(), SAMG.customerAuth.loginCallback);
        });
    };
    var isVoucherCodeValid = function (code) {
        streamPayments.isVoucherCodeValid(
            {
                code: code
            },
            function (data) {
                if (data.IsGift === true && data.IsValid === true) {
                    window.location.replace(data.RedirectUrl);
                } else {
                    $(".packages-wrapper").append("<div class='voucher-error alert alert-danger'><strong>" + data.ModelErrors.code + "</strong></div>");
                }
                return data;
            }
        );
    }
    //*** End of Private functions ***//


    return {
        init: init,
        showLogin: showLogin,
        logout: logout,
        playMedia: playMedia,
        checkIfLoggedIn: checkIfLoggedIn,
        isEmailAddressValid: isEmailAddressValid,
        getSubscriptionPackageList: getSubscriptionPackageList,
        doContactSubmission: doContactSubmission,
        isVoucherCodeValid: isVoucherCodeValid,

        //Callbacks
        loginCallback: loginCallback,
        playMediaCallback: playMediaCallback,
        sessionUpdateCallback: sessionUpdateCallback
    };

}());