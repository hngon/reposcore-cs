using Cocona;
using RepoScore.Data;
using RepoScore.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;
using System.Collections.Generic;

var app = CoconaApp.Create();

app.AddCommand(async ([Argument(Description = "저장소 이름 (예: owner/repo)")] string repo,
                      [Option('t', Description = "GitHub Personal Access Token")] string? token = null) =>
{
    // 1. 입력받은 repo 문자열 분리 (owner/repo 형태 처리)
    var repoParts = repo.Split('/');
    if (repoParts.Length != 2)
    {
        Console.WriteLine("[Error] 저장소 이름은 'owner/repo' 형식이어야 합니다. (예: torvalds/linux)");
        return;
    }
    string ownerName = repoParts[0];
    string repoName = repoParts[1];

    Console.WriteLine($"저장소: {repo}");
    if (!string.IsNullOrEmpty(token))
    {
        Console.WriteLine($"토큰 인증 사용 중 (토큰: {token[..Math.Min(4, token.Length)]}***)");
    }
    else
    {
        Console.WriteLine("토큰 미입력 - 비인증 모드로 실행 (API Rate Limit에 주의하세요)");
    }

    Console.WriteLine();
    Console.WriteLine("아이디, 문서이슈, 버그/기능이슈, 오타PR, 문서PR, 버그/기능PR, 총점");

    try
    {
        // 2. GitHubService 인스턴스 생성 및 실제 데이터 조회
        var githubService = new GitHubService(ownerName, repoName, token);

        var allPullRequests = await githubService.GetPullRequestsAsync(ItemStateFilter.All);
        var allIssues = await githubService.GetIssuesAsync(ItemStateFilter.All);

        // Merged 상태인 PR만 필터링
        var mergedPrs = allPullRequests.Where(pr => pr.MergedAt.HasValue).ToList();

        // 3. 기여한 모든 사용자 ID 수집 (이슈 작성자 + 병합된 PR 작성자 중복 제거)
        var allContributors = mergedPrs.Select(pr => pr.User.Login)
            .Concat(allIssues.Select(issue => issue.User.Login))
            .Distinct();

        // 4. 사용자별로 데이터 분류 및 점수 계산
        foreach (var userId in allContributors)
        {
            int docIssueCount = 0;
            int featureBugIssueCount = 0;
            int typoPrCount = 0;
            int docPrCount = 0;
            int featureBugPrCount = 0;

            // 해당 사용자의 이슈 필터링 및 분류
            var userIssues = allIssues.Where(i => i.User.Login.Equals(userId, StringComparison.OrdinalIgnoreCase));
            foreach (var issue in userIssues)
            {
                if (IsDoc(issue.Title, issue.Labels)) docIssueCount++;
                else featureBugIssueCount++;
            }

            // 해당 사용자의 병합된 PR 필터링 및 분류
            var userPrs = mergedPrs.Where(p => p.User.Login.Equals(userId, StringComparison.OrdinalIgnoreCase));
            foreach (var pr in userPrs)
            {
                if (IsDoc(pr.Title, pr.Labels)) docPrCount++;
                else if (IsTypo(pr.Title, pr.Labels)) typoPrCount++;
                else featureBugPrCount++;
            }

            // 5. ScoreCalculator에 실제 데이터 전달하여 점수 계산
            int totalScore = ScoreCalculator.CalculateFinalScore(
                featureBugPrCount,
                docPrCount,
                typoPrCount,
                featureBugIssueCount,
                docIssueCount
            );

            // 6. 기존 출력 형식에 맞게 데이터 출력
            // 아이디, 문서이슈, 버그/기능이슈, 오타PR, 문서PR, 버그/기능PR, 총점
            Console.WriteLine($"{userId}, {docIssueCount}, {featureBugIssueCount}, {typoPrCount}, {docPrCount}, {featureBugPrCount}, {totalScore}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[Error] 데이터를 가져오거나 계산하는 중 오류가 발생했습니다: {ex.Message}");
    }
});

app.Run();

// --- 데이터 분류(휴리스틱)를 위한 헬퍼 메서드 ---
// 라벨이나 제목에 특정 키워드가 포함되어 있는지 검사합니다.

static bool IsDoc(string title, IReadOnlyList<Label> labels)
{
    var keywords = new[] { "doc", "documentation", "문서" };
    bool hasLabel = labels.Any(l => keywords.Any(k => l.Name.Contains(k, StringComparison.OrdinalIgnoreCase)));
    bool hasTitle = keywords.Any(k => title.Contains(k, StringComparison.OrdinalIgnoreCase));
    return hasLabel || hasTitle;
}

static bool IsTypo(string title, IReadOnlyList<Label> labels)
{
    var keywords = new[] { "typo", "오타" };
    bool hasLabel = labels.Any(l => keywords.Any(k => l.Name.Contains(k, StringComparison.OrdinalIgnoreCase)));
    bool hasTitle = keywords.Any(k => title.Contains(k, StringComparison.OrdinalIgnoreCase));
    return hasLabel || hasTitle;
}
