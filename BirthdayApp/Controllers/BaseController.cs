using Microsoft.AspNetCore.Mvc;

namespace BirthdayApp.Controllers
{
    public abstract class BaseController : Controller, IDisposable
    {
        protected readonly CancellationTokenSource Cts;

        public BaseController()
        {
            Cts = new CancellationTokenSource();
        }

        public void Dispose()
        {
            Cts.Cancel();
            Cts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
