using System;
using System.Linq.Expressions;

namespace ScraperLib.DomainModels
{
    public class Quality
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }
        public int  MarkerId { get; set; }

        public static Expression<Func<Models.Quality, Quality>> Select => x => new Quality()
        {
            Id = x.Id,
            Value = x.Value,
            Date = x.Date,
            MarkerId = x.MarkerId
        };
    }
}
