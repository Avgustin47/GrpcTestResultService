using System;
using System.Collections.Generic;
using System.Text;

namespace UserMqDto
{
    public interface ITokenStorage
    {
        Task SaveTokenAsync(string token, int reportId, TimeSpan lifetime);
        Task<(bool Exists, int ReportId)> TryGetReportIdAsync(string token);
        Task RemoveTokenAsync(string token);
    }
}
