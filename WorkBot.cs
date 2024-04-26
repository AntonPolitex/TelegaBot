using System;
using System.Collections.Concurrent;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegaBot
{
    internal class WorkBot
    {
        static private Dictionary<string, TypeIngredient> _TypeIngredients = new Dictionary<string, TypeIngredient>()
           {
             {"Не_знаю",TypeIngredient.Unknown},
             {"Мясо",TypeIngredient.Meat},
             {"Рыба",TypeIngredient.Fish},
             {"Яица",TypeIngredient.Eggs},
             {"Молочная продукция",TypeIngredient.MilkProducts},
             {"Зерновое",TypeIngredient.Bread},
             {"Крупа",TypeIngredient.Cereals},
             {"Макароны",TypeIngredient.Pasta},
             {"Бобовые",TypeIngredient.Legumes},
             {"Овощ",TypeIngredient.Vegetables},
             {"Фрукт",TypeIngredient.Fruits},
             {"Ягод",TypeIngredient.Berries},
             {"Орех",TypeIngredient.Nuts},
             {"Гриб",TypeIngredient.Mushrooms},
             {"Кондитерское изделие",TypeIngredient.Confectionery},
             {"Съедобный Жир",TypeIngredient.EdibleFats},
             {"Напиток",TypeIngredient.Beverages}
            };
        static private Dictionary<string, CookingStage> _CookingStage = new Dictionary<string, CookingStage>()
           {
             {"Не_знаю",CookingStage.UnKnow},
             {"Сырой",CookingStage.Raw},
             {"На_пару",CookingStage.Steamed},
             {"Вареный",CookingStage.Boiled},
             {"Жареный",CookingStage.Fried}

            };

        private ConcurrentDictionary<long, Person> _persons = new ConcurrentDictionary<long, Person>() { };

        public ConcurrentDictionary<long, Person> RegistrationForms = new ConcurrentDictionary<long, Person>() { };

        public ConcurrentDictionary<string, Ingredient> _ingredients = new ConcurrentDictionary<string, Ingredient>() { };

        public ConcurrentDictionary<string, Dish> _dish = new ConcurrentDictionary<string, Dish>() { };
        private readonly ITelegramBotClient GlobalClient;
        public WorkBot(ConcurrentDictionary<long, Person> persons, ConcurrentDictionary<string, Ingredient> ingredients, ConcurrentDictionary<string, Dish> dish , ITelegramBotClient globalClient)
        {
            _persons = persons;
            _ingredients = ingredients;
            _dish = dish;
            GlobalClient = globalClient;
        }

        public async Task Handler(ITelegramBotClient client, Update update, CancellationToken ct)
        {   

            Person human;
            if (!_persons.TryGetValue(update.Message!.Chat.Id, out human))
            {
                human = new Person(update.Message.Chat.FirstName ,update.Message!.Chat.Id, new Dictionary<string, int>() { });


                _persons.TryAdd(update.Message.Chat.Id, human);

            }


            if (human.cmd.TypeCom == TypeComad.None)
            {
                if (update.Message.Text != "")
                {
                    _persons[human.Id].cmd = new Comand(update.Message.Text);
                }
                else
                {
                    await client.SendTextMessageAsync(chatId: human.Id,
                                  text: $"Введите команду");
                }

            }
           if (human.Role == RolePerson.Guest && human.cmd.TypeCom != TypeComad.Registretion)
            {
                await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                             text: $"Я работаю только с зарегистрированными пользователями.\n для регистрации введите /reg");
            }
            switch (human.cmd.TypeCom)
            {

                case TypeComad.Registretion:
                    await registretionUser(client, update);
                    break;
                case TypeComad.AddIngredient:
                    if (human.Role > RolePerson.User)
                    {
                        await addIngredient(client, update);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                        text: $"Недостаточно прав для данной команды");
                        _persons[human.Id].SetTypeComad(TypeComad.None);
                    }
                    break;
                case TypeComad.GetIngredients:
                    await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                        text: $"Все типы ингридиентов:\n{GetAllTypes()}");
                    await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                       text: $"Все типы приготовления:\n{GetAllCooking()}");
                    _persons[human.Id].SetTypeComad(TypeComad.None);

                    break;
                case TypeComad.AddDish:
                    if (human.Role > RolePerson.User)
                    {
                        await addDish(client, update);
                    }
                    else 
                    {
                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                        text: $"Недостаточно прав для данной команды");
                        _persons[human.Id].SetTypeComad(TypeComad.None);
                    }
                    break;
                case TypeComad.Agree or TypeComad.DisAgree:
                    if (human.hasEvent())
                    {
                        human.Invoke();
                        break;
                    }
                    
                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                       text: $"У вас нет команд требующих вашего одобрения");
                        _persons[human.Id].SetTypeComad(TypeComad.None);
                    
                    break;
                case TypeComad.Help:
                    await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                       text: $"Я знаю следующие команды:\n" +
                             $"/reg - зарегистрироваться\n" +
                             $"/addIngredient - добавить в базу новый ингридиент\n" +
                             $"/addDish - добавить в базу новое блюдо\n" +
                             $"/GetIngredients - Получить список всех типов ингридиентов и степени готовности \n" +
                             $"/GetBase - получить список всех ингридиентов и блюд внесенных в базу");
                    _persons[human.Id].SetTypeComad(TypeComad.None);
                    break;
                case TypeComad.GetBase:

                    await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                        text:"Список ингридиентов:\n" + string.Join("\n", _ingredients.Keys));
                    await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                        text: "Список блюд:\n" + string.Join("\n", _dish.Keys));
                        _persons[human.Id].SetTypeComad(TypeComad.None);
                    break;
                default:
                    await client.SendTextMessageAsync(chatId: human.Id,
                                 text: $"Я не понял команду");
                    break;
            }



        }


        public async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken ct)
        {
            Console.WriteLine("Ошибка  - " + exception.Message);
            Console.WriteLine(exception.Source);
            Console.WriteLine(exception.HelpLink);
        }

        async Task registretionUser(ITelegramBotClient client, Update update)
        {
            long id = update.Message!.Chat.Id;
            Comand cmd = _persons[id].cmd;
            switch (cmd.StateCom)
            {
                case StateComad.New:
                    {
                        if (_persons[id].Role != RolePerson.Guest) 
                        {
                            await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                            text: $"Ты уже зарегистрирован");
                            _persons[id].cmd = new  Comand("");
                            return;
                        }
                        _persons[id].SetStateComad(StateComad.Wait);
                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                            text: $" Рад, что ты решил зарегистривроваться, твоя заявка ожидает подтверждение администратора");


                        _persons[id].SetStateComad(StateComad.Wait);

                        if (RegistrationForms.TryAdd(update.Message!.Chat.Id, _persons[id]))
                        {
                            foreach (Person person in _persons.Values)
                            {
                                if (person.Role == RolePerson.Administrator)
                                {
                                    await client.SendTextMessageAsync(chatId: person.Id,
                            text: $"Пользователь {_persons[id].Name} хочет зарегистрироваться, для его добавления введи /y {id} роль( от -1 до 3), для отказа - /n {id} ");
                                    if (!person.hasEvent())
                                    {
                                        _persons[person.Id].Subscribe(AddUser);
                                    }
                                    else 
                                    {
                                        var flag = false;
                                        foreach (var metod in person.ChangeTypeComand.GetInvocationList()) 
                                        {
                                            Console.WriteLine($"{metod}");
                                            if (metod == AddUser) 
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }
                                        if (!flag) 
                                        {
                                            _persons[person.Id].Subscribe(AddUser);
                                        }
                                    }
                                   
                                    
                                }
                            }
                        };
                        break;
                    }
                case StateComad.Wait:
                    {
                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                             text: $" Твоя заявка ожидает подтверждения администратора");

                        break;
                    }
                    

                default:
                    break;
            }
        }
        
        async Task addIngredient(ITelegramBotClient client, Update update)
        {
            long id = update.Message!.Chat.Id;
            var comand = _persons[id].cmd;
            string[] arg = update.Message.Text.Split(" ");
            switch (comand.StateCom)
            {
                case StateComad.New:
                    {
                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                        text: $"Перечисли через пробел основные характеристики ингридиента: имя, каллорийность , тип ");
                        _persons[id].SetStateComad(StateComad.Getinfo);
                        break;
                    }
                case StateComad.Getinfo:
                    {
                        if (arg.Length == 3)
                        {
                            try
                            {
                                string name = arg[0];
                                float callvalueCalories = float.Parse(arg[1]);
                                TypeIngredient type =StringToTypes(arg[2]);
                                CookingStage Stage = CookingStage.UnKnow;
                                if (Stage != CookingStage.Eror && type != TypeIngredient.Eror)
                                {

                                    _ingredients.TryAdd(name, new Ingredient(name, callvalueCalories, Stage, type));

                                    DataBase.SaveIngredientInfo(_ingredients.Values.ToArray());

                                    await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                                text: $"Ингридиент добавлен");

                                    await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                                text: $"Сейчас в базе следующие ингридиенты:");
                                    foreach (string var in _ingredients.Keys) 
                                    {
                                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                                text: $"{var}");
                                    }
                                }
                                else
                                {
                                    if (type == TypeIngredient.Eror)
                                    {
                                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                                text: $"Неправильно введен тип ингридиента");
                                    }
                                    if (Stage == CookingStage.Eror)
                                    {
                                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                                text: $"Неправильно введена степень готовности ингридиента");
                                    }

                                }


                            }
                            catch
                            {
                                await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                            text: $"Ты ошибся в аргументах");
                            }
                            
                        }
                        else
                        {
                            await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                            text: $"Ты ошибся в количестве аргументов, их должно быть 4");
                        }
                        _persons[id].cmd = new Comand("");
                        break;
                    }
            }

        }

        async Task addDish(ITelegramBotClient client, Update update)
        {
            long id = update.Message!.Chat.Id;
            var comand = _persons[id].cmd;
            string[] arg = update.Message.Text.Split(" ");

            switch (comand.StateCom)
            {
                case StateComad.New:
                    {
                        await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                        text: $"Перечисли через пробел имя рецепты и ссылку на него, затем введите ингридиенты ");
                        _persons[id].SetStateComad(StateComad.Getinfo);
                        break;
                    }
                case StateComad.Getinfo:
                    {
                        string tmp = "";
                        try
                        {
                          
                            string name = arg[0];
                            string pathDish = arg[1];
                            var tmpResults = new List<string>();

                                foreach (string ing in arg[2..])
                                {
                                    tmp = ing;
                                    tmpResults.Add(_ingredients[ing].Names);
                                }
                                Dish newDish = new Dish(name, pathDish, tmpResults.ToArray());
                                _dish.TryAdd(name, newDish);
         

                           
                            DataBase.SaveDishInfo(_dish.Values.ToArray());
                            await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                        text: $"Новое блюдо под названием {name} было добавлено");
                            Console.WriteLine(_persons.Values.Count);

                            try 
                            {
                                foreach(long i in _persons.Keys)
                                {
                                    _persons[i].AddDish(name);
                                    Console.WriteLine(_persons.Values.Count);

                                }
                                DataBase.SavePersonInfo(_persons.Values.ToArray());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("или тут");
                            }

                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                            await client.SendTextMessageAsync(chatId: update.Message!.Chat.Id,
                                        text: $"Ты ошибся в аргументах, ингредиента {tmp} нет в базе данных, можете его добавить с помощью команды /addIngredient");
                        }

                    }
                        _persons[id].cmd = new Comand("");
                        break;
                    }
            }

        public static TypeIngredient StringToTypes(string value)
        {

            try
            {
                return _TypeIngredients[value];
            }
            catch
            {
                return TypeIngredient.Eror;
            }


        }
        public static string GetAllTypes()
        {
            string result = "";
            foreach (string name in _TypeIngredients.Keys)
            {
                result += name + "\n";
            }
            return result;

        }

        public static CookingStage StringToCooking(string value)
        {

            try
            {
                return _CookingStage[value];
            }
            catch
            {
                return CookingStage.Eror;
            }


        }
        public static string GetAllCooking()
        {
            string result = "";
            foreach (string name in _CookingStage.Keys)
            {
                result += name + "\n";
            }
            return result;

        }

        public void AddUser(TypeComad type,string[] messeg,long idAdmin) 
        {
            long idUser ;
            int Role  = -1;
            try
            {
                idUser = long.Parse(messeg[0]);
                if (!RegistrationForms.TryGetValue(idUser, out Person a))
                {
                    GlobalClient.SendTextMessageAsync(chatId: idAdmin, text: $"Данный пользователь не посылал запрос на регистрацию");
                    _persons[idAdmin].SetTypeComad(TypeComad.None);
                    return;
                }

                Console.WriteLine(_persons[idUser].cmd.TypeCom);
                Console.WriteLine(idUser);
                if (_persons[idUser].cmd.TypeCom != TypeComad.Registretion) 
                {
                    GlobalClient.SendTextMessageAsync(chatId: idAdmin, text: $"Данный пользователь не посылал запрос на регистрацию");
                    _persons[idAdmin].SetTypeComad(TypeComad.None);
                    return;
                }


                _persons[idUser].SetTypeComad(TypeComad.None);
                Dictionary<string, int> tmp = new Dictionary<string, int>() { };
                _persons[idUser].SetPriorities(tmp);

                foreach (string dish in _dish.Keys)
                {
                    tmp.Add(dish, 0);
                }

                if (type == TypeComad.Agree)
                {
                    Role = int.Parse(messeg[1]);
                    GlobalClient.SendTextMessageAsync(chatId: idUser, text: $" Твоя заявка была подтверждена администратором");

                }
                else
                {
                    Console.WriteLine("А я сюда зашел");
                    GlobalClient.SendTextMessageAsync(chatId: idUser, text: $" Твоя заявка была отклонена администратором");
                }

                _persons[idUser].SetRole((RolePerson)Role);
                DataBase.SavePersonInfo(_persons.Values.ToArray());

                RegistrationForms.Remove(_persons[idUser].Id, out Person f);

                if (!(RegistrationForms.Count > 0)) 
                {
                    Console.WriteLine("я удалил функцию");
                    _persons[idAdmin].Unsubscribe(AddUser);
                }
                _persons[idAdmin].SetTypeComad(TypeComad.None);

            }
            catch 
            {
                GlobalClient.SendTextMessageAsync(chatId: idAdmin, text: $"Ты ошибся в аргументах, они должны быть числами");
                _persons[idAdmin].cmd = new Comand("");
                Console.WriteLine("Бот не пишет");
            }
            
        }
    }
}
