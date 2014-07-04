Lotus
=====

An Entity-Component-System genre-agnostic game engine.

Entities
====
Entities are just collections of Aspects with a little bit of metadata. Each Entity has a unique ID 

Aspects
====
Aspects are mostly just collections of raw data. They also have methods for retrieving, modifying, and serializing their data. Aspects do not directly communicate with other Aspects or with Managers, but they can communicate indirectly via Signals and Slots. Aspects can be subclassed for objects with near-identical functionality when an 'is-a' relationship is more ideal than a 'has-a' relationship, but they must provide functionality to the same extent and scope as their parent class. Parent-child relationships and other external connections are exposed through relational Aspects.

Managers
====
Managers are the backbone of the game logic and deal with the interaction between Aspects, as well a services like audio and rendering. Managers register themselves for specific combinations of Aspects, and recieve callbacks when an Entity with a matching combination is created, destroyed, or no longer fits the criteria.

Slots and Signals
====
Signals can be wired to Slots by Managers, so when the Signal is emitted by the first Aspect the function of the Slot of the second Aspect will automatically fire. Managers can also have Slots and Signals. Parents and children can 'forward' Signals to Slots through relational Aspects, but strictly speaking Signals and Slots are confined to same-Entity components.

Future Considerations
====
Does an Entity store its Aspects or reference its Aspects? Should Aspects be in the Entity, in EntityID-indexed arrays or dictionaries, or in AspectID-indexed arrays or dictionaries? Is locality of reference really that important in a managed language?