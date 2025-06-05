console.log("✅ customize.js para EditarUsuario cargado correctamente");

const waitForUsuarioExpansion = setInterval(() => {
    const container = document.querySelector('[id^="operations-Usuario-put_api_Usuario_EditarUsuario"]');
    const bodyContainer = container?.querySelector('.opblock-body');
    const input = container?.querySelector('input[type="text"], input[type="number"]');
    const textarea = container?.querySelector('textarea');

    if (!container || !bodyContainer || !input || !textarea) {
        console.log("⌛ Esperando a que el contenedor, input o textarea aparezcan...");
        return;
    }

    clearInterval(waitForUsuarioExpansion);
    console.log("🎯 Contenedor, input y textarea encontrados");

    // Crear botón
    const button = document.createElement('button');
    button.textContent = "Buscar usuario";
    button.style = "margin-left: 10px; padding: 6px;";
    button.onclick = async () => {
        const id = input.value.trim();
        console.log("🔍 Buscando usuario con ID:", id);

        if (!id) {
            alert("⚠ Ingrese un ID válido.");
            return;
        }

        try {
            const res = await fetch(`/api/Usuario/BuscarUsuario/${id}`);
            if (!res.ok) {
                alert("❌ Usuario no encontrado.");
                return;
            }

            const data = await res.json();

            // Solo insertamos lo necesario para el PUT
            textarea.value = JSON.stringify({
                sede: data.sede,
                contrasena: "", // El usuario puede escribirla si desea cambiarla
                rol: data.rol
            }, null, 4);

            console.log("✅ Datos del usuario cargados en el request body correctamente");
        } catch (error) {
            alert("⚠ Error al buscar usuario.");
            console.error(error);
        }
    };

    // Insertar el botón a la par del input de ID
    input.parentElement.appendChild(button);
    console.log("✅ Botón 'Buscar usuario' insertado junto al input");
}, 500);
