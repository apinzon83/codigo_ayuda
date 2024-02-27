const aplicacion = document.querySelector('.container')

/*
const url = 'https://jsonplaceholder.typicode.com/users'

fetch(url)
.then(res => res.json())
.then(data => {
    data.forEach(usuario => {
        console.log(usuario.name)
        const p = document.createElement('p')
        p.innerHTML = usuario.id + ' - ' + usuario.name
        aplicacion.appendChild(p)
    });
})
.catch(err => console.log(err))
*/

const url2 = 'http://localhost:8181/api/clientes'

fetch(url2)
.then(res => res.json())
.then(data => {
    data.forEach(cliente => {
        console.log(cliente.cardName)
        /*
        const p = document.createElement('p')
        p.innerHTML = cliente.idCliente +' - '+ cliente.cardName
        aplicacion.appendChild(p)
        */

        /*
        var table = document.getElementById('myTable')
        var row =   `<tr>
                        <td>${cliente.idCliente}</td>
                        <td>${cliente.cardCode}</td>
                        <td>${cliente.cardName}</td>
                        <td>${cliente.estatus}</td>
                    </tr>`
        table.innerHTML += row;
        */

        var table = document.getElementById('myTable')
        const row = document.createElement('tr')
        row =   `<tr>
                    <td>${cliente.idCliente}</td>
                    <td>${cliente.cardCode}</td>
                    <td>${cliente.cardName}</td>
                    <td>${cliente.estatus}</td>
                </tr>`
        row.addEvenListener('click',function(){
            alert('seleccionaste el id '+cliente.idCliente)
        })
        table.innerHTML += row;
    });
})
.catch(err => console.log(err))



