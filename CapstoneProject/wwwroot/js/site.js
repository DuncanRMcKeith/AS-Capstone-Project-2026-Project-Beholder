// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//CHATROOM CODE STARTS HERE
document.addEventListener("DOMContentLoaded", function () {

    const friends = [
        { id: 1, name: "John Doe" },
        { id: 2, name: "Jane Smith" },
        { id: 3, name: "Bob Johnson" }
    ]

    const communities = [
        { id: 101, name: "Entry Level Players" },
        { id: 102, name: "Advanced Campaigns" }
    ]

    const conversations = {}

    let lastList = null
    let currentConversationId = null
    let currentView = null

    // SIGNALR CONNECTION
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    connection.start()
        .then(() => console.log("SignalR Connected"))
        .catch(err => console.error("SignalR Connection Error: ", err));

    connection.on("ReceiveMessage", (conversationId, sender, message) => {

        if (!conversations[conversationId]) {
            conversations[conversationId] = []
        }

        conversations[conversationId].push({
            sender: sender,
            text: message
        })

        if (currentConversationId == conversationId) {
            const messages = document.getElementById("chat-messages")
            const p = document.createElement("p")
            p.innerHTML = `<strong>${sender}:</strong> ${message}`
            messages.appendChild(p)
            messages.scrollTop = messages.scrollHeight
        }
    })


    // VIEW CONTROLS
    window.toggleChat = function () {
        const chatBody = document.getElementById("chat-body");

        if (chatBody.style.display === "none" || chatBody.style.display === "") {
            chatBody.style.display = "flex";
        } else {
            chatBody.style.display = "none";
        }
    }

    window.showView = function (viewId) {
        document.getElementById("chatHome").style.display = "none";
        document.getElementById("communityDM").style.display = "none";
        document.getElementById("friendsDM").style.display = "none";
        document.getElementById(viewId).style.display = "block";
        document.getElementById("inputBox").style.display = "none";
        document.getElementById("conversationView").style.display = "none"

        if (viewId === "friendsDM") {
            renderFriends();
            currentView = viewId
            lastList = "friendsDM"
        }

        if (viewId === "communityDM") {
            renderCommunities()
            currentView = viewId
            lastList = "communityDM"
        }
    }

    window.goHome = function () {
        document.getElementById("chatHome").style.display = "block";
        document.getElementById("communityDM").style.display = "none";
        document.getElementById("friendsDM").style.display = "none";
        document.getElementById("inputBox").style.display = "none";
    }

    window.openConversation = function (entity) {

        // Leave previous conversation group if there is one
        if (currentConversationId !== null) {
            connection.invoke("LeaveConversation", currentConversationId)
                .catch(err => console.error(err.toString()));
        }

        // Join the new conversation group
        connection.invoke("JoinConversation", entity.id)
            .catch(err => console.error(err.toString()));

        document.getElementById("friendsDM").style.display = "none";
        document.getElementById("communityDM").style.display = "none"
        document.getElementById("conversationView").style.display = "flex"
        document.getElementById("inputBox").style.display = "flex"

        const input = document.getElementById("chat-input")
        const sendBtn = document.querySelector('#inputBox button')
        const messages = document.getElementById("chat-messages")
        messages.innerHTML = ""

        sendBtn.onclick = () => sendMessage(entity.id)

        input.onkeydown = (event) => {
            if (event.key === "Enter") {
                sendMessage(entity.id)
            }
        }

        if (!conversations[entity.id]) {
            conversations[entity.id] = []
        }

        conversations[entity.id].forEach(msg => {
            const p = document.createElement("p")
            p.innerHTML = `<strong>${msg.sender}: </strong> ${msg.text}`
            messages.appendChild(p)
        })

        currentConversationId = entity.id
    }

    window.closeConversation = function () {

        // Leave the current group on close
        if (currentConversationId !== null) {
            connection.invoke("LeaveConversation", currentConversationId)
                .catch(err => console.error(err.toString()));
        }

        currentConversationId = null

        document.getElementById("conversationView").style.display = "none"

        if (lastList === "friendsDM") {
            showView(lastList);
        } else if (lastList === "communityDM") {
            showView(lastList)
        }
    }


    // RENDERING
    window.renderFriends = function () {
        const listContainer = document.getElementById("friendsContainer")
        listContainer.innerHTML = ""

        friends.forEach(friend => {
            const btn = document.createElement("button")
            btn.textContent = friend.name
            btn.onclick = function () {
                openConversation(friend)
            }
            listContainer.appendChild(btn)
        })
    }

    window.renderCommunities = function () {
        const listContainer = document.getElementById("communityContainer")
        listContainer.innerHTML = ""

        communities.forEach(comm => {
            const btn = document.createElement("button")
            btn.textContent = comm.name
            btn.onclick = function () {
                openConversation(comm)
            }
            listContainer.appendChild(btn)
        })
    }


    // MESSAGING
    window.sendMessage = function (conversationId) {
        const input = document.getElementById("chat-input")
        const message = input.value.trim();

        if (!message) return;

        connection.invoke("SendMessage", conversationId, "You", message)
            .catch(err => console.error(err.toString()))

        input.value = ""
    }

});
