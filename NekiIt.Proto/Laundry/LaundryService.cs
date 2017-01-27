using MicroPos.Core;
using MicroPos.Core.Authorization;
using Pinpad.Sdk.Model;
using System;
using System.Collections.Generic;

namespace NekiIt.Proto.Laundry
{
    // TODO: Doc
    internal sealed class LaundryService
    {
        public ICardPaymentAuthorizer Authorizer { get; private set; }
        private List<IAuthorizationReport> Transactions { get; set; } = new List<IAuthorizationReport>();

        public void StartMachine()
        {
            MicroPos.Platform.Uwp.CrossPlatformUwpInitializer.Initialize();

            // Constrói as mensagens que serão apresentadas na tela do pinpad:
            DisplayableMessages pinpadMessages = new DisplayableMessages();
            pinpadMessages.ApprovedMessage = "Approved";
            pinpadMessages.DeclinedMessage = "Declined";
            pinpadMessages.InitializationMessage = "Ola";
            pinpadMessages.MainLabel = "Laundromat";
            pinpadMessages.ProcessingMessage = "Just a sec...";

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
                    this.TurnMachineOn(option, time);
                    this.PrompToContinue(string.Empty);
                    this.Transactions.Add(authorizationReport);
                }
                else
                {
                    continueLooping = this.PrompToContinue("Não autorizada");
                }
            }
            while (continueLooping == true);

            this.CancelTransactions();
        }

        private void CancelTransactions()
        {
            foreach (IAuthorizationReport currentTransaction in this.Transactions)
            {
                this.Authorizer.Cancel(currentTransaction.AcquirerTransactionKey, currentTransaction.Amount);
            }
        }

        private async void TurnMachineOn(LaundryOptionCode option, short time)
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

            await System.Threading.Tasks.Task.Delay(3000);
        }
        private bool PrompToContinue(string label)
        {
            this.Authorizer.PinpadFacade.Display.ShowMessage(label, "Deseja continuar?", 
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
        private short GetTime()
        {
            Nullable<short> time;
            do
            {
                time = this.Authorizer.PinpadFacade.Keyboard.DataPicker
                    .GetValueInOptions("TEMPO EM MINUTOS", 15, 30, 45, 60);
            }
            while (time == null);

            return (time.HasValue == true) ? time.Value : (short) 0;
        }
        private decimal GetAmount(LaundryOptionCode option, short time)
        {
            return ((int)option) + time / 10;
        }
    }
}
