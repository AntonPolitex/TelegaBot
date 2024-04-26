using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TelegaBot
{
   
    public enum CookingStage
    {
        Eror =-1,
        UnKnow = 0,
        Raw = 1,
        Boiled = 2,
        Fried = 3,
        Steamed = 4
    }
    public enum TypeIngredient
    {
        Eror = -1,
        Unknown = 0,
        Meat = 1,
        Fish = 2,
        Eggs = 3,
        MilkProducts = 4,
        Bread = 5,
        Cereals = 6,
        Pasta = 7,
        Legumes = 8,
        Vegetables = 9,
        Fruits = 10,
        Berries = 11,
        Nuts = 12,
        Mushrooms = 13,
        Confectionery = 14,
        EdibleFats = 15,
        Beverages = 16
    }
    public class Ingredient
    {
        
        public string Names { get; private set; }
        public float ValueCalories { get; private set; }
        public CookingStage Stage { get; private set; }
        public TypeIngredient Type { get; private set; }

        public Ingredient(string names, float valueCalories, CookingStage stage, TypeIngredient type)
        {
            Names = names;
            ValueCalories = valueCalories;
            Stage = stage;
            this.Type = type;
        }

    }
}
