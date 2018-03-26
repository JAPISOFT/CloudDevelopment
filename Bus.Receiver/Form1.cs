﻿using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.ServiceBus;
using Message = Microsoft.Azure.ServiceBus.Message;

namespace Bus.Receiver
{
    public partial class Receiver : Form
    {
        private readonly QueueClient _queueClient;

        public Receiver()
        {
            InitializeComponent();

            _queueClient = new QueueClient(ConfigurationManager.AppSettings.Get("ServiceBusConnectionString"), "demo")
            {
                PrefetchCount = 1,
            };
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            // todo: log exception
            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
        {
            MessageText.Text += Encoding.UTF8.GetString(message.Body) + Environment.NewLine;

            Thread.Sleep(new Random(Guid.NewGuid().GetHashCode()).Next(1000, 2000));

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private void ReceiveButton_Click(object sender, EventArgs e)
        {
            ReceiveButton.Enabled = false;

            _queueClient.RegisterMessageHandler(ProcessMessageAsync,
                new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                }
            );
        }
    }
}