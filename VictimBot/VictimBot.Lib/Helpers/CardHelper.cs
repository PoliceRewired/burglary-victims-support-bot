using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VictimBot.Lib.Helpers
{
    public static class CardHelper
    {
        public static string PngTo64(this byte[] png)
        {
            return "data:image/png;base64," + Convert.ToBase64String(png);
        }

        public static IMessageActivity GenerateHero(string heading, string subheading, string content, params byte[][] pngs)
        {
            var images = pngs.Select(png => new CardImage(png.PngTo64(), alt: "greeting image"));

            var card = new HeroCard(
                title: heading,
                subtitle: subheading,
                text: content,
                images: images.ToList(),
                buttons: null,
                tap: null);

            return MessageFactory.Attachment(card.ToAttachment());
        }


    }
}
