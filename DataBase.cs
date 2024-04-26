using System.Linq;
using System.Text.Json;

namespace TelegaBot
{
    public static class DataBase
    {
        const string PathPersonInfo = "C:\\Users\\anton\\source\\repos\\TelegaBot\\Persons.json";
        const string PathIngridientInfo = "C:\\Users\\anton\\source\\repos\\TelegaBot\\Ingredients.json";
        const string PathDishInfo = "C:\\Users\\anton\\source\\repos\\TelegaBot\\Dish.json";
        const string PathBotToken = "C:\\Users\\anton\\source\\repos\\TelegaBot\\BotToken.txt";
        public static string GetToken(string path = PathBotToken) 
        {
            try 
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string text = reader.ReadToEnd();
                    if (text != "") 
                    {
                        return text;
                    }
                    throw new Exception("Файл не верно прочитался");
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message + " Проблема с чтением токена");
                return "";
            }
          

        }
       public static Person[] GetPersonInfo(string path = PathPersonInfo)
        {
            var a = GetInfoToFile<Person>(path).ToArray<Person>();
            return a;
        }
        public static void SavePersonInfo(Person[] array, string path = PathPersonInfo)
        {
            SaveInfoToFile<Person>(array.ToList<Person>(), path);
        }

        public static Ingredient[] GetIngredientInfo(string path = PathIngridientInfo)
        {
            var a = GetInfoToFile<Ingredient>(path).ToArray<Ingredient>();
            return a;
        }
        public static void SaveIngredientInfo(Ingredient[] array, string path = PathIngridientInfo)
        {
            SaveInfoToFile<Ingredient>(array.ToList<Ingredient>(), path);
        }
        public static Dish[] GetDishInfo(string path = PathDishInfo)
        {
            var a = GetInfoToFile<Dish>(path).ToArray<Dish>();
            return a;
        }
        public static void SaveDishInfo(Dish[] array, string path = PathDishInfo)
        {
            SaveInfoToFile<Dish>(array.ToList<Dish>(), path);
        }


        private static List<T> GetInfoToFile<T>(string path)
        {
            List<T> array;
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    array = JsonSerializer.Deserialize<List<T>>(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не смог открыть файл {path} для чтения");
                Console.WriteLine(ex.ToString());
                array = Enumerable.Empty<T>().ToList();
            }
            return array;
        }
        private static void SaveInfoToFile<T>(List<T> array, string path)
        {

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    JsonSerializer.Serialize<List<T>>(fs, array);
                    Console.WriteLine($"я смог записать файл {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не смог записать файл {path}");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}