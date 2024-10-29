// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('#btn-menu').click(function () {
        $('body').toggleClass('side-menu-open');
        return false; 
    });

    $('.close-menu, .menu-overlay').click(function () {
        $('body').removeClass('side-menu-open');
        return false; 
    });

    // fechar o menu
    $('#sideMenu ul li a').click(function () {
        $('body').removeClass('side-menu-open');
    });
});