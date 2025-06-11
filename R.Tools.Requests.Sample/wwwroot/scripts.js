(function () {
    let token = null;

    const li = document.querySelector("#login");
    const lo = document.querySelector("#logout");
    const un = document.querySelector("#username");
    const dot = document.querySelector("#dot");
    const alert = document.querySelector("#alert");
    const error = document.querySelector("#error");

    const reloadProducts = async function () {
        const t = document.querySelector("#products");
        const b = t.tBodies[0];

        const products = await (await fetch("/api/product")).json();

        var nb = document.createElement('tbody');
        products.forEach(p => {
            var r = document.createElement('tr');
            r.className = "table-success";
            var n = document.createElement('td');
            n.innerText = p.name;
            var d = document.createElement('td');
            d.innerText = p.description;
            var pr = document.createElement('td');
            pr.innerText = p.price + '€';

            r.appendChild(n);
            r.appendChild(d);
            r.appendChild(pr);

            nb.appendChild(r);
        });
        t.replaceChild(nb, b);
    }

    const checkLogin = function () {

        const userData = localStorage.getItem("user");
        if (!userData) {
            un.classList.add("d-none");
            lo.classList.add("d-none");
            return;
        }
        const user = JSON.parse(userData);
        if (!user.token) {
            un.classList.add("d-none");
            lo.classList.add("d-none");
            return;
        }
        token = user.token;
        li.classList.add("d-none");
        un.classList.remove("d-none");
        lo.classList.remove("d-none");

        un.textContent = user.name;
    }

    const assignEvents = function () {
        document.querySelector("#loginButton").addEventListener("click", async () => {
            
            const myModalEl = document.getElementById('loginModal');
            const modal = bootstrap.Modal.getInstance(myModalEl);
            const userLogin = document.querySelector('#loginText');
            const pass = document.querySelector('#passText');
            if (!userLogin.value) {
                userLogin.classList.add("is-invalid")
                return;
            }
            userLogin.classList.remove("is-invalid")
            if (!pass.value) {
                pass.classList.add("is-invalid")
                return;
            }
            pass.classList.remove("is-invalid")
            modal.hide();

            const resp = await (await fetch('api/auth', {
                body: JSON.stringify({
                    login: userLogin.value,
                    password: pass.value
                }),
                method: "POST",
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json'
                }
            })).json();

            localStorage["user"] = JSON.stringify({ token: resp.auth_token, name: userLogin.value });
            checkLogin();
        });
        lo.addEventListener("click", async () => {
            localStorage.removeItem("user");
            li.classList.remove("d-none");
            token = null;
            checkLogin();
        });
        document.querySelector("#createProduct").addEventListener("click", async () => {
            const rsp = await fetch('api/product', {
                body: JSON.stringify({
                    id: 0,
                    name: document.querySelector("#nm").value,
                    description: document.querySelector("#descr").value,
                    price: document.querySelector("#price").value || 0
                }),
                method: "POST",
                headers: {
                    'Accept': 'application/json, text/plain, */*',
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });
            if (rsp.ok) {
                await reloadProducts();
                return;
            }
            alert.classList.add("show");
            error.innerText = `Ooops! Something goes wrong. Please try again later. Details: [${rsp.status}] ${rsp.statusText}`;
            //const resp = await ().json();

        });
    };

    const ping = async function () {
        var resp = await fetch("api/ping", { method: "POST" });

        if (resp.ok) {
            dot.classList.remove("bg-danger");
            dot.classList.add("bg-success");
        } else {
            dot.classList.add("bg-danger");
            dot.classList.remove("bg-success");
        }
    };

    assignEvents();
    checkLogin();
    reloadProducts();
    setInterval(ping, 1000);
})();


