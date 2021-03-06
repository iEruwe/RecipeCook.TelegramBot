using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Console_TelegramBot.Classes.Commands
{
    //наследование от абстрактного класса Command
    class Command_Random : Command
    {
        public override string Name => "/random";

        public override async void Execute(Message message, TelegramBotClient Bot)
        {
            //создание случайного числа, которое будет id для запроса
            Random random = new Random();

            int recipe_id = random.Next(1, 999999);

            Console.WriteLine("    #Searching for recipe...");

            //создаём запрос на Spoonacular API с использованием полученного id
            HttpWebRequest recipe_request = (HttpWebRequest)WebRequest.Create("https://api.spoonacular.com/recipes/" + recipe_id + "/information?apiKey=eaaa3a4d5e9046d0800682645d0a4cd2");
            HttpWebResponse recipe_response = (HttpWebResponse)recipe_request.GetResponse();

            Stream recipe_stream = recipe_response.GetResponseStream();
            StreamReader recipe_sr = new StreamReader(recipe_stream);

            string recipe_SReadData = recipe_sr.ReadToEnd();
            recipe_response.Close();

            dynamic recipe_d = JsonConvert.DeserializeObject(recipe_SReadData);

            //считываем данные из полученного JSON пакета и распеределяем по переменным
            bool vegetarian = recipe_d.vegetarian;
            Console.WriteLine("        #Vegetarian: " + vegetarian);

            bool vegan = recipe_d.vegan;
            Console.WriteLine("        #Vegan: " + vegan);

            bool glutenFree = recipe_d.glutenFree;
            Console.WriteLine("        #GlutenFree: " + glutenFree);

            bool dairyFree = recipe_d.dairyFree;
            Console.WriteLine("        #dairyFree: " + dairyFree);

            int weightWatcherSmartPoints = recipe_d.weightWatcherSmartPoints;
            Console.WriteLine("        #weightWatcherSmartPoints: " + weightWatcherSmartPoints);

            int aggregateLikes = recipe_d.aggregateLikes;
            Console.WriteLine("        #aggregateLikes: " + aggregateLikes);

            int healthScore = recipe_d.healthScore;
            Console.WriteLine("        #healthScore: " + healthScore);

            //создаём переменную первого сообщения в котором будут баллы блюда
            string recipe_Scores = recipe_d.title + "\n\n";

            //отправка пользователю фотографии блюда
            await Bot.SendPhotoAsync(message.Chat.Id, recipe_d.image.ToString());

            //редактирование первого сообщения
            recipe_Scores += (char.ConvertFromUtf32(0x1F44D) + " Aggregate Likes: " + aggregateLikes + "\n");
            recipe_Scores += (char.ConvertFromUtf32(0x2764) + " Health Score: " + healthScore + "\n");
            recipe_Scores += (char.ConvertFromUtf32(0x1F354) + " Weight Watcher Smart Points: " + weightWatcherSmartPoints + "\n");
            recipe_Scores += ("\n");

            if (glutenFree == true)
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F35E) + " Is gluten free\n");
            }
            else
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F35E) + " Is NOT gluten free\n");
            }

            if (dairyFree == true)
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F95B) + " Is dairy free\n");
            }
            else
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F95B) + " Is NOT dairy free\n");
            }

            if (vegetarian && vegan == true)
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F34F) + " Suitable for vegetarians and vegans\n");
            }
            else if (vegetarian)
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F34F) + " Suitable for vegetarians but not vegans\n");
            }
            else if (vegan)
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F34F) + " Suitable for vegans but not vegetarians\n");
            }
            else
            {
                recipe_Scores += (char.ConvertFromUtf32(0x1F34F) + " Not suitable for vegetarians and vegans\n");
            }

            //отправка первого сообщения
            await Bot.SendTextMessageAsync(message.Chat.Id, recipe_Scores);

            Console.WriteLine("        #Getting recipe...");
            //создание второго сообщения - самого рецепта
            string instructions = recipe_d.instructions;

            //удаление из него тегов HTML
            Regex ol = new Regex(@"<ol>");
            Regex ol2 = new Regex(@"</ol>");
            Regex li = new Regex(@"<li>");
            Regex li2 = new Regex(@"</li>");

            instructions = ol.Replace(instructions, "");
            instructions = ol2.Replace(instructions, "");
            instructions = li.Replace(instructions, "");
            instructions = li2.Replace(instructions, "");


            Console.WriteLine("        #Recipe sended");
            Console.WriteLine("        #Task complete\n");
            //отправка второго сообщения
            await Bot.SendTextMessageAsync(message.Chat.Id, instructions);

            Console.WriteLine("");

        }
    }
}
