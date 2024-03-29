﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ScraperLib.DomainModels;

namespace ScraperLib.DomainServices.Interfaces
{
    public interface IMarkerService
    {
        Task<IEnumerable<Marker>> ScrapeAndSaveMarkersAsync(string url);
        Task<IEnumerable<Marker>> GetMarkersAsync(int? skip = null, int? take = null);
        Task<Marker> GetMarkerByIdAsync(int id);
    }
}
