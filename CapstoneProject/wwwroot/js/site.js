// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//CHATROOM CODE STARTS HERE
document.addEventListener("DOMContentLoaded", async function () {

    // ---- LOAD FROM DATABASE ----
    let friends = []

    try {
        const friendsRes = await fetch('/ChatDataHandler?handler=Friends')
        if (friendsRes.ok) friends = await friendsRes.json()
    } catch (err) {
        // Failed to load friends
    }
    // ----------------------------

    const conversations = {}

    let lastList = null
    let currentConversationId = null

    // SIGNALR CONNECTION
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub?username=" + currentUser)
        .build();

    connection.start()
        .then(async () => {

            const friendsRes = await fetch('/ChatDataHandler?handler=Friends')
            if (friendsRes.ok) friends = await friendsRes.json()

            renderFriends()

            friends.forEach(friend => {
                const roomId = [currentUserId, friend.id].sort((a, b) => a - b).join("_")
                connection.invoke("JoinConversation", roomId)
                    .catch(err => console.error(err.toString()))
            })
        })
        .catch(err => console.error("SignalR Connection Error: ", err));

    connection.on("ReceiveMessage", (conversationId, sender, message) => {

        if (!conversations[conversationId]) {
            conversations[conversationId] = []
        }

        conversations[conversationId].push({ sender: sender, text: message })

        if (currentConversationId == conversationId) {
            const messages = document.getElementById("chat-messages")
            const p = document.createElement("p")
            p.innerHTML = `<strong>${sender}:</strong> ${message}`
            messages.appendChild(p)
            messages.scrollTop = messages.scrollHeight
        } else {
            const friend = friends.find(f => {
                const roomId = [currentUserId, f.id].sort((a, b) => a - b).join("_")
                return roomId === conversationId
            })

            if (friend) {
                const tabBtn = document.getElementById("friendsTabBtn")
                if (tabBtn) {
                    let badge = tabBtn.querySelector(".msg-badge")
                    if (!badge) {
                        badge = document.createElement("span")
                        badge.className = "msg-badge"
                        badge.style.cssText = "background:red; color:white; border-radius:50%; padding:2px 6px; font-size:11px; margin-left:6px;"
                        tabBtn.appendChild(badge)
                    }
                    badge.textContent = (parseInt(badge.textContent) || 0) + 1
                }

                const entityBtn = document.querySelector(`button[data-room="${conversationId}"]`)
                if (entityBtn) {
                    let badge = entityBtn.querySelector(".msg-badge")
                    if (!badge) {
                        badge = document.createElement("span")
                        badge.className = "msg-badge"
                        badge.style.cssText = "background:red; color:white; border-radius:50%; padding:2px 6px; font-size:11px; margin-left:6px;"
                        entityBtn.appendChild(badge)
                    }
                    badge.textContent = (parseInt(badge.textContent) || 0) + 1
                }
            }
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

    window.showFriends = function () {
        document.getElementById("chatHome").style.display = "none";
        document.getElementById("friendsDM").style.display = "block";
        document.getElementById("inputBox").style.display = "none";
        document.getElementById("conversationView").style.display = "none"
        renderFriends()
        lastList = "friendsDM"
        document.getElementById("friendsTabBtn")?.querySelector(".msg-badge")?.remove()
    }

    window.goHome = function () {
        document.getElementById("chatHome").style.display = "block";
        document.getElementById("friendsDM").style.display = "none";
        document.getElementById("inputBox").style.display = "none";
        currentConversationId = null
    }

    window.openConversation = async function (entity) {

        // Leave previous conversation group if there is one
        const roomId = [currentUserId, entity.id].sort((a, b) => a - b).join("_")

        if (currentConversationId !== null) {
            connection.invoke("LeaveConversation", currentConversationId)
                .catch(err => console.error(err.toString()));
        }

        // Join the new conversation group
        connection.invoke("JoinConversation", roomId)
            .catch(err => console.error(err.toString()));

        document.getElementById("friendsDM").style.display = "none";
        document.getElementById("conversationView").style.display = "flex"
        document.getElementById("inputBox").style.display = "flex"

        const input = document.getElementById("chat-input")
        const sendBtn = document.querySelector('#inputBox button')
        const messages = document.getElementById("chat-messages")
        messages.innerHTML = ""

        sendBtn.onclick = () => sendMessage(roomId)

        input.onkeydown = (event) => {
            if (event.key === "Enter") {
                sendMessage(roomId)
            }
        }

        currentConversationId = roomId

        try {
            const res = await fetch(`/ChatDataHandler?handler=Messages&roomId=${roomId}`)
            if (res.ok) {
                const history = await res.json()
                conversations[roomId] = history
                history.forEach(msg => {
                    const p = document.createElement("p")
                    p.innerHTML = `<strong>${msg.sender}:</strong> ${msg.text}`
                    messages.appendChild(p)
                })
                messages.scrollTop = messages.scrollHeight
            }
        } catch (err) {
            // Failed to load message history
        }
    }

    window.closeConversation = function () {

        // Leave the current group on close
        if (currentConversationId !== null) {
            connection.invoke("LeaveConversation", currentConversationId)
                .catch(err => console.error(err.toString()));
        }

        currentConversationId = null

        document.getElementById("conversationView").style.display = "none"
        document.getElementById("friendsDM").style.display = "block"
        renderFriends()
    }


    // RENDERING
    window.renderFriends = function () {
        const listContainer = document.getElementById("friendsContainer")

        const existingBadges = {}
        listContainer.querySelectorAll("button[data-room]").forEach(btn => {
            const badge = btn.querySelector(".msg-badge")
            if (badge) existingBadges[btn.dataset.room] = badge.textContent
        })

        listContainer.innerHTML = ""

        friends.forEach(friend => {
            const roomId = [currentUserId, friend.id].sort((a, b) => a - b).join("_")
            const btn = document.createElement("button")
            btn.textContent = friend.name
            btn.dataset.room = roomId

            if (existingBadges[roomId]) {
                const badge = document.createElement("span")
                badge.className = "msg-badge"
                badge.style.cssText = "background:red; color:white; border-radius:50%; padding:2px 6px; font-size:11px; margin-left:6px;"
                badge.textContent = existingBadges[roomId]
                btn.appendChild(badge)
            }

            btn.onclick = function () {
                btn.querySelector(".msg-badge")?.remove()
                openConversation(friend)
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