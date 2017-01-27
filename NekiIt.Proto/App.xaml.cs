using MicroPos.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace NekiIt.Proto
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        //protected override async void OnWindowCreated(WindowCreatedEventArgs args)
        //{
        //    base.OnWindowCreated(args);
            
        //    //// Recupera Advanced Query Syntax para passar ao FindAllAsync para recuperar serial devices.
        //    //string aqs = SerialDevice.GetDeviceSelector();
        //    //// Recupera coleção de dispositivos conectados.
        //    //DeviceInformationCollection dis = await DeviceInformation.FindAllAsync(aqs);
        //    //// Retorna lista de Ids dos dispositivos.
        //    //string [] ports = dis.Select(x => x.Id).ToArray();

        //    //foreach (string port in ports)
        //    //{
        //    //    SerialDevice device = await SerialDevice.FromIdAsync(port);

        //    //    if (device == null) { continue; }
        //    //    if (device.UsbVendorId == 0 || device.UsbProductId == 0) { continue; }

        //    //    device.WriteTimeout = TimeSpan.FromMilliseconds(1000);
        //    //    device.ReadTimeout = TimeSpan.FromMilliseconds(1000);
        //    //    device.BaudRate = 9600;
        //    //    device.Parity = SerialParity.None;
        //    //    device.StopBits = SerialStopBitCount.One;
        //    //    device.DataBits = 8;
        //    //    device.Handshake = SerialHandshake.None;

        //    //    DataWriter _writer = new DataWriter(device.OutputStream);

        //    //    byte[] opn = { 0x16, 0x4F, 0x50, 0x4E, 30, 30, 30, 0x17 };

        //    //    _writer.WriteBytes(opn);
        //    //    Task<UInt32> storeAsyncTask = _writer.StoreAsync().AsTask();
        //    //    UInt32 bytesWritten = await storeAsyncTask;
        //    //}

        //    //this.Authorizer = DeviceProvider.ActivateAndGetOneOrFirst("407709482");
        //}

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
