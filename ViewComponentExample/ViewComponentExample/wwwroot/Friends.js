
document.querySelector("#load-friends-button").addEventListener("click", async () => {
    var response = await fetch("/friendslist", {
        method: "GET"
    });
    var content = await response.text();

    document.querySelector(".friends-content").hidden = false;
    document.querySelector(".friends-content").innerHTML = content;
})