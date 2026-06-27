# NorthSouthSystems.Opinions.Entities.Vogen

This .NET library contains opinionated value objects for working with Microsoft.EntityFrameworkCore + SqlServer.

# DO NOT USE PolyType.GenerateShapeAttribute IN THIS PROJECT / ASSEMBLY!

We created this standalone Vogen project in order to separate Vogen source generation from where PolyType
GenerateShapeAttribute is used (i.e. PolyType source generation) due to a known design constraint whereby PolyType
cannot "see" the ```[TypeShape(Marshaler = typeof(PolyTypeMarshaler))]``` that Vogen adds to each ValueObject.

[Nerdbank.MessagePack discusses this constraint.](https://aarnott.github.io/Nerdbank.MessagePack/docs/type-shapes.html#working-with-vogen)
It presents an alternative solution to use the `ReflectionTypeShapeProvider.Default`; however, doing so potentially
introduces other issues.

Using ```<PackageReference ... PrivateAssets="analyzers"/>``` we've blocked Vogen source generation analyzers from
flowing to projects that reference this project. Unfortunately, some compiler warning functionality is lost; however,
the increased potential of Vogen + PolyType bugs is not worth the value of those warnings.