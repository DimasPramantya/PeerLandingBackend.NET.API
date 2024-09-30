using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResPagination<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string? OrderBy { get; set; }
        public string? OrderDirection { get; set; }
        public int TotalRecords { get; set; }
        public string NextUrl { get; set; } = "";
        public string PreviousUrl { get; set; } = "";
        public T Records { get; set; }
        public void SetUrls(string baseUrl, object additionalQuery = null)
        {
            if (PageNumber > 1)
            {
                PreviousUrl = $"{baseUrl}?PageNumber={PageNumber - 1}&PageSize={PageSize}";
            }

            if (TotalRecords > PageNumber * PageSize)
            {
                NextUrl = $"{baseUrl}?PageNumber={PageNumber + 1}&PageSize={PageSize}";
                if (additionalQuery != null)
                {
                    var additionalParams = GetQueryString(additionalQuery);
                    if (!string.IsNullOrEmpty(additionalParams))
                    {
                        if (!string.IsNullOrEmpty(PreviousUrl))
                        {
                            PreviousUrl += "&" + additionalParams;
                        }

                        if (!string.IsNullOrEmpty(NextUrl))
                        {
                            NextUrl += "&" + additionalParams;
                        }
                    }
                }
            }
        }
        private string GetQueryString(object additionalQuery)
        {
            var properties = additionalQuery.GetType().GetProperties();
            var queryParams = new List<string>();

            foreach (var property in properties)
            {
                var value = property.GetValue(additionalQuery);
                if (value != null)
                {
                    string paramName = Uri.EscapeDataString(property.Name);
                    string paramValue = Uri.EscapeDataString(value.ToString());
                    queryParams.Add($"{paramName}={paramValue}");
                }
            }

            return string.Join("&", queryParams);
        }
    }
}
