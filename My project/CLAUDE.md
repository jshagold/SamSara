# CLAUDE.md — Samsara Project

**Version:** 2.0.0 | **Date:** 2026-05-28

이 파일은 Claude Code가 시작 시 자동으로 읽는 부트스트랩 문서다.
운영 규칙(지침)과 아키텍처 규칙(Constitution)으로 가는 진입점 역할만 한다.

---

## 지침 자동 로드

@.claude/ClaudeProjectInstruction.md

위 파일이 Samsara 프로젝트의 운영 규칙을 담는다.
- 도구 분담 (Claude Code / Web / Gemini)
- 절대 규칙 (추측 금지, Constitution 우선, UI 텍스트 하드코딩 금지)
- 아키텍처 레드라인 요약
- 노션 구조 안내
- AI Spec 작업 운영 (Status / Specify / Plan / Tasks / Decisions / Patch / Backlog)
- 작업 진행 원칙 (승인 흐름, 질문 규율)
- 응답 스타일 (한국어 존댓말)
- Notion 문서 작성 규칙
- 버전 관리 & Changelog

이 CLAUDE.md와 충돌 시 지침이 우선한다.

---

## Constitution (아키텍처 바이블)

**구현 착수 전 `.claude/constitution.md`를 반드시 읽는다.**

- 모든 아키텍처 결정의 단일 출처
- 폴더 구조, Bootstrapper 계층, DI 규칙, Coding Standards, Data Persistence, Libraries 등
- 지침 / CLAUDE.md와 충돌 시 Constitution이 우선
- 현재 read-only 상태 (수정은 명시적 승인 필요)

---

## 노션 진입점

**Samsara Project (루트):** https://www.notion.so/Samsara-Project-30252975d2df8073a47cf33c53e3703d

이 루트 페이지 아래에 지침 §4의 5개 영역(로드맵 / 게임 정의·기획 / 개발 tech 문서 / AI Spec 문서 / 일지)이 위치한다.

| 영역 / 문서 | 위치 |
|---|---|
| CLAUDE.md (원본) | 루트 > 개발 tech 문서 > CLAUDE.md |
| Claude Project Instruction / 지침 (원본) | 루트 > 개발 tech 문서 > Claude Project Instruction (지침) |
| Constitution (원본) | 루트 > 개발 tech 문서 > Constitution |
| AI Spec 문서 영역 | 루트 > AI Spec 문서 |
| 로드맵 | 루트 > 로드맵 |

- 어디를 봐야 할지 모르면 루트에서 해당 영역의 상위 페이지("들어갈 것 / 들어가지 않을 것 / 구분 기준")부터 확인한다.
- 작업 방법·양식·운영 규칙은 자동 로드된 지침이 1차 출처다. 지침에 답이 있으면 노션을 fetch하지 않는다.
- 노션의 CLAUDE.md / 지침 / Constitution 페이지가 단일 출처(SSOT)다. 로컬 파일은 그 사본이며, 노션 변경 시 동기화한다.

> 프로젝트 문서 개편 진행 중 — "진행 상태(Project Status)" 페이지는 현재 부재 상태이며 개편 결과에 따라 신설 또는 폐기 예정. 결정되면 이 표와 체크리스트를 갱신한다.

---

## 구현 착수 전 체크리스트

- [ ] 지침이 자동 로드되어 있는지 확인 (이 파일 상단 `@.claude/ClaudeProjectInstruction.md`)
- [ ] Constitution 해당 섹션 확인 (`.claude/constitution.md`)
- [ ] 필요 시 노션 루트(Samsara Project)에서 해당 작업 영역 fetch (현재 Project Status 페이지 부재 — 개편 중)
- [ ] 대상 작업의 Specify / Plan / Tasks (또는 Patch) 확인
- [ ] `Assets/_Game/` 경로 규칙 확인
- [ ] 참조 파일 존재 여부 확인 — 없으면 빈 인터페이스 stub 먼저 생성

---

## Changelog

- **2.0.0 (2026-05-28)** — 전면 재작성. 운영 규칙은 `ClaudeProjectInstruction.md`(v2.0.0)로 분리하고 `@` import로 자동 로드. 아키텍처/코딩 규칙은 Constitution이 단일 출처임을 명확화. CLAUDE.md는 부트스트랩 역할로 축소. 한국어 전환. 노션 진입점을 Samsara Project 루트 페이지로 지정하고, 옛 Project Status Page ID 제거(Project Status 페이지는 문서 개편 중 부재). CLAUDE.md를 노션의 SSOT 페이지로 관리하도록 표에 추가.
- **1.3.0 (2026-03-30)** — Decisions Tags, Patch Files, Project Status, Spec Document Locations 추가.
