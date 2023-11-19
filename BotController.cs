using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concentus.Oggfile;
using Concentus.Structs;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using GiphyDotNet.Model.Results;
using NAudio.Wave;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
//using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using Message = Telegram.Bot.Types.Message;

namespace superluigi
{
    internal class BotController
    {
        private TelegramBotClient _client;

        private Giphy _giphyClient;

        private ReceiverOptions _receiverOptions;

        private CancellationTokenSource _cancellationTokenSource;

        public BotController()
        {
            _client = new TelegramBotClient("6907480484:AAHSWNRbio8uKSCSSYw4GMTQaQ5bMduP4H4");

            _giphyClient = new Giphy("zoaE4NGJm6pIvgZPdZpotPEZtPwgs7UA");

            _cancellationTokenSource = new CancellationTokenSource();

            _receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }
            };
        }

        public void Start()
        {
            _client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: _receiverOptions,
                cancellationToken: _cancellationTokenSource.Token);
            Console.WriteLine("Bot start");
        }
        public void Stop()
        {
            _cancellationTokenSource.Cancel();

            Console.WriteLine("bot vikluchen");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            switch (update.Message.Type)
            {
                case MessageType.Voice:
                    await HandleVoiceMessage(update.Message);
                    break;
            }
            
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (Exception is ApiRequestException apiRequestException)
            {
                Console.WriteLine($"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}");
            }

            return Task.CompletedTask;
        }

        #region MessagesHandlers

        private async Task HandleVoiceMessage(Message message)
        {
            using (FileStream fileStream = new FileStream("../voice_message.ogg", FileMode.Create))
            {
                await _client.GetInfoAndDownloadFileAsync(message.Voice.FileId, fileStream, _cancellationTokenSource.Token);

                await ConvertOggAudioToWav(fileStream);

                string voiceText = await WavAudioToText();

                voiceText = voiceText.ToLower();

                if (voiceText.Contains("vikluchi pk"))
                {
                    ExecuteTurnOffPCCommand();
                }

                if (voiceText.Contains("perezagruzipk"))
                {
                    ExecuteRestartPCCommand();
                }

                if (voiceText.Contains("sdelay skrin"))
                {
                    await ExecuteTakeScreenShotCommand(message.Chad.Id);
                }

                if (voiceText.Contains("randomnayagifka"))
                {
                    await ExecuteRandomGifCommand(message.Chad.Id);
                }
            }
        }

        private async Task<string> WavAudioToText()
        {
            byte[] wavAudioBytes = System.IO.File.ReadAllBytes("../voice_message.wav");
        }
    }
}
