# Public SMS demo

The public demo verifies a visitor's own phone with Twilio Verify, then permits **two lifetime SMS attempts per number**, shared across sessions and browsers. It sends one of two fixed demo messages, never arbitrary text or staff contacts. No login, SQL Server, or production data is needed in demo-only mode. Verification sends an additional code SMS and has its own cost.

## Deploy as one Docker web service

1. Push these changes to your repository. Create a Docker web service on a host that supports a **persistent disk**, using `Dockerfile.demo` at the repository root.
2. Mount a persistent disk at `/data`. Run **one instance**. The disk must preserve `/data/demo-ledger.json` across restarts and deploys. Do not use ephemeral/serverless storage or independent replicas: removing the ledger resets quotas. The application uses a cross-process file lock and atomic replacement on a local filesystem.
3. Configure these server environment variables in the host's secret settings:

   | Variable | Value |
   | --- | --- |
   | `Twilio__AccountSid` | Twilio account SID |
   | `Twilio__AuthToken` | Twilio auth token |
   | `Twilio__FromNumber` | Your SMS-capable Twilio sender in E.164 format |
   | `Twilio__VerifyServiceSid` | A Twilio Verify service SID (`VA...`) |
   | `Demo__DailyLimit` | `20` (default; maximum 100) |
   | `Demo__AllowedPrefixes` | `+1` by default; comma-separated dialing prefixes if needed |

   `Dockerfile.demo` already sets `Demo__Enabled=true`, `Demo__Only=true`, `Demo__StorePath=/data/demo-ledger.json`, and port 8080. Terminate HTTPS at your host. Do not set an HTTPS redirect port on the container's HTTP-only listener. No JWT or database credentials are required for this deployment.

4. Configure Twilio Verify's [geographic permissions and fraud protection](https://www.twilio.com/docs/verify) for your intended audience, and use an SMS sender approved for your destination countries. `+1` includes the North American Numbering Plan, not just the US. [Trial accounts restrict recipient numbers](https://www.twilio.com/docs/verify/api/verification), so a trial account will not work for arbitrary portfolio visitors.
5. Open the deployed root URL. Verify your own number, send the two test messages, then confirm a third send is blocked. All normal `/api` endpoints return 404 in demo-only mode. The root and `/demo` show the demo UI.
6. Once deployed, use this URL for Recall Roster's portfolio link and label it **Live demo**. Keep the GitHub link separately if desired.

## Limits and behavior

- Two SMS attempts per phone, stored as a SHA-256 phone key in the durable ledger. This is a pseudonymous identifier, not anonymized data; protect the disk as personal data.
- Default global daily caps: 20 verification starts **and** 20 demo SMS attempts, resetting at midnight UTC. Up to 3 verification starts per phone per UTC day and 5 code checks per session. These cap API activity, not a currency amount; Twilio billing still applies.
- Verification session expires after 10 minutes; an approved session lasts 30 minutes. A new session never resets a phone's two-send quota.
- Sending is reserved durably before contacting Twilio. Provider failures or timeouts consume an attempt because delivery can be ambiguous. The frontend does not automatically retry SMS requests. “Submitted” means accepted by the provider, not confirmed delivered.
- Fixed short messages identify themselves as demos and include opt-out text. Reply handling and full roster management are outside this demo. Configure standard opt-out behavior on your Twilio sender.
- The ledger temporarily holds phone numbers for active sessions; expired sessions are removed on subsequent successful ledger writes. Restrict disk access and retain the ledger for as long as you want the lifetime quota enforced. Raw session tokens are only stored in the visitor's session storage; the server stores hashes.
- No real SMS is sent by automated tests; Twilio is mocked. Actual delivery needs the deployment credentials and a manual check.

## Local demo

Set the server environment variables above plus `Demo__Only=true`, `Demo__Enabled=true`, and an absolute persistent local `Demo__StorePath` outside `wwwroot`. Start the backend on port 5000 with `dotnet run --project recall_roster --no-launch-profile --urls http://localhost:5000`.

In a second PowerShell terminal in `frontendrecall`:

```powershell
$env:REACT_APP_DEMO_ONLY = 'true'
$env:REACT_APP_API_URL = 'http://localhost:5000/api'
npm start
```

Open the frontend URL. The backend allows localhost:3000 and localhost:3001 by default; override `Frontend__Origins` with a comma-separated list if needed. On the regular app build, `/demo` is also available when the backend demo is enabled; set `REACT_APP_API_URL` for that page if the backend runs at a separate origin.

## Validation

```powershell
dotnet test recall_roster.Tests/recall_roster.Tests.csproj
cd frontendrecall
npm test -- --watchAll=false --runInBand
$env:REACT_APP_DEMO_ONLY = 'true'
$env:REACT_APP_API_URL = '/api'
npm run build
```
