using MicroPos.Core;
using MicroPos.Core.Authorization;
using Pinpad.Sdk.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace NekiIt.Proto.Laundry
{
    /// <summary>
    /// Responsible for simulating the laundry workflow.
    /// </summary>
    internal sealed class LaundryService
    {
        /// <summary>
        /// Responsible for access pinpad features, authoriza and cancel transactions.
        /// </summary>
        public ICardPaymentAuthorizer Authorizer { get; private set; }
        /// <summary>
        /// Collection of approved transactions.
        /// </summary>
        private List<IAuthorizationReport> Transactions { get; set; } = new List<IAuthorizationReport>();

        /// <summary>
        /// Start the main flow of reading laundry options and performing a transaction
        /// with the corresponding amount.
        /// </summary>
        public void StartMachine()
        {
            MicroPos.Platform.Uwp.CrossPlatformUwpInitializer.Initialize();

            // Constrói as mensagens que serão apresentadas na tela do pinpad:
            DisplayableMessages pinpadMessages = new DisplayableMessages();
            pinpadMessages.ApprovedMessage = "Aprovada";
            pinpadMessages.DeclinedMessage = "Declinada";
            pinpadMessages.InitializationMessage = "Inicializando";
            pinpadMessages.MainLabel = "Laundromat";
            pinpadMessages.ProcessingMessage = "Processando...";

            this.Authorizer = DeviceProvider.ActivateAndGetOneOrFirst("407709482", pinpadMessages);

            bool continueLooping = false;

            do
            {
                LaundryOptionCode option = this.GetOption();
                short time = this.GetTime();

                ITransactionEntry transaction = new TransactionEntry();
                transaction.Amount = this.GetAmount(option, time);
                transaction.CaptureTransaction = true;

                IAuthorizationReport authorizationReport = this.Authorizer.Authorize(transaction);
                
                if (authorizationReport.WasApproved == true)
                {
                    Debug.WriteLine("ATK: {0}, Valor: {1}", authorizationReport.AcquirerTransactionKey, 
                        authorizationReport.Amount);

                    this.TurnMachineOn(option, time);
                    continueLooping = this.PrompToContinue(string.Empty);
                    this.Transactions.Add(authorizationReport);
                }
                else
                {
                    continueLooping = this.PrompToContinue("Nao autorizada.");
                }
            }
            while (continueLooping == true);

            this.CancelTransactions();
            this.Authorizer.PinpadFacade.Communication.ClosePinpadConnection(pinpadMessages.MainLabel);
            Application.Current.Exit();
        }

        /// <summary>
        /// Cancel approved transactions at the end of the laundry workflow.
        /// All <see cref="Transactions"/> are cancelled.
        /// </summary>
        private void CancelTransactions()
        {
            foreach (IAuthorizationReport currentTransaction in this.Transactions)
            {
                this.Authorizer.Cancel(currentTransaction.AcquirerTransactionKey, currentTransaction.Amount);
            }
        }
        /// <summary>
        /// Simulates turning on a wash or dry machine.
        /// </summary>
        /// <param name="option">Laundry option.</param>
        /// <param name="time">Time to wash or dry clothes.</param>
        private void TurnMachineOn(LaundryOptionCode option, short time)
        {
            if (option == LaundryOptionCode.Dry)
            {
                this.Authorizer.PinpadFacade.Display.ShowMessage("SECADORA LIGADA",
                    string.Format("POR {0} MINUTOS", time),
                    DisplayPaddingType.Center);
            }
            else
            {
                this.Authorizer.PinpadFacade.Display.ShowMessage("LAVADORA LIGADA",
                    string.Format("POR {0} MINUTOS", time),
                    DisplayPaddingType.Center);
            }

            System.Threading.Tasks.Task.Delay(3000).Wait();
        }
        /// <summary>
        /// Show a message and prompt for the user to confirm or decline.
        /// </summary>
        /// <param name="label">Label to present at the first line of pinpad display.</param>
        /// <returns>Returns the user input.</returns>
        private bool PrompToContinue(string label)
        {
            this.Authorizer.PinpadFacade.Display.ShowMessage(label, "Continuar?", 
                DisplayPaddingType.Center);

            PinpadKeyCode key;

            do
            {
                key = this.Authorizer.PinpadFacade.Keyboard.GetKey();
            }
            while (key != PinpadKeyCode.Return && key != PinpadKeyCode.Cancel);

            return key == PinpadKeyCode.Return;
        }

        // getters
        /// <summary>
        /// Reads the laundry option, that is, if the user wants to use the wash or dry machine.
        /// </summary>
        /// <returns>The laundry option input by the user.</returns>
        private LaundryOptionCode GetOption()
        {
            // Read option:
            string option;
            do
            {
                option = this.Authorizer.PinpadFacade.Keyboard.DataPicker
                    .GetValueInOptions("OPCOES", "LAVAGEM", "SECAGEM");
            }
            while (string.IsNullOrEmpty(option) == true);

            return option.ToLaundryOptionCode();
        }
        /// <summary>
        /// Reads the time to wash or dry clothes.
        /// </summary>
        /// <returns>The time input by the user.</returns>
        private short GetTime()
        {
            Nullable<short> time;
            do
            {
                time = this.Authorizer.PinpadFacade.Keyboard.DataPicker
                    .GetValueInOptions("TEMPO EM MINUTOS", 15, 30, 45, 60);
            }
            while (time == null);

            this.Authorizer.PinpadFacade.Display.ShowMessage("Processando...", string.Empty, 
                DisplayPaddingType.Center);

            return (time.HasValue == true) ? time.Value : (short) 0;
        }
        /// <summary>
        /// Get transaction amount based on a combination of the laundry option
        /// (wash or dry) and the time the machine will be working.
        /// </summary>
        /// <param name="option">Laundry option.</param>
        /// <param name="time">Time to wash or dry clothes.</param>
        /// <returns>The transaction amount.</returns>
        private decimal GetAmount(LaundryOptionCode option, short time)
        {
            return ((int)option) + time / 10;
        }
    }
}
