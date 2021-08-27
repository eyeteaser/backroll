using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BackRoll.Services.Abstractions;
using BackRoll.Services.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace BackRoll.Telegram.Bot
{
    public class StreamingHelper : IStreamingHelper
    {
        private readonly StreamingService[] _streamingServices;

        public StreamingHelper(IEnumerable<IStreamingService> streamingServices)
        {
            _streamingServices = streamingServices.Select(x => x.Name).ToArray();
        }

        public string GetStreamingPrettyName(StreamingService streamingService)
        {
            return streamingService switch
            {
                StreamingService.Spotify => "Spotify",
                StreamingService.YandexMusic => "Yandex Music",
                _ => streamingService.ToString(),
            };
        }

        public IReplyMarkup CreateStreamingButtonsMarkup(string commandPrefix, StreamingService exclude = StreamingService.Undefined)
        {
            var buttons = _streamingServices
                .Where(x => x != exclude)
                .Select(x => CreateStreamingButton(commandPrefix, x))
                .ToArray();
            var markup = new InlineKeyboardMarkup(buttons);
            return markup;
        }

        public bool TryParseStreamingData(string commandPrefix, string data, out StreamingService streamingService)
        {
            string streamingDataRegex = commandPrefix + @"_(?<service>\w+)";
            var match = Regex.Match(data, streamingDataRegex);
            if (match.Success)
            {
                string streaming = match.Groups["service"].Value;
                return Enum.TryParse(streaming, out streamingService);
            }

            streamingService = StreamingService.Undefined;
            return false;
        }

        private InlineKeyboardButton CreateStreamingButton(string commandPrefix, StreamingService streamingService)
        {
            return InlineKeyboardButton.WithCallbackData(GetStreamingPrettyName(streamingService), $"{commandPrefix}_{streamingService}");
        }
    }
}
