# Recall Roster unit access: operator handoff

## Implemented

Units own contacts, rosters, and recalls. Membership grants viewing. Separate permissions grant staff/roster management, recall initiation, and access to descendant units. Rank alone and administrator status alone grant no operational access. Administrators manage units, accounts, grants, revocation, password resets, and audit history. The last enabled administrator is protected.

Backend authorization and default-deny EF filters protect reads. Persistence checks protect writes, immutable ownership, and same-unit roster membership. Only the signature-validated Twilio webhook gets the internal response-writing scope. Recalls record their initiating user. Memberships are checked from the database on every authenticated request. Disabling/enabling accounts and resetting passwords revoke old tokens. UI permissions refresh every 30 seconds and on focus; active recalls and progress refresh every five seconds.

## Local database cutover completed October 15, 2026

The original Roster database had a different migration history from the repository. It was left unchanged rather than assigning fabricated migration entries.

- Original preserved: .\SQLEXPRESS / Roster.
- Copy-only backup verified and successfully restored: C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\RecallRoster_BeforeUnitAccess_20261015.bak.
- Read-only snapshot: Roster_LegacySnapshot_20261015.
- New application database: Roster_UnitAccess_20261015.
- The backend appsettings.json now targets the new database through .\SQLEXPRESS. Other settings were retained.
- Private configuration backup: C:\Users\kofir\Documents\Codex\2026-10-09\ex\work\appsettings-before-unit-access.json. Do not publish it.

Verified copies retain 8 contacts, 2 rosters, 7 recalls, 8 responses, and 2 users, with original IDs and password values. Emails were normalized for login. Old totals, nullable legacy active flags, and the old roster-member responded flag were preserved for audit. Historical recalls have no fabricated recipient snapshots or creator identities. Duplicate responses are archived rather than deleted.

The chosen administrator is kofireevesmiller@gmail.com; its existing valid BCrypt password hash was preserved. The other legacy account had an unsupported credential format and was retained but disabled. Reset its password in the administrator screen before enabling it.

## First use

1. Restart the backend and frontend, then sign in with the administrator's existing password.
2. Open Manage units and access from the dashboard.
3. In Assign unit permissions, choose your account and Legacy / unassigned unit. Check Manage staff and rosters and Initiate recalls to give yourself coordinator access. Administrator status intentionally does not do this automatically.
4. You can rename that legacy unit to the appropriate unit name without changing ownership or IDs.
5. Create squadron/flight units as needed. Grant read-only access with no operational checkboxes. Select child-unit access only when intended. Overlapping grants combine.
6. Use fictional staff and a consenting test phone for demonstrations.

Backend, from the repository root:

```powershell
$env:Demo__Only = 'false'
$env:Demo__Enabled = 'false'
dotnet run --project recall_roster --no-launch-profile --urls http://localhost:5000
```

Frontend, from frontendrecall:

```powershell
$env:REACT_APP_DEMO_ONLY = 'false'
npm start
```

Real SMS replies still need Twilio webhook configuration. A local backend uses a tunnel; a hosted backend uses its public URL. No hosting was deployed and no real SMS was sent during implementation.

## Deployment and rollback

The exported idempotent schema script is for a fresh database or a database using the repository's canonical migration history. Do not run it blindly on the preserved original Roster database. The new local database already has the migrations applied.

For later production deployment, restore-test a backup, validate the target schema, and use server-side secret configuration. The original database and configuration backup are the pre-cutover recovery source. Once new writes exist, reconcile them before switching back; rollback is not a merge. UnitAccess is forward-only to preserve ownership and audit history.

## Verification

46 backend tests and 23 frontend tests passed; the strict production frontend build succeeded. Real HTTP smoke checks verified that anonymous and disabled users receive 401, the chosen administrator can access administration endpoints, and administrator status without unit membership exposes no operational records. Tests cover hierarchy, isolation, revocation, guessed IDs, unauthorized SMS attempts, signed acknowledgments, account lifecycle, and preserved migration data. No live texting was performed.
