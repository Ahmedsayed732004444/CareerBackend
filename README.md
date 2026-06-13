# Career-Path — Detailed Project Documentation

This document provides an in-depth description of the **Career-Path** system, intended as a reference for your graduation project documentation (system overview, modules, architecture, database design, and API reference chapters).

---

## 1. Project Overview

Career-Path is a full-stack **AI-powered career development and professional networking platform**. It is designed to support job seekers and companies through the entire career journey: building a profile, discovering and applying to jobs, receiving AI-driven career guidance (CV analysis, job matching, learning roadmaps, mock interviews), and engaging with a professional social community (posts, comments, follows, real-time chat, and notifications).

The system consists of three cooperating parts:

1. **Backend API (this repository)** — an ASP.NET Core Web API responsible for business logic, authentication, data persistence (SQL Server via EF Core), real-time communication (SignalR), and orchestration of AI features.
2. **Frontend client** — a separate web application (React, deployed on Vercel) that consumes the API.
3. **AI/Extraction microservice** — an external service (referred to in configuration as `ExtractionApi`) that performs the actual AI/NLP work: CV text extraction & analysis, skill-based job matching, roadmap generation, and interview question generation. The .NET backend communicates with this service over HTTP.

---

## 2. Problem Statement & Motivation

Job seekers, especially new graduates, often face several challenges:

- Difficulty understanding which skills/jobs match their CV.
- Lack of a clear, personalized learning path toward a target career.
- Limited practice opportunities for technical interviews.
- Fragmented experience: job boards, networking, and learning resources are usually separate platforms.

Career-Path addresses these problems by combining **job discovery, AI-driven self-assessment, personalized guidance, and professional networking** into a single platform, reducing the friction between "I have a CV" and "I know what to do next."

---

## 3. Objectives

- Provide a unified platform for job seekers and companies (recruiters).
- Use AI to automatically extract structured information (skills, education, experience) from an uploaded CV.
- Recommend the most relevant job openings to a user based on their extracted skills.
- Generate a personalized career roadmap (learning plan) for users.
- Allow companies to generate AI-based interview questions for their job postings and let candidates practice them.
- Provide a social layer (posts, likes, comments, follows) so users can build a professional network and share content.
- Support real-time messaging and notifications between users.
- Implement role-based access control (Admin, Company, Member) with fine-grained permissions.
- Provide a membership upgrade workflow for premium features.

---

## 4. User Roles

The system defines three default roles (see `DefaultRoles` and `Permissions` constants):

| Role | Description |
|---|---|
| **Admin** | Full administrative access: manage users, roles & permissions, review membership upgrade requests, manage all jobs. |
| **Company** | Represents an employer/recruiter account. Can post and manage job listings, view applicants, generate AI interview questions for their jobs, and review job submissions. |
| **Member** | Represents a regular job seeker / professional user. Can build a profile, upload a CV, apply to jobs, use AI career tools (matching, roadmap, mock interviews), and participate in the social feed (posts, comments, likes, follows, chat). |

Permissions are granted per role and enforced through a custom `[HasPermission]` authorization attribute (policy-based authorization), e.g.:
- `users:read`, `users:add`, `users:update`
- `roles:read`, `roles:add`, `roles:update`
- `profile:read`, `profile:update`
- `jobs:read`, `jobs:add`, `jobs:update`, `jobs:delete`
- `jobApplicants:read`
- `membershipUpgradeRequests:read` / `approve` / `reject`

---

## 5. System Architecture

```
                ┌──────────────────────────┐
                │        Frontend           │
                │  (React, hosted on Vercel)│
                └──────────────┬─────────────┘
                                 │ REST + SignalR (WebSockets)
                                 ▼
                ┌──────────────────────────────────┐
                │     Career-Path Backend API        │
                │      (ASP.NET Core Web API)        │
                │                                     │
                │  - Controllers (REST endpoints)    │
                │  - Services (business logic)       │
                │  - SignalR Hubs (Chat, Notifications)│
                │  - Identity & JWT Auth              │
                │  - EF Core (ApplicationDbContext)   │
                └───────┬───────────────────┬─────────┘
                        │                     │
                        ▼                     ▼
            ┌────────────────────┐   ┌─────────────────────────────┐
            │   SQL Server DB     │   │  AI / Extraction Microservice │
            │ (EF Core Migrations)│   │  (FastAPI-like external API)  │
            │                     │   │  - CV parsing                  │
            │  Users, Jobs, Posts,│   │  - Job matching                │
            │  Notifications, ... │   │  - Roadmap generation           │
            └────────────────────┘   │  - Interview question generation│
                                       └─────────────────────────────┘
                        │
                        ▼
            ┌────────────────────┐
            │  RemoteOK Job API   │  (external public job board, aggregated)
            └────────────────────┘
```

