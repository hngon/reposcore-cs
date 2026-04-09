# reposcore-cs
A CLI for scoring student participation in an open-source class repo, implemented in C# using GraphQL

## Overview

`reposcore-cs`는 오픈소스 수업에서 학생들의 GitHub 기여도(PR, 이슈)를 자동으로 분석하고 점수를 산출하는 CLI 도구입니다. GitHub GraphQL API를 활용하여 데이터를 수집하고, 기여 내역에 따라 점수를 계산합니다.

## Documentation
상세한 설치 가이드 및 기여 방법은 [docs/](./docs) 디렉토리를 참고해 주세요.

## Quick Start

### 1. 사전 준비 
(현재 Codespace에서는 필요없음. 이미 설치되어 있을 것임.)
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) 설치 필요
  - 자세한 설치 방법은 [docs/dotnet-guide.md](docs/dotnet-guide.md) 참고

### 2. 저장소 클론 (Codespace에서는 필요없음)

```bash
git clone https://github.com/oss2026hnu/reposcore-cs.git
cd reposcore-cs
```

### 3. 빌드

```bash
dotnet build
```

### 4. 실행

```bash
dotnet run
```

요청하신 대로, 이전에 논의했던 특정 기능뿐만 아니라 `-h`, `--help`를 포함하여 프로그램 전체를 아우르는 **종합적인 명령어 Synopsis 및 사용 가이드**를 작성해 드립니다. 

이 내용을 `README.md` 파일의 '사용법' 섹션에 그대로 복사하여 사용하시면 됩니다. 의미 없는 이모지는 배제하고 가독성 높은 표 형태로 구성했습니다.

---

## 사용법 (Synopsis & Options)

`reposcore-cs`는 Cocona 프레임워크를 기반으로 구축되어 단일 실행 구조를 가집니다. 복잡한 하위 명령어 없이, 실행 시 인자(Arguments)와 옵션(Options)을 조합하여 원하는 기능을 수행합니다.

### 기본 명령어 구조 (Synopsis)

```bash
dotnet run -- <Owner> <Repo> <TargetUser> [Options]
```
*(참고: `dotnet run` 명령어 뒤에 반드시 `--`를 붙여야 뒤따르는 인자와 옵션이 애플리케이션으로 정상 전달됩니다.)*

### 인자 (Arguments)
명령어 실행 시 **반드시 정해진 순서대로** 입력해야 하는 필수 값들입니다.

| 순서 | 인자명 | 설명 | 입력 예시 |
| :--- | :--- | :--- | :--- |
| 1 | **`Owner`** | 분석할 GitHub 저장소의 소유자 (조직명 또는 개인 계정) | `torvalds`, `microsoft` |
| 2 | **`Repo`** | 분석할 GitHub 저장소의 이름 | `linux`, `vscode` |
| 3 | **`TargetUser`** | 기여도 점수를 조회할 대상 학생(사용자)의 GitHub ID | `student-id` |

### 옵션 (Options)
명령어의 동작 방식을 제어하거나 부가적인 기능을 켜고 끄는 역할을 합니다. 순서에 상관없이 자유롭게 배치할 수 있습니다.

| 단축 | 전체 명령어 | 설명 |
| :--- | :--- | :--- |
| `-t` | `--token` | GitHub GraphQL API 통신을 위한 Personal Access Token (PAT). 미입력 시 프로그램이 실행되지 않습니다. |
| `-h` | `--help` | 프로그램의 전체 사용법, 인자 및 옵션 목록을 보여주는 도움말 화면을 출력합니다. (Cocona 프레임워크 자동 지원 기능) |

### 실행 예시

**1. 도움말 확인 (가장 먼저 실행해 볼 명령어)**
프로그램의 구체적인 사용법이 기억나지 않을 때 사용합니다.
```bash
dotnet run -- --help
```

---

## 향후 명령어 추가 가이드

`reposcore-cs` 프로젝트에 새로운 기능을 기여하고자 하는 팀원들을 위한 가이드입니다. 프로그램이 너무 복잡해지는 것을 막기 위해 아래의 원칙을 지켜주세요.

1. **하위 명령어(Sub-command) 추가 지양: `app.AddCommand`를 통한 하위 명령어 생성을 피해주세요.
2. **옵션(Option/Flag) 활용:** 새로운 기능은 메인 로직 내에서 `[Option]` 어트리뷰트를 활용한 스위치 형태로 구현하는 것을 권장합니다.
3. **문서화 필수:** 코드를 수정하여 새로운 인자나 옵션이 생겼다면, 반드시 본 `README.md`의 **사용법** 섹션(표 및 실행 예시)을 최신 상태로 업데이트하여 다른 사용자들이 혼란을 겪지 않도록 해야 합니다.


> 현재 개발 진행 중으로 실행 옵션 및 사용법은 추후 업데이트될 예정입니다.

## GitHub Markdown 문서(확장자 `.md` 파일) 작성에 대한 표준 가이드

## 참고자료
- GitHub Markdown (확장자 .md 파일) [기본 서식 구문](https://docs.github.com/ko/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)
