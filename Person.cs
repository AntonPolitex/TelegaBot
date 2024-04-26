using System.Collections.Concurrent;
using Telegram.Bot;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TelegaBot
{
    public enum RolePerson
    {
        Ban =-1,
        Guest = 0,
        User = 1,
        Moderator = 2,
        Administrator = 3
    }



    public class Person
    {
        public Action<TypeComad, string[],long> ChangeTypeComand; // тип команды, айди тому чью команду одобряем, сообщение к команде, айди отправившего 
        public string Name { get; private set; }
        public long Id { get; private set; }
        public Dictionary<string,int> Priorities { get; private set; }
        public RolePerson Role { get; private set; }
 
        private Comand _cmd;
        [JsonIgnore]
        public Comand cmd {
            get 
            {
                return _cmd;
            } 
            set 
            {
                _cmd = value;
            } 
        }
        // не забудь добавить про продукты, что предпочитает есть, что нет, и список продуктов которые он уже ел.



        public Person(string name, long id, Dictionary<string, int> priorities, RolePerson role = 0 )
        {
            Name = name;
            Id = id;
            Role = role;
            _cmd = new Comand("", 0, 0);
            Priorities = priorities;
        }
        public void SetRole(RolePerson value)
        {
            Role = value;
        }
        public void SetStateComad(StateComad value)
        {
            cmd.StateCom = value;
        }
        public void SetTypeComad(TypeComad value)
        {
            cmd.TypeCom = value;
        }
        public void SetPriorities(Dictionary<string,int> value)
        {
            Priorities = value;
        }
        public void AddDish(string value)
        {
            Priorities.Add(value,0);
        }
        
        public void Subscribe(Action<TypeComad, string[],long> fun)
        {
            ChangeTypeComand += fun;
        }
        public void Unsubscribe(Action<TypeComad, string[],long> fun)
        {
            if (ChangeTypeComand != null)
            {
                ChangeTypeComand -= fun;
            }
           
        }
        public void Invoke() 
        {
            if (ChangeTypeComand != null)
            {
                Console.WriteLine("я и сюда зашел");
                ChangeTypeComand.Invoke(_cmd.TypeCom, _cmd.Args, Id);
            }
        }
        public bool hasEvent() 
        {
            return ChangeTypeComand != null;
        }

    }
}
