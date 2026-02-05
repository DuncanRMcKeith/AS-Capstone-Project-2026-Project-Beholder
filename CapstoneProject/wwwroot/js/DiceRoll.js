document.addEventListener("DOMContentLoaded", () => {
    const cubes = document.querySelectorAll(".dice-cube");
    const btn = document.getElementById("rollBtn");
    if (!cubes.length || !btn) return;

    cubes.forEach(c => c.innerHTML = buildDiceFaces());
    btn.addEventListener("click", () => cubes.forEach(c => rollDice(c)));
});

function buildDiceFaces() {
    const names = ["one", "two", "three", "four", "five", "six"];
    return names.map(name => `
    <div class="dice-face ${name}">
      ${[1, 2, 3, 4, 5, 6, 7, 8, 9].map(n => `<span class="pip p${n}"></span>`).join("")}
    </div>
  `).join("");
}

function rollDice(cube) {
    const value = Math.floor(Math.random() * 6) + 1;

    const spinX = 360 * (Math.floor(Math.random() * 3) + 2);
    const spinY = 360 * (Math.floor(Math.random() * 3) + 2);

    cube.style.transform = `rotateX(${spinX}deg) rotateY(${spinY}deg)`;

    cube.addEventListener("transitionend", () => {
        cube.style.transform = getFaceTransform(value);
    }, { once: true });
}

function getFaceTransform(value) {
    switch (value) {
        case 1: return "rotateX(0deg) rotateY(0deg)";
        case 2: return "rotateX(0deg) rotateY(-90deg)";
        case 3: return "rotateX(0deg) rotateY(180deg)";
        case 4: return "rotateX(0deg) rotateY(90deg)";
        case 5: return "rotateX(-90deg) rotateY(0deg)";
        case 6: return "rotateX(90deg) rotateY(0deg)";
    }
}