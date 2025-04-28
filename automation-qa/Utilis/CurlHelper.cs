using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace ApiTests.Utilities
{
    public class CurlHelper
    {
        private readonly string _curlPath;

        public CurlHelper(string curlPath = "curl")
        {
            _curlPath = curlPath;
        }

        public string GetCurlPath()
        {
            return _curlPath;
        }

        public (HttpStatusCode statusCode, JArray? responseBodyArray) ExecuteGetRequestForArray(string url)
        {
            var (statusCodeStr, responseBody) = ExecuteGetRequest(url);

            HttpStatusCode statusCode = ParseStatusCode(statusCodeStr);
            JArray? responseBodyArray = null;

            if (!string.IsNullOrEmpty(responseBody))
            {
                try
                {
                    responseBodyArray = JArray.Parse(responseBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response as array: {ex.Message}");
                }
            }

            return (statusCode, responseBodyArray);
        }

        public (HttpStatusCode statusCode, JObject? responseBodyObject) ExecuteGetRequestForObject(string url)
        {
            var (statusCodeStr, responseBody) = ExecuteGetRequest(url);

            HttpStatusCode statusCode = ParseStatusCode(statusCodeStr);
            JObject? responseBodyObject = null;

            if (!string.IsNullOrEmpty(responseBody))
            {
                try
                {
                    responseBodyObject = JObject.Parse(responseBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response as object: {ex.Message}");
                }
            }

            return (statusCode, responseBodyObject);
        }

        public (string statusCode, string responseBody) ExecuteGetRequest(string url)
        {
            Console.WriteLine($"Executing curl GET request to: {url}");

            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {_curlPath} -k -s -o - -w \"%{{http_code}}\" \"{url}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            try
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                if (output.Length >= 3)
                {
                    string statusCode = output.Substring(output.Length - 3);
                    string responseBody = output.Substring(0, output.Length - 3).Trim();

                    Console.WriteLine($"Curl response status: {statusCode}");
                    if (responseBody.Length > 100)
                    {
                        Console.WriteLine($"Curl response body (first 100 chars): {responseBody.Substring(0, 100)}...");
                    }
                    else
                    {
                        Console.WriteLine($"Curl response body: {responseBody}");
                    }

                    return (statusCode, responseBody);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
                    return ("000", output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing curl: {ex.Message}");
                return ("500", $"{{\"error\": \"{ex.Message}\"}}");
            }
        }

        public (string statusCode, string responseBody) ExecuteGetRequestWithParams(string url, string queryParams)
        {
            return ExecuteGetRequest($"{url}?{queryParams}");
        }

        public (HttpStatusCode statusCode, JObject? responseBodyObject) ExecuteGetRequestWithAuthForObject(
            string url, string idToken, string accessToken = null)
        {
            string tempHeadersFile = Path.GetTempFileName();
            try
            {
                using (StreamWriter writer = new StreamWriter(tempHeadersFile))
                {
                    writer.WriteLine($"Authorization: Bearer {idToken}");
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        writer.WriteLine($"X-Amz-Security-Token: {accessToken}");
                    }
                    writer.WriteLine($"Date: {DateTime.UtcNow.ToString("r")}");
                }

                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {_curlPath} -k -s -o - -w \"%{{http_code}}\" -H @{tempHeadersFile} \"{url}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
                JObject? responseBodyObject = null;

                // The last 3 characters should be the status code
                if (output.Length >= 3)
                {
                    string statusCodeStr = output.Substring(output.Length - 3);
                    string responseBody = output.Substring(0, output.Length - 3).Trim();

                    statusCode = ParseStatusCode(statusCodeStr);

                    Console.WriteLine($"Curl response status: {statusCodeStr}");
                    if (responseBody.Length > 100)
                    {
                        Console.WriteLine($"Curl response body (first 100 chars): {responseBody.Substring(0, 100)}...");
                    }
                    else
                    {
                        Console.WriteLine($"Curl response body: {responseBody}");
                    }

                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        try
                        {
                            responseBodyObject = JObject.Parse(responseBody);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing response as object: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
                }

                return (statusCode, responseBodyObject);
            }
            finally
            {
                if (File.Exists(tempHeadersFile))
                {
                    File.Delete(tempHeadersFile);
                }
            }
        }

        public (HttpStatusCode statusCode, JObject? responseBodyObject) ExecutePostRequestForObject(string url, string jsonBody)
        {
            var (statusCodeStr, responseBody) = ExecutePostRequest(url, jsonBody);

            HttpStatusCode statusCode = ParseStatusCode(statusCodeStr);
            JObject? responseBodyObject = null;

            if (!string.IsNullOrEmpty(responseBody))
            {
                try
                {
                    responseBodyObject = JObject.Parse(responseBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response as object: {ex.Message}");
                }
            }

            return (statusCode, responseBodyObject);
        }

        public (string statusCode, string responseBody) ExecutePostRequest(string url, string jsonBody)
        {
            Console.WriteLine($"Executing curl POST request to: {url}");
            Console.WriteLine($"Request body: {jsonBody}");

            string tempJsonFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempJsonFile, jsonBody);

                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {_curlPath} -k -s -o - -w \"%{{http_code}}\" -X POST -H \"Content-Type: application/json\" -d @{tempJsonFile} \"{url}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                // The last 3 characters should be the status code
                if (output.Length >= 3)
                {
                    string statusCode = output.Substring(output.Length - 3);
                    string responseBody = output.Substring(0, output.Length - 3).Trim();

                    Console.WriteLine($"Curl response status: {statusCode}");
                    if (responseBody.Length > 100)
                    {
                        Console.WriteLine($"Curl response body (first 100 chars): {responseBody.Substring(0, 100)}...");
                    }
                    else
                    {
                        Console.WriteLine($"Curl response body: {responseBody}");
                    }

                    return (statusCode, responseBody);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
                    return ("000", output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing curl: {ex.Message}");
                return ("500", $"{{\"error\": \"{ex.Message}\"}}");
            }
            finally
            {
                if (File.Exists(tempJsonFile))
                {
                    File.Delete(tempJsonFile);
                }
            }
        }

        public (HttpStatusCode statusCode, string responseBody) ExecutePostRequestWithAuthForString(
            string url, string jsonBody, string idToken)
        {
            Console.WriteLine($"Executing curl POST request with auth to: {url}");
            Console.WriteLine($"Request body: {jsonBody}");

            string tempJsonFile = Path.GetTempFileName();
            string tempHeadersFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempJsonFile, jsonBody);

                using (StreamWriter writer = new StreamWriter(tempHeadersFile))
                {
                    writer.WriteLine("Content-Type: application/json");
                    writer.WriteLine($"Authorization: Bearer {idToken}");
                }

                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {_curlPath} -k -s -o - -w \"%{{http_code}}\" -X POST -H @{tempHeadersFile} -d @{tempJsonFile} \"{url}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                // The last 3 characters should be the status code
                if (output.Length >= 3)
                {
                    string statusCodeStr = output.Substring(output.Length - 3);
                    string responseBody = output.Substring(0, output.Length - 3).Trim();

                    HttpStatusCode statusCode = ParseStatusCode(statusCodeStr);

                    Console.WriteLine($"Curl response status: {statusCodeStr}");
                    if (responseBody.Length > 100)
                    {
                        Console.WriteLine($"Curl response body (first 100 chars): {responseBody.Substring(0, 100)}...");
                    }
                    else
                    {
                        Console.WriteLine($"Curl response body: {responseBody}");
                    }

                    return (statusCode, responseBody);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
                    return (HttpStatusCode.InternalServerError, output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing curl: {ex.Message}");
                return (HttpStatusCode.InternalServerError, $"{{\"error\": \"{ex.Message}\"}}");
            }
            finally
            {
                if (File.Exists(tempJsonFile))
                {
                    File.Delete(tempJsonFile);
                }

                if (File.Exists(tempHeadersFile))
                {
                    File.Delete(tempHeadersFile);
                }
            }
        }

        private HttpStatusCode ParseStatusCode(string statusCodeStr)
        {
            if (int.TryParse(statusCodeStr, out int statusCodeInt))
            {
                if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                {
                    return (HttpStatusCode)statusCodeInt;
                }
            }

            Console.WriteLine($"Failed to parse status code: {statusCodeStr}");
            return HttpStatusCode.InternalServerError;
        }

        public (HttpStatusCode, JObject) ExecutePutRequestWithAuthForObject(string url, string jsonBody, string idToken)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url), "URL cannot be null or empty");
            }

            if (string.IsNullOrEmpty(idToken))
            {
                throw new ArgumentNullException(nameof(idToken), "ID token cannot be null or empty");
            }

            var (statusCodeStr, responseBodyStr) = ExecutePutRequest(url, jsonBody, idToken);

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            if (int.TryParse(statusCodeStr, out int statusCodeInt))
            {
                if (Enum.IsDefined(typeof(HttpStatusCode), statusCodeInt))
                {
                    statusCode = (HttpStatusCode)statusCodeInt;
                }
            }

            JObject responseBodyObject = null;
            if (!string.IsNullOrEmpty(responseBodyStr))
            {
                try
                {
                    responseBodyObject = JObject.Parse(responseBodyStr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing response as object: {ex.Message}");
                }
            }

            return (statusCode, responseBodyObject);
        }

        public (string statusCode, string responseBody) ExecutePutRequest(string url, string jsonBody, string authToken = null)
        {
            Console.WriteLine($"Executing curl PUT request to: {url}");
            Console.WriteLine($"Request body: {jsonBody}");

            string tempJsonFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempJsonFile, jsonBody);

                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";

                string arguments = $"/c {_curlPath} -k -s -o - -w \"%{{http_code}}\" -X PUT -H \"Content-Type: application/json\"";

                if (!string.IsNullOrEmpty(authToken))
                {
                    arguments += $" -H \"Authorization: Bearer {authToken}\"";
                }

                arguments += $" -d @{tempJsonFile} \"{url}\"";
                process.StartInfo.Arguments = arguments;

                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                // The last 3 characters should be the status code
                if (output.Length >= 3)
                {
                    string statusCode = output.Substring(output.Length - 3);
                    string responseBody = output.Substring(0, output.Length - 3).Trim();

                    Console.WriteLine($"Curl response status: {statusCode}");
                    if (responseBody.Length > 100)
                    {
                        Console.WriteLine($"Curl response body (first 100 chars): {responseBody.Substring(0, 100)}...");
                    }
                    else
                    {
                        Console.WriteLine($"Curl response body: {responseBody}");
                    }

                    return (statusCode, responseBody);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
                    return ("000", output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing curl: {ex.Message}");
                return ("500", $"{{\"error\": \"{ex.Message}\"}}");
            }
            finally
            {
                if (File.Exists(tempJsonFile))
                {
                    File.Delete(tempJsonFile);
                }
            }
        }

        public (HttpStatusCode statusCode, JObject? responseBodyObject) ExecuteDeleteRequest(string url, string idToken = null)
        {
            Console.WriteLine($"Executing curl DELETE request to: {url}");

            string tempHeadersFile = null;
            if (!string.IsNullOrEmpty(idToken))
            {
                tempHeadersFile = Path.GetTempFileName();
                using (StreamWriter writer = new StreamWriter(tempHeadersFile))
                {
                    writer.WriteLine($"Authorization: Bearer {idToken}");
                }
            }

            try
            {
                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                var arguments = $"/c {_curlPath} -k -s -o - -w \"%{{http_code}}\" -X DELETE";
                if (!string.IsNullOrEmpty(idToken))
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
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                // The last 3 characters should be the status code
                if (output.Length >= 3)
                {
                    string statusCodeStr = output.Substring(output.Length - 3);
                    string responseBody = output.Substring(0, output.Length - 3).Trim();

                    HttpStatusCode statusCode = ParseStatusCode(statusCodeStr);

                    Console.WriteLine($"Curl response status: {statusCodeStr}");
                    if (responseBody.Length > 100)
                    {
                        Console.WriteLine($"Curl response body (first 100 chars): {responseBody.Substring(0, 100)}...");
                    }
                    else
                    {
                        Console.WriteLine($"Curl response body: {responseBody}");
                    }

                    JObject? responseBodyObject = null;
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        try
                        {
                            responseBodyObject = JObject.Parse(responseBody);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing response as object: {ex.Message}");
                        }
                    }

                    return (statusCode, responseBodyObject);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
                    return (HttpStatusCode.InternalServerError, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing curl: {ex.Message}");
                return (HttpStatusCode.InternalServerError, JObject.FromObject(new { error = ex.Message }));
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempHeadersFile) && File.Exists(tempHeadersFile))
                {
                    File.Delete(tempHeadersFile);
                }
            }
        }

        public (HttpStatusCode statusCode, JObject? responseBodyObject) ExecutePostRequestWithAuthForObject(
            string url, string jsonBody, string idToken)
        {
            Console.WriteLine($"Executing curl POST request with auth to: {url}");
            Console.WriteLine($"Request body: {jsonBody}");

            string tempJsonFile = Path.GetTempFileName();
            string tempHeadersFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempJsonFile, jsonBody);

                using (StreamWriter writer = new StreamWriter(tempHeadersFile))
                {
                    writer.WriteLine("Content-Type: application/json");
                    writer.WriteLine($"Authorization: Bearer {idToken}");
                }

                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {_curlPath} -k -s -o - -w \"%{{http_code}}\" -X POST -H @{tempHeadersFile} -d @{tempJsonFile} \"{url}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Curl error: {error}");
                }

                // The last 3 characters should be the status code
                if (output.Length >= 3)
                {
                    string statusCodeStr = output.Substring(output.Length - 3);
                    string responseBody = output.Substring(0, output.Length - 3).Trim();

                    HttpStatusCode statusCode = ParseStatusCode(statusCodeStr);

                    Console.WriteLine($"Curl response status: {statusCodeStr}");
                    if (responseBody.Length > 100)
                    {
                        Console.WriteLine($"Curl response body (first 100 chars): {responseBody.Substring(0, 100)}...");
                    }
                    else
                    {
                        Console.WriteLine($"Curl response body: {responseBody}");
                    }

                    JObject? responseBodyObject = null;
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        try
                        {
                            responseBodyObject = JObject.Parse(responseBody);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing response as object: {ex.Message}");
                        }
                    }

                    return (statusCode, responseBodyObject);
                }
                else
                {
                    Console.WriteLine($"Curl produced unexpected output: {output}");
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
                if (File.Exists(tempJsonFile))
                {
                    File.Delete(tempJsonFile);
                }

                if (File.Exists(tempHeadersFile))
                {
                    File.Delete(tempHeadersFile);
                }
            }
        }
    }
}