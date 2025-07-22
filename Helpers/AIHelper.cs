using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;

namespace NBA.Helpers
{
    public static class AIHelper
    {
        public static async Task GetAIResponseStreaming(
            List<Dictionary<string, string>> messages,
            string apiKey,
            string apiUrl,
            RichTextBox outputBox)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = messages,
                    stream = true,
                    max_tokens = 1300,
                    temperature = 0.2
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                bool isFirstToken = true;
                
                using (var response = await client.PostAsync(apiUrl, content))
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.StartsWith("data: "))
                        {
                            var jsonData = line.Substring(6).Trim();
                            if (jsonData == "[DONE]") break;

                            var json = JObject.Parse(jsonData);
                            var token = json["choices"]?[0]?["delta"]?["content"]?.ToString();

                            if (!string.IsNullOrEmpty(token))
                            {
                                string displayToken = token.Replace("#", " ").Replace("*", " ");
                                outputBox.Invoke((MethodInvoker)delegate
                                {
                                    // 第一个token到达时，如果最后一行包含"正在思考"或"正在查找"，则删除最后一行
                                    if (isFirstToken)
                                    {
                                        string text = outputBox.Text;
                                        // 使用Contains更宽松地匹配各种可能的提示文本
                                        if (text.Contains("正在思考，请稍候") || 
                                            text.Contains("AI正在思考，请稍候") || 
                                            text.Contains("正在查找，请稍候"))
                                        {
                                            // 找到最后一个"正在"的位置作为剪切点
                                            int lastLineStart = Math.Max(
                                                Math.Max(
                                                    text.LastIndexOf("正在思考"),
                                                    text.LastIndexOf("AI正在思考")
                                                ),
                                                text.LastIndexOf("正在查找")
                                            );
                                            if (text.Contains("AI正在思考，请稍候")) { lastLineStart -= 2; }
                                            if (lastLineStart >= 0)
                                            {
                                                // 删除从lastLineStart到末尾的文本
                                                outputBox.Text = text.Substring(0, lastLineStart);
                                            }
                                        }
                                        isFirstToken = false;
                                    }
                                    outputBox.AppendText(displayToken);
                                });
                                await Task.Delay(20);
                            }
                        }
                    }
                    // 在流式输出结束后添加换行
                    outputBox.Invoke((MethodInvoker)delegate
                    {
                        outputBox.AppendText("\r\n\r\n");
                    });
                }
            }
        }

        public static async Task<string> GetDeepSeekSingleReply(string userMessage, string apiKey, string apiUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
                client.Timeout = TimeSpan.FromSeconds(30);

                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = new[] { new { role = "user", content = userMessage } },
                    max_tokens = 1350,
                    temperature = 0.2,
                    stream = true
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                using (var response = await client.PostAsync(apiUrl, content))
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                {
                    StringBuilder sb = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        if (line.StartsWith("data: "))
                        {
                            var jsonData = line.Substring(6).Trim();
                            if (jsonData == "[DONE]") break;

                            var json = JObject.Parse(jsonData);
                            var token = json["choices"]?[0]?["delta"]?["content"]?.ToString();

                            if (!string.IsNullOrEmpty(token))
                            {
                                string displayToken = token.Replace("#", " ").Replace("*", " ");
                                sb.Append(displayToken);
                            }
                        }
                    }
                    return sb.ToString();
                }
            }
        }
    }
}
