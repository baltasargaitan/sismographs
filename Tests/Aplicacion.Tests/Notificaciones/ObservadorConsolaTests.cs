using Xunit;
using Aplicacion.Servicios.Notificaciones;

namespace Aplicacion.Tests.Notificaciones
{
    public class ObservadorConsolaTests
    {
        [Fact]
        public void ActualizarDebeLoguearMensaje()
        {
            // Arrange
            var observador = new ObservadorConsola();
            var mensaje = "Test notification";
            var destinatario = "test@example.com";

            // Act & Assert (no debe lanzar excepción)
            observador.Actualizar(mensaje, destinatario);
        }

        [Fact]
        public void ActualizarConMensajeVacio_NoDebeLanzarExcepcion()
        {
            // Arrange
            var observador = new ObservadorConsola();

            // Act & Assert
            observador.Actualizar("", "test@example.com");
        }
    }
}
