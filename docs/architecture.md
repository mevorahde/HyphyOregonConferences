# Architecture

The modern solution separates input/output concerns from assignment rules.

```mermaid
flowchart LR
    Host[CLI host<br/>process and exit codes]
    App[Application workflow<br/>parsing, prompting, presentation]
    Core[Core assignment engine<br/>validation and Fisher-Yates assignment]
    Random[Injected random source<br/>system-backed or SplitMix64-v1]

    Host --> App
    App --> Core
    Core --> Random
```

`HyphyOregon.ConferenceGenerator.Core` owns immutable domain models,
validation, balanced assignment, and the bounded-random interface. It has no
console, filesystem, environment, network, process, logging, or UI access.

`HyphyOregon.ConferenceGenerator.Cli` depends on Core and supplies argument
parsing, interactive prompting through injected text streams, presentation,
random-source composition, safe diagnostics, cancellation, and exit-code
mapping.

The test project depends on both production projects. Production projects
never depend on tests. Randomness points inward through Core's
`IBoundedRandomSource`, so seeded and system-backed behavior can be tested
without coupling the domain engine to console or runtime state.
