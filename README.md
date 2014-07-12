Lotus
=====

A general-purpose game engine based on OpenTK and a custom Entity-Component System.

ECS Guidelines
=====

Entities are just an integer ID. No data should ever be stored directly in an Entity.

If two components have shared data, split it off into its own component. If you find yourself wanting to use inheritance with components, e.g. a CubeMesh component and FileMesh component, split it up into a Shape component and Mesh component.

Components should _never_ access other components. Inter-component behavior is what Processors are for.