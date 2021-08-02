using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Ubiquitous.AutoDevOps.Stack;

namespace Ubiquitous.AutoDevOps.GitLab {
    class GitLabClient {
        readonly HttpClient            _httpClient;
        readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

        public static GitLabClient? Create() {
            var baseUrl = GetEnv("CI_API_V4_URL");
            return baseUrl.IsEmpty() ? null : new GitLabClient(baseUrl!);
        }

        GitLabClient(string baseUrl) {
            _httpClient = new HttpClient {
                BaseAddress           = new Uri(baseUrl),
                DefaultRequestHeaders = {{"PRIVATE-TOKEN", GetEnv("CI_JOB_TOKEN")}}
            };
        }

        public async Task AddMergeRequestNote(string content) {
            var projectId = GetEnv("CI_MERGE_REQUEST_PROJECT_ID");
            if (projectId == null) return;

            var mrIid = GetEnv("CI_MERGE_REQUEST_IID")!;

            var resource = $"projects/{projectId}/merge_requests/{mrIid}/notes";
            var note     = new NewNote(content);
            await _httpClient.PostAsJsonAsync(resource, note, _serializerOptions);
        }

        static string? GetEnv(string varName) => Environment.GetEnvironmentVariable(varName);

        record NewNote(string Body);
    }
}