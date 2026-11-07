using System;
using TP_ControlVehicular.Negocio.Services;
using Xunit;

namespace TP_ControlVehicular.Tests.Services
{
    public class ErrorNotifierServiceTests
    {
        [Fact]
        public void Notify_ConExcepcion_DisparaEvento()
        {
            var svc = new ErrorNotifierService();
            ErrorNotificadoEventArgs? recibido = null;
            svc.ErrorOccurred += (s, e) => recibido = e;

            svc.Notify(new Exception("boom"), "Ctx");

            Assert.NotNull(recibido);
            Assert.Equal("boom", recibido!.Mensaje);
            Assert.Equal("Ctx", recibido.Contexto);
        }

        [Fact]
        public void Notify_Dedup_SuprimeDuplicadoEn2s()
        {
            var svc = new ErrorNotifierService();
            int count = 0;
            svc.ErrorOccurred += (s, e) => count++;

            svc.Notify(new Exception("dup"), "Ctx2");
            svc.Notify(new Exception("dup"), "Ctx2");

            Assert.Equal(1, count);
        }

        [Fact]
        public void Notify_SinExcepcion_NoLanzaYNotifica()
        {
            var svc = new ErrorNotifierService();
            ErrorNotificadoEventArgs? recibido = null;
            svc.ErrorOccurred += (s, e) => recibido = e;

            svc.Notify(null, "Ctx3");

            Assert.NotNull(recibido);
            Assert.Contains("Ctx3", recibido!.Mensaje);
        }
    }
}