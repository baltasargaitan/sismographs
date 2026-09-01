# Quick Start Guide - Portfolio Project Submission

## 1. Local Setup (One-Time)

### Copy Environment Template
```powershell
cd c:\Users\Usuario\Desktop\Baltasar\GITHUB\Projects\sismographs
cp .env.example .env
```

### Edit .env with Your Configuration
```
# Open .env and update:
ConnectionStrings__DefaultConnection=Server=localhost\SQLEXPRESS;Database=SistemaSismografosDB;Trusted_Connection=True;TrustServerCertificate=True;Connection Timeout=30;
MAIL_USER=your-mailjet-api-key
MAIL_KEY=your-mailjet-secret-key
SMTP_FROM=noreply@yourdomain.com
SMTP_NAME="Your Name"
FRONTEND_ORIGIN=http://localhost:5173
```

## 2. Verify Everything Works

### Build Backend
```powershell
cd c:\Users\Usuario\Desktop\Baltasar\GITHUB\Projects\sismographs
dotnet build --configuration Release
```
Expected: ✅ "Compilación correcto con 0 Errores"

### Run Tests
```powershell
dotnet test --configuration Release --no-build
```
Expected: ✅ "14 Passed | 0 Failed"

### Build Frontend
```powershell
cd frontend-react
npm ci
npm run build
```
Expected: ✅ "built in [X]s"

## 3. Commit Changes to Git

### Stage All Changes
```powershell
cd c:\Users\Usuario\Desktop\Baltasar\GITHUB\Projects\sismographs
git add .
git status
```

### Suggested Commit Strategy (Atomic Commits)

**Commit 1: Security Hardening**
```powershell
git reset HEAD  # Unstage all
git add Api/Program.cs
git add Aplicacion/Servicios/Notificaciones/ObservadorEmailSMTP.cs
git add .gitignore
git add .env.example
git commit -m "chore(security): remove secrets from console logging

- Removed 4 Console.WriteLine statements from Api/Program.cs that printed SMTP credentials
- Removed 3 debug statements from ObservadorEmailSMTP.cs
- Added .env.example as template for environment configuration
- Enhanced .gitignore to exclude environment files but include .env.example

Fixes: Secrets no longer exposed in logs"
```

**Commit 2: Configuration Management**
```powershell
git add Api/appsettings.json
git add Api/appsettings.Development.json
git add Infraestructura/ServiceCollectionExtensions.cs
git add Infraestructura/Persistencia/AppDbContextFactory.cs
git commit -m "chore(config): parameterize database connection string

- Changed hardcoded connection string from NTBK-GAITAN to localhost\SQLEXPRESS
- Added environment variable reading: CONNECTIONSTRING_DEFAULTCONNECTION
- Updated ServiceCollectionExtensions to use env var with config fallback
- Updated AppDbContextFactory for EF Core CLI commands
- Connection string now portable across development machines

Refs: Environment-based configuration pattern"
```

**Commit 3: CORS Security**
```powershell
git add Api/Program.cs  # (if not already committed)
git commit -m "chore(security): restrict CORS to environment-based origin

- Changed from AllowAnyOrigin() to WithOrigins(frontendOrigin)
- Added FRONTEND_ORIGIN environment variable (default: http://localhost:5173)
- CORS policy now production-ready and environment-specific

Fixes: Overly permissive CORS configuration"
```

**Commit 4: Documentation**
```powershell
git add README.md
git add TRANSFORMATION_SUMMARY.md
git commit -m "docs: comprehensive portfolio documentation

- Rewrote README.md with professional structure
  * Hero section with tech stack badges
  * System architecture multi-layer diagram
  * Observer pattern explanation with diagrams
  * Technology stack table
  * Local development setup guide
  * Environment configuration details
  * Testing and CI/CD sections
- Added TRANSFORMATION_SUMMARY.md with complete project documentation"
```

**Commit 5: Testing Infrastructure**
```powershell
git add Tests/
git add SistemaSismografos.sln
git commit -m "test: add xUnit test projects with 14 test cases

- Created Dominio.Tests: 3 entity and business rule tests
- Created Aplicacion.Tests: 9 Observer pattern and use case tests
- Created Api.Tests: 2 API endpoint structure tests
- All tests passing: 14/14 ✓
- Uses xUnit and Moq for enterprise testing patterns

Implements: Test-driven development culture"
```

**Commit 6: CI/CD Automation**
```powershell
git add .github/
git commit -m "ci: add GitHub Actions CI/CD workflow

- Added .github/workflows/ci.yml with 2 parallel jobs
- Backend job: restore → build → test (.NET 9.0)
- Frontend job: npm ci → lint → build (Node.js 18)
- Triggered on: push to main/develop, PR to main/develop
- All jobs must pass before merge

Enables: Automated quality assurance"
```

