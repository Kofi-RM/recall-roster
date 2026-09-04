# Recall Roster database-integrity rollout

The code fixes are implemented. The live SQL Server database has NOT been migrated, and no live SMS messages were sent during verification.

## What changed

- Recall creation validates the roster, active recipients, supported ranks, phone numbers, message, and deadline. The server sets start time and counts.
- Each new recall and its recipient snapshots are committed in one serializable transaction. Snapshots retain names, ranks, and destination phone numbers even if the source roster/contact later changes.
- Acknowledgments must match the saved contact ID, recall ID, and phone number. Late acknowledgments remain valid and are labeled late.
- Only the signature-validated Twilio webhook can insert acknowledgments. The former general-purpose response POST and public test-SMS action were removed. Response reads and SMS submission require authentication.
- The first acknowledgment wins. A filtered unique index prevents two canonical responses for the same recipient/recall. Historical duplicate rows remain stored with IsDuplicate=1, not deleted. Response insertion and counter increment are atomic.
- Roster additions are deduplicated and existing members are not inserted again. Add/remove overlap, missing rosters, and invalid/inactive additions are rejected without partial saves.
- Statistics use snapshots, not current roster memberships; refresh failures do not mark people unresponsive. Recall creation awaits SMS submission and reports partial/ambiguous failures without falsely claiming delivery.
- Removed JWT/Twilio credential logging and password-hash exposure in the user-detail response.

## Before deployment

1. Back up the intended database and verify the backup can be restored. Test this rollout against a restored copy, with a test SMS account or stubbed provider.
2. Schedule a maintenance window and finish existing active recalls. The migration refuses unfinished legacy recalls so it cannot silently interrupt their response workflow.
3. Confirm the database's actual schema and EF migration history. It could not be inspected in this session: the configured SQL Server connection failed with a client security/TLS error.
4. Check for both User and Logins tables, duplicate normalized emails, custom triggers, orphan response foreign keys, and schema customizations. The migration stops rather than guessing how to reconcile ambiguous account data or enabled recall/response/roster triggers. Do not blindly bypass these checks.
5. Preserve account IDs and valid BCrypt password hashes when resolving legacy account tables. The migration does not reset passwords or convert plaintext credentials.
6. Configure the SMS settings below before restarting the backend. Deploy frontend and backend together after the schema update.

## Migration behavior and execution

The new LegacyContactsBaseline migration precedes the old 2024 migration and creates Contacts only when missing. This repairs the missing empty-database baseline without replacing an existing table.

RecallIntegrity renames a lone Logins table to User instead of dropping accounts; normalizes email addresses and adds uniqueness; adds response text where absent; adds recipient snapshots; archives old totals in LegacyTotal before defining Total as the sum of rank counters; and replaces response cascade deletes with restrictive foreign keys.

Old active and LegacyTotal columns remain as audit/compatibility data outside the runtime model. No historical recipient list is fabricated. Old recalls are marked HasRecipientSnapshot=false: they display recorded acknowledgments only, without a completion percentage or purported historical names/ranks. New webhook acknowledgments are not accepted for legacy recalls.

Generate a reviewed SQL Server deployment script from the repository root:

```powershell
dotnet ef migrations script --idempotent --project recall_roster --output recall-roster-migration.sql
```

An exported copy was provided with this handoff. Review its target database and run it on a restored copy before production. Use a SQL deployment tool that stops on the first error and rolls back an open transaction; do not continue executing subsequent batches after a guard fails. Do not manually edit EF migration history to make the script pass without reconciling the actual schema.

The design-time factory uses a placeholder database only for migration generation. It deliberately does not read production secrets. Do not use an unqualified `dotnet ef database update`; use the reviewed SQL script against the explicitly selected database.

For a manually assembled legacy database without a reliable migration history, a DBA must baseline its actual schema first. The idempotent script relies on migration history; it is not a universal repair script for arbitrary drift.

This migration is forward-only. Restore the verified pre-deployment backup to roll back; its Down method refuses to discard newly collected snapshots or response history.

## Required SMS configuration

Use environment variables or an appropriate secret store, not committed credentials:

- Twilio__AccountSid: the existing account SID.
- Twilio__AuthToken: the auth token used to validate inbound signatures and submit outbound SMS.
- Twilio__FromNumber: the existing approved Twilio sending number in E.164 format. This replaces the formerly hardcoded sender.
- Twilio__WebhookUrl: the EXACT public incoming-SMS URL configured in Twilio, including path and any query string. For ngrok/reverse proxies this is essential; localhost/internal hostnames will not match the public signature.
- Existing Jwt and database connection settings remain required.

Signature verification uses Twilio's SDK over the complete form parameters and URL. No bypass is included. Without an auth token or a valid signature, the webhook returns 403.

Reference: [Twilio's ASP.NET Core request-validation documentation](https://www.twilio.com/docs/usage/tutorials/how-to-secure-your-csharp-aspnet-core-app-by-validating-incoming-twilio-requests).

If earlier runs wrote real JWT signing keys or Twilio auth tokens to logs, rotate those credentials and review access to the logs. Rotation was not performed automatically.

## Verification

- Backend regression suite: isolated SQLite relational tests for snapshots, validation, duplicates, atomic updates, and sender matching; controller tests for webhook signatures and response API restrictions.
- SQL Server model/snapshot consistency and SQL Server 2022 syntax parsing of the generated idempotent migration are tested.
- Frontend tests cover immutable progress data, legacy warnings, UTC timestamps, creation errors, and partial SMS submission.
- Frontend production build and backend compilation succeed; pre-existing warnings remain.
- SQLite tests and SQL syntax checks do NOT replace executing this migration on an actual SQL Server staging database.

Run:

```powershell
dotnet test recall_roster.Tests/recall_roster.Tests.csproj
cd frontendrecall
npm test -- --watchAll=false --runInBand
npm run build
```

## Known limits

- Original recipients for legacy recalls cannot be recovered from information that was never saved.
- SMS submission is not delivery confirmation. A provider/network timeout can be ambiguous; inspect the provider's logs before retrying. This update does not implement a durable outbox or exactly-once outbound SMS delivery.
- Existing registration and account-administration policy, broader authorization roles, rate limits, and dependency upgrades still need a separate production-hardening review.
