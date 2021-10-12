using System.Net.Http.Json;
using System.Text.Json;
using Serilog;
using Ubiquitous.AutoDevOps.Stack;

namespace Ubiquitous.AutoDevOps.GitLab; 

class GitLabClient {
    readonly string     _baseUrl;
    readonly HttpClient _httpClient;

    static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    static GitLabClient? Create() {
        var baseUrl = GetEnv("CI_API_V4_URL");
        var token   = GetEnv("GITLAB_API_TOKEN");
        return baseUrl.IsEmpty() || token.IsEmpty() ? null : new GitLabClient(baseUrl!, token!);
    }

    public static async Task PostMrNote(string noteContent) {
        var gitLabClient = Create();
        if (gitLabClient != null) {
            await gitLabClient.AddMergeRequestNote(noteContent);
        }
    }

    GitLabClient(string baseUrl, string token) {
        _baseUrl = baseUrl;

        _httpClient = new HttpClient {
            DefaultRequestHeaders = {{"PRIVATE-TOKEN", token}}
        };
    }

    async Task AddMergeRequestNote(string content) {
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

        if (!response.IsSuccessStatusCode) {
            Log.Warning("Posting a note wasn't successful: {Reason}", response.ReasonPhrase);
        }
    }

    static string? GetEnv(string varName) => Environment.GetEnvironmentVariable(varName);

    record NewNote(string Body);
}