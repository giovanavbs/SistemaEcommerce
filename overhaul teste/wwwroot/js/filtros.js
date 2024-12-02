    const carros = [
        { imagem: "src/img/carro2.png", nome: "Continental 6.0 W12 Turbo Flying Spur", preco: 430000, km: 40000, ano: 2011, cor: "Branca", fabricante: "BYD", modelo: "Continental" },
        { imagem: "src/img/carro1.png", nome: "Cayenne 3.0 V6 E-Hybrid Coupe AWD S", preco: 740000, km: 24240, ano: 2022, cor: "Preta", fabricante: "Porsche", modelo: "Cayenne" },
        { imagem: "src/img/carro1.png", nome: "SUV", preco: 500000, km: 35000, ano: 2021, cor: "Azul", fabricante: "Porsche", modelo: "Cayenne" },
        { imagem: "src/img/carro1.png", nome: "Esportivo", preco: 950000, km: 10000, ano: 2023, cor: "Vermelha", fabricante: "BYD", modelo: "Continental" },
        { imagem: "src/img/carro1.png", nome: "Fusion Hybrid", preco: 120000, km: 20000, ano: 2018, cor: "Prata", fabricante: "Ford", modelo: "Fusion" },
        { imagem: "src/img/carro1.png", nome: "Mustang GT", preco: 280000, km: 15000, ano: 2020, cor: "Amarela", fabricante: "Ford", modelo: "Mustang" },
        { imagem: "src/img/carro1.png", nome: "Continental 6.0 W12 Turbo Flying Spur", preco: 430000, km: 40000, ano: 2011, cor: "Branca", fabricante: "BYD", modelo: "Continental" },
        { imagem: "src/img/carro1.png", nome: "Cayenne 3.0 V6 E-Hybrid Coupe AWD S", preco: 740000, km: 24240, ano: 2022, cor: "Preta", fabricante: "Porsche", modelo: "Cayenne" },
        { imagem: "src/img/carro1.png", nome: "SUV", preco: 500000, km: 35000, ano: 2021, cor: "Azul", fabricante: "Porsche", modelo: "Cayenne" },
    ];
    
    const carrosPorPagina = 3;
    let paginaAtual = 1;
    let carrosFiltrados = carros;
    
    function atualizarCarrosComPaginacao(carros) {
        const carList = document.querySelector(".car-list");
        carList.innerHTML = ""; 
    
        const inicio = (paginaAtual - 1) * carrosPorPagina;
        const fim = inicio + carrosPorPagina;
        const carrosPagina = carros.slice(inicio, fim);
    
        carrosPagina.forEach(carro => {
            const carItem = document.createElement("div");
            carItem.classList.add("car-item");
    
            carItem.innerHTML = `
                <img src="${carro.imagem}" alt="${carro.nome}">
                <div class="car-details">
                    <h2>${carro.nome}</h2>
                    <p>R$ ${carro.preco.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}</p>
                    <p>KM: ${carro.km}</p>
                    <p>Ano/Modelo: ${carro.ano}</p>
                    <p>Cor Externa: ${carro.cor}</p>
                    <a href="detalhes.html"><button>Mais Detalhes > </button></a>
                </div>
            `;
            carList.appendChild(carItem);
        });
        atualizarPaginacao(carros);
    }
    
    function atualizarPaginacao(carros) {
        const pagination = document.getElementById("pagination");
        pagination.innerHTML = "";
    
        const totalPaginas = Math.ceil(carros.length / carrosPorPagina);

        function criarBotao(numero) {
            const button = document.createElement("button");
            button.innerText = numero;
            button.classList.add("page-button");
            if (numero === paginaAtual) button.classList.add("active");
    
            button.addEventListener("click", () => {
                paginaAtual = numero;
                atualizarCarrosComPaginacao(carros);
            });
    
            pagination.appendChild(button);
        }

        criarBotao(1);

        if (totalPaginas > 5) {
            if (paginaAtual > 3) {
                const dots = document.createElement("span");
                dots.innerText = "...";
                pagination.appendChild(dots);
            }

            for (let i = Math.max(2, paginaAtual - 1); i <= Math.min(totalPaginas - 1, paginaAtual + 1); i++) {
                criarBotao(i);
            }
    
            if (paginaAtual < totalPaginas - 2) {
                const dots = document.createElement("span");
                dots.innerText = "...";
                pagination.appendChild(dots);
            }
        } else {

            for (let i = 2; i < totalPaginas; i++) {
                criarBotao(i);
            }
        }
        if (totalPaginas > 1) criarBotao(totalPaginas);
    }
    window.onload = function() {
        atualizarCarrosComPaginacao(carros);
    };