using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;

namespace MVP.Services
{
    public class SuggestionService
    {
        const string typeKey = "type-suggestions";
        const string techKey = "tech-suggestions";

        public async static Task<List<Guid>> GetContributionTypeSuggestions()
        {
            try
            {
                var items = await BlobCache.LocalMachine.GetObject<List<Guid>>(typeKey);

                return items.GroupBy(x => x)
                      .OrderByDescending(g => g.Count())
                      .SelectMany(g => g)
                      .Distinct()
                      .Take(3)
                      .ToList();
            }
            catch (KeyNotFoundException)
            {
                return new List<Guid>();
            }
        }

        public async static Task<List<Guid>> GetContributionTechnologySuggestions()
        {
            try
            {
                var items = await BlobCache.LocalMachine.GetObject<List<Guid>>(techKey);

                return items.GroupBy(x => x)
                      .OrderByDescending(g => g.Count())
                      .SelectMany(g => g)
                      .Distinct()
                      .Take(3)
                      .ToList();
            }
            catch (KeyNotFoundException)
            {
                return new List<Guid>();
            }
        }

        public async static Task StoreContributionTypeSuggestionAsync(Guid id)
        {
            try
            {
                var items = await BlobCache.LocalMachine.GetObject<List<Guid>>(typeKey);
                items.Add(id);
                await BlobCache.LocalMachine.InsertObject(typeKey, items);
            }
            catch (KeyNotFoundException)
            {
                var items = new List<Guid> { id };
                await BlobCache.LocalMachine.InsertObject(typeKey, items);
            }
        }

        public static async Task StoreContributionTechnologySuggestionAsync(Guid id)
        {
            try
            {
                var items = await BlobCache.LocalMachine.GetObject<List<Guid>>(techKey);
                items.Add(id);
                await BlobCache.LocalMachine.InsertObject(techKey, items);
            }
            catch (KeyNotFoundException)
            {
                var items = new List<Guid> { id };
                await BlobCache.LocalMachine.InsertObject(techKey, items);
            }
        }

        public async static Task StoreContributionTechnologySuggestionsAsync(List<Guid> ids)
        {
            try
            {
                var items = await BlobCache.LocalMachine.GetObject<List<Guid>>(techKey);
                items.AddRange(ids);
                await BlobCache.LocalMachine.InsertObject(techKey, items);
            }
            catch (KeyNotFoundException)
            {
                var items = new List<Guid>();
                items.AddRange(ids);
                await BlobCache.LocalMachine.InsertObject(techKey, items);
            }
        }
    }
}
