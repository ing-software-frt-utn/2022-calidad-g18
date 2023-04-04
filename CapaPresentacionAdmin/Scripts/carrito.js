$(document).ready(function () {
    / Obtener el valor actual del contador en el carrito
    var contador = parseInt($("#contador-carrito").text());
    // Almacenar los IDs de los vehículos agregados en un arreglo
    var vehiculosAgregados = [];

    // Agregar evento click al botón "Agregar al carrito"
    $("#agregarcarrito").click(function () {
        // Obtener el ID del vehículo seleccionado
        var idVehiculo = $(this).data("idvehiculo");

        // Verificar si el ID del vehículo ya se encuentra en el arreglo de vehículos agregados
        if (!vehiculosAgregados.includes(idVehiculo)) {
            // Si no está, agregar el ID al arreglo de vehículos agregados
            vehiculosAgregados.push(idVehiculo);

            // Incrementar el contador en 1
            contador++;
            // Actualizar el contador en la etiqueta span
            $("#contador-carrito").text(contador);

            // Almacenar el valor del contador y el arreglo de vehículos agregados en la sesión del usuario
            sessionStorage.setItem("contador", contador);
            sessionStorage.setItem("vehiculosAgregados", JSON.stringify(vehiculosAgregados));

            // Mostrar mensaje de éxito
            swal("", "Agregado al carrito", "success");
        } else {
            // Mostrar mensaje de advertencia
            swal("", "Vehiculo ya agregado", "warning");
        }
    });

    // Agregar evento click al botón "Limpiar carrito"
    $("#limpiar-carrito").click(function () {
        // Reiniciar el contador
        contador = 0;
        // Reiniciar el arreglo de vehículos agregados
        vehiculosAgregados = [];
        // Actualizar el contador en la etiqueta span
        $("#contador-carrito").text(contador);

        // Limpiar los valores del contador y el arreglo de vehículos agregados de la sesión del usuario
        sessionStorage.removeItem("contador");
        sessionStorage.removeItem("vehiculosAgregados");

        // Mostrar mensaje de éxito
        swal("", "Carrito limpiado", "success");
    });
});