### Alternative: Single Comprehensive Commit
If you prefer one commit for the entire transformation:
```powershell
git add .
git commit -m "refactor: transform academic project to professional portfolio

SECURITY IMPROVEMENTS:
- Remove secrets from console logging (Program.cs, ObservadorEmailSMTP.cs)
- Parameterize database connection string with env vars
- Restrict CORS to environment-based origin
- Create .env.example template (exclude .env from git)

CONFIGURATION:
- Environment-based database connection (CONNECTIONSTRING_DEFAULTCONNECTION)
- Environment-based CORS origin (FRONTEND_ORIGIN)
- SMTP credentials via environment variables

TESTING:
- Add 3 xUnit test projects with 14 test cases (100% passing)
- Dominio.Tests: Entity validation (3 tests)
- Aplicacion.Tests: Observer pattern validation (9 tests)
- Api.Tests: API structure tests (2 tests)

DOCUMENTATION:
- Rewrite README with professional structure, diagrams, setup instructions
- Add TRANSFORMATION_SUMMARY.md with complete architecture documentation

CI/CD:
- Add GitHub Actions workflow (build-backend + build-frontend jobs)
- Automated testing on every push and PR

RESULT: Professional-grade portfolio project demonstrating:
- Clean Architecture (5-layer separation)
- Observer design pattern
- Enterprise .NET practices
- Secure credential handling
- Comprehensive testing
- Automated deployment pipeline"
```

## 4. Push to GitHub

### Set Remote (if not already configured)
```powershell
git remote add origin https://github.com/[YOUR-USERNAME]/sismographs.git
# or if already set:
git remote set-url origin https://github.com/[YOUR-USERNAME]/sismographs.git
```

### Verify Remote
```powershell
git remote -v
# Output should show:
# origin  https://github.com/[YOUR-USERNAME]/sismographs.git (fetch)
# origin  https://github.com/[YOUR-USERNAME]/sismographs.git (push)
```

### Push Commits
```powershell
git push origin main
# or if using develop branch:
git push origin develop
```

### Create Pull Request (if using GitHub Flow)
If you prefer PRs for review:
```powershell
git push origin feature-branch-name
# Then create PR on GitHub from feature-branch to main
```

## 5. Verify GitHub CI Pipeline

1. Go to: https://github.com/[YOUR-USERNAME]/sismographs
2. Click "Actions" tab
3. Verify workflows running:
   - ✅ build-backend (should run dotnet build + dotnet test)
   - ✅ build-frontend (should run npm build)
4. When complete, check badges display in README

## 6. Configure GitHub Settings (Optional but Recommended)

### Enable Branch Protection
1. Go to Settings → Branches
2. Under "Branch protection rules", click "Add rule"
3. Apply to: `main`
4. Enable:
   - ✓ Require a pull request before merging
   - ✓ Require status checks to pass before merging
   - ✓ Require branches to be up to date before merging

### Add CI Badge
Edit README.md and add (after title):
```markdown
![Build Status](https://github.com/[YOUR-USERNAME]/sismographs/workflows/CI/badge.svg?branch=main)
```

## 7. Present the Project

### Show Reviewers:
1. **README.md** - Professional overview and architecture
2. **TRANSFORMATION_SUMMARY.md** - Detailed phase-by-phase changes
3. **GitHub Actions** - Show passing CI pipeline
4. **Test Results** - 14 passing unit tests
5. **Code Quality** - Clean Architecture pattern, Observer implementation
6. **Security** - Environment-based configuration, no exposed secrets

### Talking Points:
- "Started with university project, elevated to enterprise architecture"
- "Implemented Clean Architecture with 5-layer separation"
- "Demonstrated Observer pattern for event-driven notifications"
- "Added comprehensive test suite (14 tests, all passing)"
- "Configured GitHub Actions for automated CI/CD validation"
- "Secured all credentials using environment variables"
- "Full-stack: React + .NET + SQL Server integration"

## Troubleshooting

### Q: Build fails with "dotnet not found"
A: Ensure .NET 9 SDK is installed: `dotnet --version`

### Q: Tests fail with mocking errors
A: Run `dotnet clean` then `dotnet build` to refresh

### Q: Frontend build slow
A: Run `npm ci --no-optional` to speed up dependencies

### Q: Git push rejected
A: Ensure correct remote: `git remote -v`
   If different repo, update: `git remote set-url origin [NEW-URL]`

### Q: Environment variables not loading
A: Verify .env file exists in Api/ directory (not in root)
   Check file encoding is UTF-8 (not UTF-16)

## File Checklist Before Commit

- [ ] `.env` file exists but is `.gitignore`'d
- [ ] `.env.example` is committed (template)
- [ ] No Console.WriteLine statements printing credentials
- [ ] `appsettings.json` uses localhost\SQLEXPRESS
- [ ] CORS uses `WithOrigins(env)` not `AllowAnyOrigin()`
- [ ] Test projects build successfully
- [ ] All 14 tests pass
- [ ] Frontend builds successfully
- [ ] `git status` shows no build artifacts or secrets
- [ ] `.github/workflows/ci.yml` exists and is valid YAML

---

**Ready to demonstrate a professional-grade .NET portfolio project!** 🚀
