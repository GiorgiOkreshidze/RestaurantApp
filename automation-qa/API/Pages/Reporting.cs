using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ApiTests.Utilities;
using automation_qa.Framework;

namespace ApiTests.Pages
{
    public class Reporting : BasePage
    {
        private readonly CurlHelper _curlHelper;
        private readonly string _baseUrl;

        public Reporting(string baseUrl = null)
        {
            _baseUrl = baseUrl ?? BaseConfiguration.ApiBaseUrl;
            _curlHelper = new CurlHelper("curl");
        }

        /// <summary>
        /// Отправляет email с отчетом в Excel формате
        /// </summary>
        public (HttpStatusCode StatusCode, JObject? ResponseBody) SendReportEmailWithCurl(string? token = null)
        {
            string url = $"{_baseUrl}/reports/send";

            HttpStatusCode statusCode;
            JObject? responseBody;

            if (token == null)
            {
                (statusCode, responseBody) = _curlHelper.ExecutePostRequestForObject(url, "{}");
            }
            else
            {
                (statusCode, responseBody) = _curlHelper.ExecutePostRequestWithAuthForObject(url, "{}", token);
            }

            Console.WriteLine($"SendReportEmailWithCurl response status: {statusCode}");
            Console.WriteLine($"SendReportEmailWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Отправляет email с отчетом в Excel формате
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> SendReportEmail(string? token = null)
        {
            return await Task.Run(() => SendReportEmailWithCurl(token));
        }

        /// <summary>
        /// Получает данные отчета на основе предоставленных фильтров
        /// </summary>
        public (HttpStatusCode StatusCode, JObject? ResponseBody) GetReportDataWithCurl(
            string? startDate = null,
            string? endDate = null,
            string? locationId = null,
            string? token = null)
        {
            var queryParams = new NameValueCollection();
            if (!string.IsNullOrEmpty(startDate)) queryParams.Add("StartDate", startDate);
            if (!string.IsNullOrEmpty(endDate)) queryParams.Add("EndDate", endDate);
            if (!string.IsNullOrEmpty(locationId)) queryParams.Add("LocationId", locationId);

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/reports{(string.IsNullOrEmpty(queryString) ? "" : $"?{queryString}")}";

            HttpStatusCode statusCode;
            JObject? responseBody;

            if (token == null)
            {
                (statusCode, responseBody) = _curlHelper.ExecuteGetRequestForObject(url);
            }
            else
            {
                (statusCode, responseBody) = _curlHelper.ExecuteGetRequestWithAuthForObject(url, token);
            }

            Console.WriteLine($"GetReportDataWithCurl response status: {statusCode}");
            Console.WriteLine($"GetReportDataWithCurl response content: {responseBody?.ToString() ?? "No content"}");

            return (statusCode, responseBody);
        }

        /// <summary>
        /// Получает данные отчета на основе предоставленных фильтров
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, JObject? ResponseBody)> GetReportData(
            string? startDate = null,
            string? endDate = null,
            string? locationId = null,
            string? token = null)
        {
            return await Task.Run(() => GetReportDataWithCurl(startDate, endDate, locationId, token));
        }

        /// <summary>
        /// Скачивает отчет в указанном формате на основе предоставленных фильтров
        /// </summary>
        public (HttpStatusCode StatusCode, byte[]? ResponseContent) DownloadReportWithCurl(
            string format,
            string? startDate = null,
            string? endDate = null,
            string? locationId = null,
            string? token = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException(nameof(format), "Формат отчета должен быть указан");
            }

            var queryParams = new NameValueCollection();
            queryParams.Add("Format", format);
            if (!string.IsNullOrEmpty(startDate)) queryParams.Add("StartDate", startDate);
            if (!string.IsNullOrEmpty(endDate)) queryParams.Add("EndDate", endDate);
            if (!string.IsNullOrEmpty(locationId)) queryParams.Add("LocationId", locationId);

            string queryString = BuildQueryString(queryParams);
            string url = $"{_baseUrl}/reports/downloads?{queryString}";

            Console.WriteLine($"Executing curl GET request to: {url}");

            string tempHeadersFile = null;
            if (!string.IsNullOrEmpty(token))
            {
                tempHeadersFile = Path.GetTempFileName();
                using (StreamWriter writer = new StreamWriter(tempHeadersFile))
                {
                    writer.WriteLine($"Authorization: Bearer {token}");
                }
            }

            try
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "cmd.exe";
                var arguments = $"/c {_curlHelper.GetCurlPath()} -k -s -o - -w \"%{{http_code}}\"";
                if (!string.IsNullOrEmpty(token))
                {
                    arguments += $" -H @{tempHeadersFile}";
                }
                arguments += $" \"{url}\"";
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                byte[] output = new byte[0];
                using (var memoryStream = new MemoryStream())
                {
                    // Читаем вывод как бинарные данные
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                    }
                    output = memoryStream.ToArray();
                }
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                // Последние 3 символа - код статуса
                if (output.Length >= 3)
                {
                    // Отделяем код статуса от содержимого файла
                    string statusCodeStr = System.Text.Encoding.UTF8.GetString(output.AsSpan(output.Length - 3, 3));
                    byte[] fileContent = new byte[output.Length - 3];
                    Array.Copy(output, 0, fileContent, 0, output.Length - 3);

                    HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                    if (int.TryParse(statusCodeStr, out int statusCodeInt))
                    {
                        if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                        {
                            statusCode = (HttpStatusCode)statusCodeInt;
                        }
                    }

                    Console.WriteLine($"DownloadReportWithCurl response status: {statusCode}");
                    Console.WriteLine($"DownloadReportWithCurl response content length: {fileContent.Length} bytes");

                    return (statusCode, fileContent);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output, length: {output.Length}");
                    return (HttpStatusCode.InternalServerError, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing curl: {ex.Message}");
                return (HttpStatusCode.InternalServerError, null);
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempHeadersFile) && File.Exists(tempHeadersFile))
                {
                    File.Delete(tempHeadersFile);
                }
            }
        }

        /// <summary>
        /// Скачивает отчет в указанном формате на основе предоставленных фильтров
        /// </summary>
        public async Task<(HttpStatusCode StatusCode, byte[]? ResponseContent)> DownloadReport(
            string format,
            string? startDate = null,
            string? endDate = null,
            string? locationId = null,
            string? token = null)
        {
            return await Task.Run(() => DownloadReportWithCurl(format, startDate, endDate, locationId, token));
        }

        private string BuildQueryString(NameValueCollection queryParams)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (string key in queryParams)
            {
                query[key] = queryParams[key];
            }
            return query.ToString();
        }
    }
}
