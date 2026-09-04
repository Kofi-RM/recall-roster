# Capstone-Gov_Recall
Repo for the Gov. Recall Project of Fall 23 - Spring 24.

## Database integrity update

Read [the rollout guide](docs/database-integrity-rollout.md) before running the updated backend.
The schema migration must be reviewed and tested on a backup first. Do not run the old scratch SQL files in the frontend source folder.

Regression tests:

```powershell
dotnet test recall_roster.Tests/recall_roster.Tests.csproj
cd frontendrecall
npm test -- --watchAll=false --runInBand
npm run build
```
