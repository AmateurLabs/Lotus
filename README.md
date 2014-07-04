Lotus
=====

An Entity-Component-System genre-agnostic game engine.

Entities
-----
Entities are just collections of Aspects with a little bit of metadata. Each Entity has a unique identification number, a list of its Aspects, and a list of connections between the Signals and Slots of its Aspects. Entities have no default Aspects or other game-related properties, but lightweight Entity wrappers which cache references to specific Aspects and provide other helper functions are acceptable.

Aspects
-----
Aspects are mostly just collections of raw data. They also have methods for retrieving, modifying, and serializing their data. Aspects do not directly communicate with other Aspects or with Managers, but they can communicate indirectly with other Aspects via Signals and Slots. Aspects can be subclassed for objects with near-identical functionality when an 'is-a' relationship is more ideal than a 'has-a' relationship, but they must provide functionality to the same extent and scope as their parent class. Parent-child relationships and other external connections are exposed through relational Aspects.

Templates
-----
Templates are a type of data that represents an Entity and its Aspects. Their primary use is in content saving and loading, where they can represent a snapshot of an Entity at a given point, a prefab for Entities that haven't been created yet, or any other representations of an Entity. Templates are the primary form of persistant data.

Managers
-----
Managers are the backbone of the game logic and deal with the interaction between Aspects, as well a services like audio and rendering. Managers register themselves with the central engine code, which provides callbacks for when an Entity with Aspects matching a Manager-specified combination is created, destroyed, or no longer fits the criteria. Managers cannot communicate with each other directly, but they can manipulate Aspects and share data that way.

Slots and Signals
-----
Signals and Slots are components similar to events and event listeners that allow simple, common interactions between Aspects. Signals can be wired to Slots by Managers, so when the Signal is emitted by the first Aspect the function of the Slot of the second Aspect will automatically fire. Parents and children can 'forward' Signals to Slots through relational Aspects, but strictly speaking Signals and Slots are confined to same-Entity components.

Future Considerations
-----
Does an Entity store its Aspects or reference its Aspects? Should Aspects be in the Entity, in EntityID-indexed arrays or dictionaries, or in AspectID-indexed arrays or dictionaries? Is locality of reference really that important in a managed language? Should Managers have their own Slots and Signals? Should Managers be able to serialize data, or should they have to rely on Aspects alone?