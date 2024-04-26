using System;

namespace TelegaBot
{
    public enum StateComad
    {
        None = 0,
        New = 1, // Команда поставленна
        Getinfo = 2, // получена информация о команде
        Wait = 3, // Ожидание подтверждения
        Successful = 4, // удачное исполнение команды
        Failed = 5 // Неудачное исполнение команды
    }
    public enum TypeComad
    {
        None = 0,
        Registretion = 1,
        Addperson = 2,
        AddIngredient = 3,
        AddDish = 4,
        GetIngredients = 5,
        Agree = 6,
        DisAgree =7,
        Help = 8,
        GetBase = 9
    }
    public class Comand
    {


        readonly string[] nameComands = { "", "/reg", "/addUser", "/addIngredient","/addDish","/GetIngredients", "/y", "/n","/help", "/GetBase" };
        public string StrComad = "";
        public string[] Args = new string[0];
        public TypeComad TypeCom;
        public StateComad StateCom;
        public Comand(string comandLine, TypeComad typeCom = TypeComad.None, StateComad stateCom = StateComad.None)
        {
            string[] tmp = comandLine.Split(" ");
            StrComad = tmp[0];
            if (comandLine != "")
            {
                for (int i = 0; i < nameComands.Length; i++)
                {
                    if (tmp[0] == nameComands[i])
                    {
                        TypeCom = (TypeComad)i;
                        StateCom = StateComad.New;
                        break;
                    }
                }

            }
            else
            {
                TypeCom = TypeComad.None;
            }

            Args = tmp[1..];

        }

    }
}
