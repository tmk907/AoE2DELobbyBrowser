using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;

namespace AoE2DELobbyBrowser
{
    class Program
    {
        private const string appInstanceKey = "419CEECD-FDDD-41E2-BC3D-E203208D7974";

        [STAThread]
        static async Task<int> Main(string[] args)
        {
            System.Diagnostics.Debug.WriteLine($"LOG: Program Main {string.Join(",", args)}");

            WinRT.ComWrappersSupport.InitializeComWrappers();
            bool isRedirect = await DecideRedirection();
            if (!isRedirect)
            {
                System.Diagnostics.Debug.WriteLine($"LOG:Program not Redirect");
                Microsoft.UI.Xaml.Application.Start((p) =>
                {
                    var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    new App();
                });
            }

            return 0;
        }

        private static async Task<bool> DecideRedirection()
        {
            bool isRedirect = false;

            AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
            ExtendedActivationKind kind = args.Kind;

            try
            {
                AppInstance keyInstance = AppInstance.FindOrRegisterForKey(appInstanceKey);

                if (!keyInstance.IsCurrent)
                {
                    isRedirect = true;
                    System.Diagnostics.Debug.WriteLine("LOG:RedirectActivationTo");
                    await keyInstance.RedirectActivationToAsync(args);
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return isRedirect;
        }
    }
}