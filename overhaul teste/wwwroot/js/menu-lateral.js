
//  menu latera o que tava funcionando mas agr ta com fogo 
document.getElementById('menu-toggle').addEventListener('click', function(event) {
    event.stopPropagation();
    document.getElementById('side-menu').classList.add('open');
});

document.getElementById('close-menu').addEventListener('click', function(event) {
    event.stopPropagation();
    document.getElementById('side-menu').classList.remove('open');
});

document.addEventListener('click', function(event) {
    const sideMenu = document.getElementById('side-menu');
    const menuToggle = document.getElementById('menu-toggle');

    if (!sideMenu.contains(event.target) && !menuToggle.contains(event.target)) {
        sideMenu.classList.remove('open');
    }
});