### Communication patterns
- **REST API** for CRUD operations (jobs, posts, profiles, applications, etc.).
- **SignalR Hubs** for real-time features:
  - `ChatHub` — one-to-one messaging.
  - `NotificationHub` — live notification delivery and unread-count updates.
- **Outbound HTTP calls** from the backend to:
  - The AI microservice (`ExtractionApi:BaseUrl`) for CV parsing (`/cv-box`), job matching, roadmap generation, and interview question generation (`/interview-questions`).
  - RemoteOK's public API for aggregating remote job listings.
- **Email** sending via SMTP (`MailSettings`) for account confirmation, password reset, and notification emails.

---

## 6. Core Modules & Features

### 6.1 Authentication & Identity (`AuthController`, `AuthService`, `Authentication/*`)
- Email/password **registration & login** using ASP.NET Core Identity.
- **Email confirmation** flow (confirm-email, resend-confirmation-email).
- **Password reset** flow (forget-password, reset-password).
- **JWT access tokens** + **refresh tokens** (refresh, revoke-refresh-token) for stateless authentication.
- **OAuth login** with **Google** and **GitHub** (login + callback/response endpoints), allowing social sign-in/sign-up.
- **Permission-based authorization**: a custom `HasPermissionAttribute`, `PermissionAuthorizationHandler`, and `PermissionAuthorizationPolicyProvider` check whether the authenticated user's role(s) include a required permission string (e.g., `jobs:add`) before allowing access to an endpoint.

### 6.2 User & Role Management (`UsersController`, `RolesController`, `UserService`, `RoleService`)
- Admins can list, view, create, update, enable/disable (toggle status), and unlock user accounts.
- Role management: create/update roles, assign permissions, toggle role status, list all available permissions.

### 6.3 User Profile Module (`UserProfileController`, `UserProfileService`)
- View/update **basic info** (gender, country, city, job title, years of experience, current company).
- Manage **education** and **skills** sections.
- Manage **summary/bio**.
- Upload/replace/delete **profile picture** and **cover picture**.
- Upload/replace/delete **CV file** and check whether a user has uploaded a CV (`has-resumes`).
- Retrieve another user's public profile by `userId`.

