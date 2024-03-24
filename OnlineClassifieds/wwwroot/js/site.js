// регулируем высоту списка чатов, в зависимости от высоты сообщений
function settingsHeightListChat() {
    const chatHistoryHeight = $(".chat-history").outerHeight();
    $(".people-list .chat-list").css("max-height", chatHistoryHeight + 80);
}

// сохраняем текущий идентификатор чата в куки
function setChatIdToCookie(chatId) {
    document.cookie = "currentChatId=" + chatId;
}

// получаем текущий идентификатор чата из куки}
function getCookie(name) {
    const cookieArray = document.cookie.split(";");
    for (let i = 0; i < cookieArray.length; i++) {
        let cookie = cookieArray[i].trim();
        if (cookie.startsWith(name + "=")) {
            return cookie.substring(name.length + 1);
        }
    }
    return null;
}

// подключение SignalR
$(document).ready(function () {
    // настройка к хабу
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/messagehub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // подключение к хабу
    connection.start()
        .catch((err) => {
            console.error(err.message);
        });

    // обработчик для динамического добавления сообщения от пользователя
    connection.on("ReceiveMessage", function (chatId, senderUserId, message) {
        let currentChatId = getCookie("currentChatId");  // полкчаем активный чат у пользователя
        if (chatId === currentChatId) {  // если он находится в нём, то отправляем AJAX-запрос для добавления сообщения
            $.ajax({
                url: "/Message/ShowNewMessage",
                type: "POST",
                data: { message: message, sendUserId: senderUserId, chatId: currentChatId },
                success: (data) => {
                    $("#chatHistory").append(data);  // добавляем сообщение в чат пользователя
                    $("#message").val("");  // очищаем текстовое поле для ввода
                    settingsHeightListChat();  // регулируем высоту списка чатов
                },
                error: (error) => {
                    console.error(error);
                }
            });
        }
    });

    // глобальное подключение доступно по всей странице
    window.chatConnection = connection;
});
