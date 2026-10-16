# Application design

The HTTP API routes requests to application services. Entity Framework Core services own persistence and transaction boundaries; controllers do not edit database entities directly in the contact workflow.

## Contact management

`IContactService` is the application boundary. `ContactService` loads the persisted entity and calls `Contact.UpdateDetails` or `Contact.Deactivate`. UpdateDetails validates every argument before changing any field, including supported staff ranks and phone numbers. Ordinary edits do not accept activation changes. Creation returns the saved entity so the HTTP Location header uses the database-generated ID. Invalid creation details now return HTTP 400 instead of being stored.

Entity properties remain public for compatibility with the existing EF model, JSON contracts, and legacy callers. This is behavioral encapsulation, not complete prevention of direct property assignment. A future migration to separate update DTOs and private setters should update all callers together rather than silently breaking existing contracts.

## SMS

`MessageService` owns recipient eligibility and message composition. It depends on `ISmsSender`, not Twilio configuration or global client initialization. `TwilioSmsSender` encapsulates credentials, per-call clients, normalization, and delivery. The demo's `DemoSms` also delegates delivery to that adapter while retaining its verification responsibilities. Tests substitute a recording sender without contacting Twilio.

The recall-facing `IMessageService` exposes only recall sending; arbitrary phone-number delivery belongs to the lower-level transport interface. Changing providers means implementing ISmsSender and changing dependency injection registration, without changing recall rules.

## SOLID and object-oriented choices

- Single responsibility: controllers handle HTTP, contact entities validate edits, services coordinate persistence, and the transport integrates with Twilio.
- Open/closed and dependency inversion: recall delivery can use another ISmsSender implementation through dependency injection.
- Liskov substitution and polymorphism: recording and Twilio transports implement the same contract; business logic uses either through the interface.
- Interface segregation: contact operations and recall messaging use separate contracts; provider delivery is separate from recall messaging.
- Encapsulation: validated contact operations and private transport configuration centralize behavior. Public EF/JSON setters remain a documented compatibility limitation.
- Abstraction: application and transport interfaces expose their responsibilities without exposing provider configuration.
- Inheritance: controllers retain the ASP.NET ControllerBase/TwilioController framework hierarchy. No artificial domain base class was added because the entities do not have a meaningful substitutable parent-child relationship. Composition is appropriate for transport replacement.

Existing recipient snapshots, transaction behavior, acknowledgment deduplication, database schema, routes, and frontend are retained. Regression tests cover direct service validation, atomic rejected edits, activation preservation, generated IDs, and transport substitution alongside the existing integrity suite.
