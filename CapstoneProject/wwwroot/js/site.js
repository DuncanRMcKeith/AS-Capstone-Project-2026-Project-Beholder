// CHATROOM CODE STARTS HERE
document.addEventListener("DOMContentLoaded", function () {

    const conversations = {}

    let lastList = null
    let currentConversationId = null
    let currentView = null

    // SIGNALR CONNECTION
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub?username=" + currentUser)
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

        if (!friends || friends.length === 0) {
            listContainer.innerHTML = "<p>No friends found.</p>"
            return
        }

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

        console.log(communities)
        const listContainer = document.getElementById("communityContainer")
        listContainer.innerHTML = ""

        if (!communities || communities.length === 0) {
            listContainer.innerHTML = "<p>No communities found.</p>"
            return
        }

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

        connection.invoke("SendMessage", conversationId, message)
            .catch(err => console.error(err.toString()))

        input.value = ""
    }

});
