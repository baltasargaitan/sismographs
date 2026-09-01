using Xunit;
using Moq;
using Aplicacion.Servicios.Notificaciones;
using Aplicacion.Interfaces.Notificaciones;
using System;

namespace Aplicacion.Tests.Notificaciones
{
    public class SujetoCierreOrdenTests
    {
        [Fact]
        public void SuscribirObservador_DebeAgregarALaLista()
        {
            // Arrange
            var sujeto = new SujetoCierreOrden();
            var observadorMock = new Mock<IObservadorCierreOrden>();

            // Act
            sujeto.Suscribir(observadorMock.Object);

            // Assert
            // Verificamos que el observador fue agregado notificándolo
            sujeto.Notificar("test", "test@example.com");
            observadorMock.Verify(o => o.Actualizar("test", "test@example.com"), Times.Once);
        }

        [Fact]
        public void NotificarObservadores_DebeNotificarATodos()
        {
            // Arrange
            var sujeto = new SujetoCierreOrden();
            var observador1 = new Mock<IObservadorCierreOrden>();
            var observador2 = new Mock<IObservadorCierreOrden>();
            var observador3 = new Mock<IObservadorCierreOrden>();

            sujeto.Suscribir(observador1.Object);
            sujeto.Suscribir(observador2.Object);
            sujeto.Suscribir(observador3.Object);

            // Act
            sujeto.Notificar("Orden cerrada", "empleado@domain.com");

            // Assert
            observador1.Verify(o => o.Actualizar("Orden cerrada", "empleado@domain.com"), Times.Once);
            observador2.Verify(o => o.Actualizar("Orden cerrada", "empleado@domain.com"), Times.Once);
            observador3.Verify(o => o.Actualizar("Orden cerrada", "empleado@domain.com"), Times.Once);
        }

        [Fact]
        public void DesuscribirObservador_NoDebeNotificar()
        {
            // Arrange
            var sujeto = new SujetoCierreOrden();
            var observadorMock = new Mock<IObservadorCierreOrden>();

            sujeto.Suscribir(observadorMock.Object);
            sujeto.Desuscribir(observadorMock.Object);

            // Act
            sujeto.Notificar("test", "test@example.com");

            // Assert
            observadorMock.Verify(o => o.Actualizar(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void NotificarConObservadorFallido_NoDebeDetenerOtrosObservadores()
        {
            // Arrange
            var sujeto = new SujetoCierreOrden();
            var observadorFallido = new Mock<IObservadorCierreOrden>();
            var observadorExitoso = new Mock<IObservadorCierreOrden>();

            observadorFallido
                .Setup(o => o.Actualizar(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InvalidOperationException("Simulated failure"));

            sujeto.Suscribir(observadorFallido.Object);
            sujeto.Suscribir(observadorExitoso.Object);

            // Act & Assert (no debe lanzar excepción)
            sujeto.Notificar("test", "test@example.com");

            // Verificamos que el observador exitoso fue notificado a pesar de la falla del otro
            observadorExitoso.Verify(o => o.Actualizar("test", "test@example.com"), Times.Once);
        }
    }
}

