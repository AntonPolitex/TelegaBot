using System.Collections.Concurrent;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TelegaBot
{

    internal class Program
    {

        static void Main(string[] args)
        {
            string token = DataBase.GetToken();

            var botClient = new TelegramBotClient(token);


            var ro = new ReceiverOptions
            {
                AllowedUpdates = new Telegram.Bot.Types.Enums.UpdateType[] { },
            };

            //считываем базу данных всех пользователей имеющихся на данный момент
            ConcurrentDictionary<long, Person> persons = new ConcurrentDictionary<long, Person>();
            ConcurrentDictionary<string, Ingredient> ingredients = new ConcurrentDictionary<string, Ingredient>();
            ConcurrentDictionary<string, Dish> dishs = new ConcurrentDictionary<string, Dish>() { };
            
            foreach (Person person in DataBase.GetPersonInfo())
            {
                persons.TryAdd(person.Id, person);
            }
            
            foreach (Ingredient ingredient in DataBase.GetIngredientInfo())
            {
                ingredients.TryAdd(ingredient.Names,ingredient);
            }
            
           foreach (Dish dish in DataBase.GetDishInfo())
            {
                 dishs.TryAdd(dish.Name, dish);
            }
            


            WorkBot bot = new WorkBot(persons, ingredients,dishs, botClient);

            botClient.StartReceiving(updateHandler: bot.Handler, pollingErrorHandler: bot.ErrorHandler, receiverOptions: ro);
            Console.WriteLine("Стартовали");
            Console.ReadLine();
        }

    }
}
