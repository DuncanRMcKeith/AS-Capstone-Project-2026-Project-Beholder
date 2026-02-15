document.addEventListener("DOMContentLoaded", () => {

    const dice = document.querySelectorAll(".die");
    const btn = document.getElementById("rollBtn");
    const resultBox = document.getElementById("resultBox");

    dice.forEach(d => createFace(d));

    // Roll button click event
    btn.addEventListener("click", () => {
        const interval = setInterval(() => {
            dice.forEach(d => setFace(d, roll()));
        }, 70);

        setTimeout(() => {
            clearInterval(interval);

            let values = [];
            // Animate roll with staggered timing and show total number
            dice.forEach((d, i) => {
                setTimeout(() => {
                    const value = roll();
                    setFace(d, value);
                    values.push(value);

                    if (values.length === dice.length) {
                        const total = values.reduce((a, b) => a + b, 0);
                        const lowest = Math.min(...values);
                        resultBox.textContent = "Total: " + (total - lowest);
                    }

                }, i * 180);
            });

        }, 700);
    })
});

// dice roll function (1-6)
function roll() {
    return Math.floor(Math.random() * 6) + 1;
}

// Create dice face with 9 pips (3x3 grid)
function createFace(die) {
    die.innerHTML = "";
    for (let i = 0; i < 9; i++) {
        const dot = document.createElement("div");
        dot.className = "pip";
        die.appendChild(dot);
    }
}

// set dice face based on value
function setFace(die, value) {
    const map = {
        1: [4],
        2: [0, 8],
        3: [0, 4, 8],
        4: [0, 2, 6, 8],
        5: [0, 2, 4, 6, 8],
        6: [0, 2, 3, 5, 6, 8]
    };

    die.querySelectorAll(".pip").forEach(p => p.classList.remove("show"));
    map[value].forEach(i => die.children[i].classList.add("show"));
}