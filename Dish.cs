using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TelegaBot
{
    public enum PriorityMealTimes
    {
        Eror =-1,
        None = 0,
        Morning =1,
        Dinner = 2,
        SecondDiner =3
    }
    public class Dish
    {
        public string Name { get; private set; }

        public string[] Ingridients { get; private set; }
        public string LinkToRecipe { get; private set; }
        public Dish(string name,string linkToRecipe, string[] ingridients)
        {
            Name = name;
            LinkToRecipe = linkToRecipe;
            Ingridients = ingridients;

        }
    }
}
