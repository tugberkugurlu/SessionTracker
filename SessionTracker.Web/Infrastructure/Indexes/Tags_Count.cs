using Raven.Client.Indexes;
using SessionTracker.Web.Entities;
using System;
using System.Linq;

namespace SessionTracker.Web.Infrastructure.Indexes
{
    public class Tags_Count : AbstractIndexCreationTask<Session, Tags_Count.ReduceResult>
    {
        public class ReduceResult
		{
			public string Name { get; set; }
			public int Count { get; set; }
			public DateTimeOffset LastSeenAt { get; set; }
		}

        public Tags_Count()
        {
            Map = sessions => from session in sessions
                              from tag in session.Tags
                              select new
                              {
                                  Name = tag.ToLowerInvariant(), 
                                  Count = 1, 
                                  LastSeenAt = session.CreatedOn
                              };

            Reduce = results => from tagCount in results
                                group tagCount by tagCount.Name
                                into groupedResult
                                select new
                                {
                                    Name = groupedResult.Key,
                                    Count = groupedResult.Sum(x => x.Count), 
                                    LastSeenAt = groupedResult.Max(x => x.LastSeenAt)
                                };
        }
    }
}