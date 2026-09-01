using Xunit;
using Dominio.Entidades;
using System;

namespace Dominio.Tests.Entidades
{
    public class OrdenDeInspeccionTests
    {
        [Fact]
        public void Estado_DebeIndicarSiEsCerrada()
        {
            // Arrange
            var estadoCerrado = new Estado("OrdenInspeccion", "Cerrada");
            var estadoAbierto = new Estado("OrdenInspeccion", "Abierta");

            // Act & Assert
            Assert.True(estadoCerrado.EsCerrada());
            Assert.False(estadoAbierto.EsCerrada());
        }

        [Fact]
        public void Estado_DebeIndicarSiEsCompletada()
        {
            // Arrange
            var estadoCompletado = new Estado("OrdenInspeccion", "Completada");
            var estadoAbierto = new Estado("OrdenInspeccion", "Abierta");

            // Act & Assert
            Assert.True(estadoCompletado.EsCompletamenteRealizada());
            Assert.False(estadoAbierto.EsCompletamenteRealizada());
        }

        [Fact]
        public void Estado_DebeAlmacenarAmbitoYNombre()
        {
            // Arrange & Act
            var estado = new Estado("OrdenInspeccion", "Abierta");

            // Assert
            Assert.Equal("OrdenInspeccion", estado.Ambito);
            Assert.Equal("Abierta", estado.GetNombre());
        }
    }
}

