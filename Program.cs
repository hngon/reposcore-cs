using Cocona;

var app = CoconaApp.Create();

app.AddCommand(([Argument(Description = "분석할 GitHub 저장소 (예: owner/repo)")] string repo,
                [Option('t', Description = "GitHub Personal Access Token (비공개 저장소 접근 및 속도 제한 방지)")] string? token = null,
                [Option('b', Description = "분석할 특정 브랜치 (기본값: 기본 브랜치)")] string? branch = null,
                [Option(Description = "분석 시작 날짜 (형식: YYYY-MM-DD)")] DateTimeOffset? since = null,
                [Option(Description = "분석 종료 날짜 (형식: YYYY-MM-DD)")] DateTimeOffset? until = null) =>
{
    Console.WriteLine($"저장소: {repo}");

    if (!string.IsNullOrEmpty(token))
    {
        Console.WriteLine($"토큰 인증 사용 중 (토큰: {token[..Math.Min(4, token.Length)]}***)");
    }
    else
    {
        Console.WriteLine("토큰 미입력 - 비인증 모드로 실행");
    }

    if (!string.IsNullOrEmpty(branch)) Console.WriteLine($"브랜치: {branch}");
    if (since.HasValue) Console.WriteLine($"시작일: {since.Value:yyyy-MM-dd}");
    if (until.HasValue) Console.WriteLine($"종료일: {until.Value:yyyy-MM-dd}");

    Console.WriteLine();
    Console.WriteLine("아이디, 문서이슈, 버그/기능이슈, 오타PR, 문서PR, 버그/기능PR, 총점");
    Console.WriteLine("user1, 1, 2, 1, 3, 1, 100");
    Console.WriteLine("user2, 1, 2, 5, 3, 2, 120");
    Console.WriteLine("user3, 3, 2, 5, 6, 5, 150");
    
    Console.WriteLine();
    Console.WriteLine("> 상세 분석 기능은 구현 중입니다.");
}).WithDescription("GitHub 저장소의 활동을 분석하여 기여도 점수를 계산합니다.");

app.Run();