### 6.4 Jobs Module (`JobsController`, `JobService`, `Job` entity)
- Companies can **create, update, delete, and toggle the active status** of job postings.
- Job postings include: title, description, job type, required skills/requirements, location, experience level (`EntryLevel` → `Executive`), salary range, posted date, and deadline.
- Public job listing and search, including listing jobs by a specific company.
- Candidates can **apply to a job** directly.
- Companies can view the **list of applicants** for a specific job.
- Companies can trigger **AI-generated interview questions** for a specific job posting (`generate-questions` endpoint → calls the AI microservice's `interview-questions` endpoint, results stored as `JobInterview`/`JobInterviewOption` entities).
- **External job aggregation**: the system integrates with the **RemoteOK** public job API (and supports a generic job-search contract resembling the Adzuna API format — `JobSearchRequest`/`JobSearchResponse`) to pull in remote job listings alongside locally posted jobs.

### 6.5 Job Applications & Tracking
- **`JobTrackerController`** — lets a user maintain a personal list of job applications they've made anywhere (not just on this platform), tracking: job title, company name, application date, status (`Applied`, `Interviewed`, `Offered`, `Rejected`, `Accepted`), source (e.g., LinkedIn, referral), and notes. Full CRUD support.
- **`JobSubmissionsController`** — manages the workflow where a candidate submits an application (with CV, phone, notes) for a specific job posted by a company; companies can view submissions for their jobs and send notes back to applicants; users can view "my" submissions and withdraw them.

### 6.6 AI Career Tools

#### a. Resume / CV Analysis (`ResumeController`, `ParsingService`, `ModelExtration` entity)
- Endpoint: `POST /api/Resume/update/analayse`
- Flow:
  1. The uploaded CV file is saved/updated on the user's profile.
  2. The file is forwarded to the AI microservice's `cv-box` endpoint.
  3. The AI service extracts structured data: full name, email, phone, location, summary, **skills**, **education history**, **work experience**, **certifications**, **languages** — persisted in the `ModelExtration` entity (one-to-one with the user).
  4. A textual **CV review** is returned to the frontend immediately for display.

#### b. AI Job Matching (`MatchController`, `MatchService`)
- Endpoint: `GET /api/Match`
- Flow:
  1. Verifies the user has uploaded a CV.
  2. Executes a SQL **stored procedure** (`GetTop10MatchingJobs`) to pre-filter the top candidate jobs for the user from the database.
  3. Retrieves the user's extracted **skills** (from `ModelExtration`).
  4. Sends the candidate jobs + user skills to the AI microservice, which returns a **ranked list of job matches** (`JobMatchResult`), likely including a match score/explanation per job.

#### c. Career Roadmap Generation (`RoadmapController`, `RoadmapService`, `RoadmapJson` entity)
- Lets users generate a personalized **career learning roadmap** (stored as JSON/`RoadmapData`).
- Users can:
  - List all their generated roadmaps (paginated, searchable).
  - List only their **saved** roadmaps.
  - View a specific roadmap by ID.
  - **Save/unsave (toggle)** a roadmap for future reference.

#### d. Mock Interview Practice (`InterviewController`, `InterviewService`, `JobInterview`/`JobInterviewOption` entities)
- For a given job, retrieves the AI-generated **multiple-choice interview questions** (`GET /api/Interview/{jobId}/questions`).
- Candidates submit their answers (`POST /api/Interview/{jobId}/submit`); the service:
  - Compares each answer to the correct option.
  - Computes the number of correct answers and an overall **score percentage**.
  - Returns a detailed breakdown per question (question text, the user's answer, the correct answer, and whether it was correct).

### 6.7 Social Networking Feed

#### Posts (`PostsController`, `PostService`, `Post` entity)
- Create, update, soft-delete, and retrieve posts (with optional attached file/image).
- Retrieve the logged-in user's posts, another user's posts, or a general feed.

#### Likes (`LikesController`, `LikeService`, `PostLike` entity)
- Like / unlike a post, and view who liked a post.

#### Comments & Replies (`CommentsController`, `CommentService`, `PostComment`, `CommentReply`, `CommentReaction`, `ReplyReaction`)
- Add comments to a post; delete a comment.
- Add threaded **replies** to comments; delete a reply.
- **React (like)** to comments and replies (add/remove reactions).
- Retrieve all comments for a post and all replies for a comment.

#### Follow System (`FollowController`, `FollowService`, `UserFollow` entity)
- Follow / unfollow other users.
- View a user's followers and following lists, and the current user's own followers/following.

### 6.8 Real-Time Messaging (`ChatController`, `ChatHub`, `ChatService`, `Message` entity)
- One-to-one chat between users.
- Retrieve the conversation history with another user.
- Mark messages as **read**.
- Real-time delivery of new messages via SignalR (`ChatHub`).

### 6.9 Notifications (`NotificationsController`, `NotificationHub`, `NotificationService`, `Notification`/`NotificationPreference` entities)
- A unified notification system covering:
  - **Social** events: new follower, post liked, post commented, comment replied, comment reacted.
  - **Job** events: job application received, job application status changed.
  - **Chat**: new message.
  - **System**: security alerts, general info.
- Each notification has a **type**, **priority** (Low/Normal/High), title, message, and a reference to the related entity (e.g., `PostId`, `JobId`).
- Endpoints support: listing notifications, getting unread count, marking one/all as read, deleting a notification, and getting/updating **per-type notification preferences** (in-app vs. email).
- Real-time delivery and unread-count updates pushed via `NotificationHub`.
- Optional **email notifications** are sent based on user preference (`EmailService`).

### 6.10 Membership Upgrades (`MembershipController`, `MembershipUpgradeService`, `MembershipUpgrade` entity)
- Users can **request** a membership upgrade (e.g., to a premium tier), with an optional note.
- Admins can **list all requests**, then **approve** or **reject** them (`RequestStatus`: Pending / Approved / Rejected).

---

## 7. Database Design — Key Entities

| Entity | Purpose |
|---|---|
| `ApplicationUser` | Core identity user (extends `IdentityUser`), with first/last name, disabled flag, and navigation to profile, jobs, posts, follows, etc. |
| `UserProfile` | Extended profile info: location, career details, education, summary, profile/cover pictures, CV URL, skills list. |
| `Job` | Job postings created by company accounts: title, description, type, requirements, location, experience level, salary range, dates, active flag. |
| `JobApplication` | A user's personal tracker entry for a job they applied to (status, source, notes). |
| `JobSubmission` | A formal application submitted through the platform for a specific job, including CV path and notes. |
| `ModelExtration` | AI-extracted structured CV data (skills, education, experience, certifications, languages) per user. |
| `RoadmapJson` | AI-generated career roadmap data per user, with saved/unsaved flag. |
| `JobInterview` / `JobInterviewOption` | AI-generated interview questions and multiple-choice options for a job. |
| `Post` / `PostLike` / `PostComment` / `CommentReply` / `CommentReaction` / `ReplyReaction` | Social feed entities for posts, likes, comments, replies, and reactions. |
| `UserFollow` | Follow relationships between users. |
| `Message` | Chat messages between two users. |
| `Notification` / `NotificationPreference` | Notification records and per-user, per-type delivery preferences. |
| `MembershipUpgrade` | Membership upgrade requests and their approval status. |
| `RefreshToken` | JWT refresh tokens issued per user. |
| `ApplicationRole` | Roles (Admin, Company, Member) with assigned permissions (claims). |

These entities and their relationships can be used as the basis for an **Entity-Relationship Diagram (ERD)** in your documentation.

---

## 8. API Reference (by Module)

### Auth (`/auth`)
| Method | Endpoint | Description |
|---|---|---|
| POST | `/auth` | Login |
| POST | `/auth/register` | Register a new account |
| POST | `/auth/refresh` | Refresh JWT access token |
| POST | `/auth/revoke-refresh-token` | Revoke a refresh token |
| POST | `/auth/confirm-email` | Confirm email address |
| POST | `/auth/resend-confirmation-email` | Resend confirmation email |
| POST | `/auth/forget-password` | Request password reset |
| POST | `/auth/reset-password` | Reset password |
| GET | `/auth/google-login` / `/auth/google-response` | Google OAuth flow |
| GET | `/auth/github-login` / `/auth/github-response` | GitHub OAuth flow |

### Users & Roles
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Users` | List users |
| GET | `/api/Users/{id}` | Get user by ID |
| POST | `/api/Users` | Create user |
| PUT | `/api/Users/{id}` | Update user |
| PUT | `/api/Users/{id}/toggle-status` | Enable/disable user |
| PUT | `/api/Users/{id}/unlock` | Unlock locked-out user |
| GET | `/api/Roles` / `/api/Roles/permissions` | List roles / permissions |
| GET, POST, PUT | `/api/Roles/{id}` | Get / create / update roles |
| PUT | `/api/Roles/{id}/toggle-status` | Enable/disable role |

### User Profile
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/UserProfile` | Get current user's profile |
| GET | `/api/UserProfile/{userId}` | Get another user's profile |
| GET | `/api/UserProfile/has-resumes` | Check if CV uploaded |
| GET | `/api/UserProfile/profile-picture` | Get profile picture |
| PUT | `/api/UserProfile/password` | Change password |
| PUT | `/api/UserProfile/basic-Info` | Update basic info |
| PUT/DELETE | `/api/UserProfile/cv` | Upload / delete CV |
| PUT/DELETE | `/api/UserProfile/picture` | Upload / delete profile picture |
| PUT/DELETE | `/api/UserProfile/cover-picture` | Upload / delete cover picture |

### Jobs & Applications
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Jobs` | List/search jobs |
| GET | `/api/Jobs/{id}` | Get job by ID |
| GET | `/api/Jobs/company/{companyId}` | List jobs by company |
| POST | `/api/Jobs` | Create job (company) |
| PUT | `/api/Jobs/{jobId}` | Update job |
| DELETE | `/api/Jobs/{jobId}` | Delete job |
| PUT | `/api/Jobs/{jobId}/toggle-status` | Activate/deactivate job |
| POST | `/api/Jobs/{jobId}/apply` | Apply to a job |
| GET | `/api/Jobs/{jobId}/applicants` | List applicants |
| POST | `/api/Jobs/{jobId}/generate-questions` | Generate AI interview questions |
| GET/POST/PUT/DELETE | `/api/JobTracker` | Personal job application tracker |
| GET | `/api/JobSubmissions/companies/{companyId}/jobs/{jobId}` | List submissions for a job |
| GET | `/api/JobSubmissions/my` | My submissions |
| POST | `/api/JobSubmissions/.../send-note` | Company sends note to applicant |
| DELETE | `/api/JobSubmissions/{submissionId}` | Withdraw submission |

### AI Career Tools
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Resume/update/analayse` | Upload & AI-analyze CV |
| GET | `/api/Match` | Get AI job match results |
| GET | `/api/Roadmap` | List user's roadmaps |
| GET | `/api/Roadmap/saved` | List saved roadmaps |
| GET | `/api/Roadmap/{id}` | Get specific roadmap |
| POST | `/api/Roadmap/{id}/toggle-status` | Save/unsave roadmap |
| GET | `/api/Interview/{jobId}/questions` | Get interview questions |
| POST | `/api/Interview/{jobId}/submit` | Submit interview answers |

### Social
| Method | Endpoint | Description |
|---|---|---|
| GET, POST, PUT, DELETE | `/api/Posts` | Manage posts |
| POST/DELETE | `/api/likes/{postId}` | Like / unlike a post |
| GET | `/api/likes/{postId}` | List likes |
| POST/DELETE | `/api/Comments/{postId}` / `/{commentId}` | Add/delete comments |
| POST/DELETE | `/api/Comments/{commentId}/replies` | Add/delete replies |
| POST/DELETE | `/api/Comments/{commentId}/like`, `/replies/{replyId}/like` | React to comments/replies |
| POST/DELETE | `/api/Follow/{followingId}` | Follow / unfollow |
| GET | `/api/Follow/{userId}/followers`, `/following`, `/my/followers`, `/my/following` | Follow lists |

### Chat & Notifications
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Chat/{otherUserId}` | Get conversation |
| PUT | `/api/Chat/{senderId}/read` | Mark messages read |
| (SignalR) | `/ChatHub` | Real-time messaging |
| GET | `/api/notifications` | List notifications |
| GET | `/api/notifications/unread-count` | Unread count |
| PUT | `/api/notifications/{id}/read`, `/read-all` | Mark as read |
| DELETE | `/api/notifications/{id}` | Delete notification |
| GET/PUT | `/api/notifications/preferences` | Notification preferences |
| (SignalR) | `/NotificationHub` | Real-time notifications |

### Membership
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Membership/request` | Request upgrade |
| GET | `/api/Membership/requests` | List requests (admin) |
| PUT | `/api/Membership/requests/{id}/approve` | Approve request |
| PUT | `/api/Membership/requests/{id}/reject` | Reject request |

---

## 9. Technology Stack Summary

- **Backend Framework**: ASP.NET Core Web API (.NET)
- **Database**: Microsoft SQL Server + Entity Framework Core (Code-First, migrations, stored procedures e.g. `GetTop10MatchingJobs`)
- **Authentication**: ASP.NET Core Identity, JWT Bearer + Refresh Tokens, Google OAuth, GitHub OAuth, custom permission-based authorization policies
- **Real-Time Communication**: SignalR (`ChatHub`, `NotificationHub`)
- **Object Mapping**: Mapster
- **Validation**: FluentValidation
- **Email**: SMTP-based `EmailService` with HTML body builders for confirmations, password resets, and notifications
- **External Integrations**:
  - AI/Extraction microservice (CV parsing, job matching, roadmap generation, interview question generation)
  - RemoteOK public job API (and Adzuna-style job search contracts)
- **API Documentation**: OpenAPI / Swagger
- **Background Processing**: Background job configuration for asynchronous tasks
- **Frontend**: React (deployed via Vercel) — consumes the REST API and SignalR hubs

---

## 10. Suggested Documentation Chapters for Your Graduation Report

Based on the above, you can structure your graduation documentation as:

1. **Introduction** — problem statement, motivation, objectives (Sections 2–3 above).
2. **System Analysis** — user roles, use cases, functional & non-functional requirements (Section 4 + module list in Section 6).
3. **System Design** — architecture diagram (Section 5), ERD (Section 7), sequence diagrams for key AI workflows (CV analysis, job matching, roadmap generation, interview generation).
4. **Implementation** — technology stack (Section 9), key modules and how they were implemented, API reference (Section 8).
5. **AI Integration** — how the .NET backend communicates with the external AI microservice for CV parsing, matching, roadmaps, and interview question generation; data flow diagrams.
6. **Testing** — describe testing strategy (unit/integration tests, Postman/`.http` collections such as `Career-Path.http`).
7. **Results & Screenshots** — frontend screens demonstrating each module.
8. **Conclusion & Future Work** — possible enhancements (e.g., analytics dashboard for companies, advanced search filters, video interviews, multi-language support).

---

*This document was generated from the project's source code structure (controllers, services, entities, and configuration) to support the team's graduation project documentation.*
