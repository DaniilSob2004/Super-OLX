$(document).ready(function () {
    // обработчик клика на кнопке "Деактивировать объявление"
    $(".btn-state-announ").on("click", function() {
        var id = $(this).data("id");
        var action = $(this).data("action");

        var modalTitle = "";
        var modalMessage = "";

        if (action === "deactiveAnnoun") {
            modalTitle = "Вы действительно хотите деактивировать объявление?";
            modalMessage = "Это действие обратимо";
        }
        else if (action === "activeAnnoun") {
            modalTitle = "Вы действительно хотите активировать объявление?";
            modalMessage = "Это действие обратимо";
        }

        $("#modalTitle").text(modalTitle);
        $("#modalMessage").text(modalMessage);

        $(".modalBackground").show();

        // AJAX (клик по кнопке ДА)
        $(".modalBtnYes").off("click").on("click", function () {
            handleAction(action, id);
        });

        function handleAction(action, id) {
            const urlMap = {
                "deactiveAnnoun": "/Announcement/Deactivate",
                "activeAnnoun": "/Announcement/Activate"
            };

            const announcement = $(".row[data-id='" + id + "']");
            const btnState = announcement.find(".btn-state-announ");
            const textDeactiveBtn = btnState.attr("data-deactivate-text");
            const textActiveBtn = btnState.attr("data-activate-text");
            const textDeactiveStateBtn = btnState.attr("data-deactive-state-text");
            const textActiveStateBtn = btnState.attr("data-active-state-text");

            // значения для разных состояний кнопки
            const statusMap = {
                "deactiveAnnoun": {
                    text: ` -- ${textDeactiveStateBtn} --`,
                    addClass: "text-danger",
                    removeClass: "text-success",
                    buttonText: textActiveBtn,
                    buttonClassAdd: "btn-success",
                    buttonClassRemove: "btn-danger",
                    buttonAction: "activeAnnoun"
                },
                "activeAnnoun": {
                    text: ` -- ${textActiveStateBtn} --`,
                    addClass: "text-success",
                    removeClass: "text-danger",
                    buttonText: textDeactiveBtn,
                    buttonClassAdd: "btn-danger",
                    buttonClassRemove: "btn-success",
                    buttonAction: "deactiveAnnoun"
                }
            };

            $.ajax({
                type: "POST",
                url: urlMap[action],
                data: { id: id },
                success: () => {
                    const isActiveText = announcement.find(".isActiveText");
                    const status = statusMap[action];

                    isActiveText.removeClass(status.removeClass);
                    isActiveText.addClass(status.addClass);
                    isActiveText.text(status.text);

                    btnState.removeClass(status.buttonClassRemove)
                        .addClass(status.buttonClassAdd)
                        .text(status.buttonText)
                        .data("action", status.buttonAction);

                    $(".modalBackground").hide();
                },
                error: err => {
                    console.error(err);
                }
            });
        }
    });

    // закрытие окна (клик по НЕТ, крестик, и за пределы окна)
    $(".modalBackground, .modalBtnNo, .modalClose").on("click", (e) => {
        $(".modalBackground").hide();
    });
});
