using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using Ubiquitous.AutoDevOps.Stack;

namespace Ubiquitous.AutoDevOps.GitLab {
    class GitLabClient {
        readonly string     _baseUrl;
        readonly HttpClient _httpClient;

        static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

        public static GitLabClient? Create() {
            var baseUrl = GetEnv("CI_API_V4_URL");
            // var token   = GetEnv("GITLAB_API_TOKEN");
            var token   = GetEnv("CI_JOB_TOKEN");
            return baseUrl.IsEmpty() || token.IsEmpty() ? null : new GitLabClient(baseUrl!, token!);
        }

        GitLabClient(string baseUrl, string token) {
            _baseUrl = baseUrl;

            _httpClient = new HttpClient {
                DefaultRequestHeaders = {{"PRIVATE-TOKEN", token}}
            };
        }

        public async Task AddMergeRequestNote(string content) {
            var projectId = GetEnv("CI_PROJECT_ID");
            var mrIid     = GetEnv("CI_MERGE_REQUEST_IID");

            if (projectId == null || mrIid == null) {
                Log.Information("Project or merge request id not defined");
                return;
            }

            Log.Information("Adding a note to the merge request");

            var resource = $"{_baseUrl}/projects/{projectId}/merge_requests/{mrIid}/notes";

            var note     = new NewNote(content);
            var response = await _httpClient.PostAsJsonAsync(resource, note, SerializerOptions);
            Log.Information("Result: {Code} {Reason}", response.StatusCode, response.ReasonPhrase);
        }

        static string? GetEnv(string varName) => Environment.GetEnvironmentVariable(varName);

        record NewNote(string Body);
    }
}