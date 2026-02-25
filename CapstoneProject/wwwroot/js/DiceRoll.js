
document.addEventListener("DOMContentLoaded", () => {

    const dice = document.querySelectorAll(".die");
    const btn = document.getElementById("rollBtn");
    const resultBox = document.getElementById("resultBox");

    dice.forEach(d => createFace(d));

    btn.addEventListener("click", () => {
        const rolling = setInterval(() => {
            dice.forEach(d => setFace(d, roll()));
        }, 70); // felt snappier than 100
        setTimeout(() => {
            clearInterval(rolling);
            let count = 0;
            let values = [];
            dice.forEach((d, i) => {
                setTimeout(() => {
                    const value = roll();
                    setFace(d, value);
                    values.push(value);
                    count++;
                    // DND roll for stats: roll 4 d6, drop lowest, sum remaining
                    if (count === dice.length) {
                        const total = values.reduce((a, b) => a + b, 0);
                        const lowest = Math.min(...values);
                        resultBox.textContent = "Total: " + (total - lowest);
                    }
                }, i * 180); // staggering dice at 180ms intervals
            });
        }, 700); // 700ms for rolling time
    });
});
function roll() {
    return Math.floor(Math.random() * 6) + 1;
}
function createFace(die) {
    die.innerHTML = "";
    for (let i = 0; i < 9; i++) {
        const dot = document.createElement("div");
        dot.className = "pip";
        die.appendChild(dot);
    }
}

// Maps each dice value to which pips should show on the face
function setFace(die, value) {
    const map = {
        1: [4],
        2: [0, 8],
        3: [0, 4, 8],
        4: [0, 2, 6, 8],
        5: [0, 2, 4, 6, 8],
        6: [0, 2, 3, 5, 6, 8]
    };
    die.querySelectorAll(".pip").forEach(p =>p.classList.remove("show"));
    map[value].forEach(i => die.children[i].classList.add("show"));
}