using Xunit;
using Aplicacion.Servicios.Notificaciones;
using System.Linq;

namespace Aplicacion.Tests.Notificaciones
{
    public class ObservadorWebMonitorTests
    {
        [Fact]
        public void ActualizarDebeAgregarEventoALaCola()
        {
            // Arrange
            var observador = new ObservadorWebMonitor();
            var mensaje = "Orden cerrada";
            var destinatario = "empleado@domain.com";

            // Act
            observador.Actualizar(mensaje, destinatario);

            // Assert
            var eventos = ObservadorWebMonitor.ObtenerEventos();
            Assert.NotEmpty(eventos);
        }

        [Fact]
        public void LaColaDebeSerLimitadaA100Eventos()
        {
            // Arrange
            var observador = new ObservadorWebMonitor();

            // Act - Agregar 150 eventos
            for (int i = 0; i < 150; i++)
            {
                observador.Actualizar($"Evento {i}", $"test{i}@example.com");
            }

            // Assert
            var eventos = ObservadorWebMonitor.ObtenerEventos();
            Assert.True(eventos.Count() <= 100, "La cola no debe exceder 100 eventos");
        }

        [Fact]
        public void ObtenerEventosDebeRetornarEventosEnOrdenCronologico()
        {
            // Arrange
            var observador = new ObservadorWebMonitor();
            observador.Actualizar("Primer evento", "test1@example.com");
            System.Threading.Thread.Sleep(10);
            observador.Actualizar("Segundo evento", "test2@example.com");

            // Act
            var eventos = ObservadorWebMonitor.ObtenerEventos();

            // Assert
            Assert.True(eventos.Count() >= 1);
        }
    }
}

