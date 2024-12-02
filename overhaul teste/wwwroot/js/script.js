let ul = document.querySelector('nav .ul');

function openMenu() {
    ul.classList.add('open');
}

function closeMenu() {
    ul.classList.remove('open');
}

var modal = document.getElementById("modal");
var modalImg = document.getElementById("imagem-ampliada");
var captionText = document.getElementById("caption");
var span = document.getElementsByClassName("fechar")[0];
var imagens = document.querySelectorAll(".imagem-clicavel");

imagens.forEach(function(img) {
    img.onclick = function(){
        modal.style.display = "block";
        modalImg.src = this.src;
        captionText.innerHTML = this.alt;
    }
});

span.onclick = function() { 
    modal.style.display = "none";
}

modal.onclick = function(event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}