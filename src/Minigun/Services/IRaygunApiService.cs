﻿using Minigun.Models;

namespace Minigun.Services;

public interface IRaygunApiService
{
    Task<List<Application>> ListApplicationsAsync(int? count = null, int? offset = null, string[]? orderby = null);

    Task<List<ErrorGroup>> ListErrorGroupsAsync(string applicationId, int? count = null, int? offset = null,
        string[]? orderby = null);

    Task<List<TimeseriesData>> GetErrorTimeseriesAsync(string applicationId, DateTime start, DateTime end,
        List<string>? errorGroupIds = null);

    Task<List<HistogramData>> GetRumHistogramAsync(string applicationId, DateTime start, DateTime end, string[] metrics,
        string? filter = null);
}