using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.Models;

//This is an example model used to communicate with the SQL Database. All models must be defined in DAL/Data/AmarathContext.cs
namespace Amarath.DAL.Data
{
    public interface ICharacterService
    {
        IEnumerable<Character> GetAllCharacter();
        Character GetCharacterByID(int CharacterId);
        Character Add(Character character);
        Character Update(Character character);
        Character Delete(int CharacterId);
        void Save();
    }
    public class CharacterService : ICharacterService
    {
        private AmarathContext context;
        
        public CharacterService(AmarathContext context)
        {
            this.context = context;
        }
        public Character Add(Character character)
        {
            context.Characters.Add(character);
            context.SaveChanges();
            return character;
        }

        public Character Delete(int CharacterId)
        {
            Character character = context.Characters.Find(CharacterId);
            if(character != null)
            {
                context.Characters.Remove(character);
                context.SaveChanges();
            }
            return character;
        }

        public IEnumerable<Character> GetAllCharacter()
        {
            return context.Characters.ToList();
        }

        public Character GetCharacterByID(int CharacterId)
        {
            return context.Characters.Find(CharacterId);
        }

        public Character Update(Character characterChanges)
        {
            var character = context.Characters.Attach(characterChanges);
            character.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return characterChanges;
        }
        public void Save()
        {
            throw new NotImplementedException(); //TODO: Implement
        }
    }
}