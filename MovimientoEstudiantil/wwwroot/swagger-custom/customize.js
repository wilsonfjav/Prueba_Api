console.log("✅ customize.js para EditarEstudiante cargado correctamente");

const waitForEstudianteExpansion = setInterval(() => {
    const container = document.querySelector('[id^="operations-Estudiante-put_Estudiante_ModificarEstudiante"]');
    const bodyContainer = container?.querySelector('.opblock-body');
    const input = container?.querySelector('input[type="text"], input[type="number"]');
    const textarea = container?.querySelector('textarea');

    if (!container || !bodyContainer || !input || !textarea) {
        console.log("⌛ Esperando a que el contenedor, input o textarea aparezcan...");
        return;
    }

    clearInterval(waitForEstudianteExpansion);
    console.log("🎯 Contenedor, input y textarea encontrados");

    // Crear botón
    const button = document.createElement('button');
    button.textContent = "Buscar estudiante";
    button.style = "margin-left: 10px; padding: 6px;";
    button.onclick = async () => {
        const id = input.value.trim();
        console.log("🔍 Buscando estudiante con ID:", id);

        if (!id) {
            alert("⚠ Ingrese un ID válido.");
            return;
        }

        try {
            const res = await fetch(`/Estudiante/BuscarEstudiante/${id}`);
            if (!res.ok) {
                alert("❌ Estudiante no encontrado.");
                return;
            }

            const data = await res.json();

            // Cargar solo los datos necesarios (excluyendo idEstudiante)
            textarea.value = JSON.stringify({
                correo: data.correo,
                provincia: data.provincia,
                sede: data.sede,
                satisfaccionCarrera: data.satisfaccionCarrera,
                anioIngreso: data.anioIngreso
            }, null, 4);

            console.log("✅ Datos del estudiante cargados en el request body correctamente");
        } catch (error) {
            alert("⚠ Error al buscar estudiante.");
            console.error(error);
        }
    };

    // Insertar el botón a la par del input de ID
    input.parentElement.appendChild(button);
    console.log("✅ Botón 'Buscar estudiante' insertado junto al input");
}, 500);
