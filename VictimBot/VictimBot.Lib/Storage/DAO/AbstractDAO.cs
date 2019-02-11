using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VictimBot.Lib.Storage.DAO
{
    public abstract class AbstractDAO<ItemType> where ItemType : class, IStoreItem
    {
        protected IStorage storage;

        protected AbstractDAO(IStorage storage)
        {
            this.storage = storage;
        }

        public abstract string GenerateKey(ItemType item);

        public abstract ItemType CreateNew(string key = null);

        public async Task<ItemType> ReadOrCreateAsync(string key)
        {
            var item = await ReadAsync(key);
            return item ?? CreateNew(key);
        }

        public async Task<ItemType> ReadAsync(string key)
        {
            var pair = await storage.ReadAsync(new[] { key });
            return (ItemType)pair.FirstOrDefault().Value;
        }

        public async Task WriteAsync(ItemType item)
        {
            var data = new Dictionary<string, object>() { { GenerateKey(item), item } };
            await storage.WriteAsync(data);
        }

    }
}
