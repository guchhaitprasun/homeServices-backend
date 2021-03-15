const serverMessage = [{ serviceName: "Catlog Services", message: "Container Started" },
    { serviceName: "Order Services", message: "Container Started" },
    { serviceName: "Admin Services", message: "Container Started" },
    { serviceName: "Provider Services", message: "Container Started" },
    { serviceName: "Notification Services", message: "Container Started" }]
function addHostName() {
    var hostName = `http://${location.hostname}:${location.port}`;
    var spans = document.querySelectorAll(".hostName")

    spans.forEach((ele) => {
        ele.innerHTML = hostName
    })

    let notificationUrl = hostName + "/" + "notifications"
    setInterval(() => {
        fetch(notificationUrl)
            .then(res => {
                return res.json();
            })
            .then(data => {
                let spinner = document.getElementById("logSpinner");
                if (spinner) {
                    spinner.parentElement.removeChild(spinner)
                }
                addLogs(serverMessage.concat(data));                
            })
    }, 1000);
}

function addLogs(data) {
    let logDiv = document.getElementById("logs");
    logDiv.innerHTML = "";
    data.forEach(element => {
        let div = document.createElement('div');
        div.setAttribute("class", "row")
        let string = `<div class = "col-2"><span><strong>${element.serviceName}</strong></div><div class = "col-10"> ::&nbsp; ${element.message}<span></div>`
        div.innerHTML = string;
        logDiv.appendChild(div);
    });

}

