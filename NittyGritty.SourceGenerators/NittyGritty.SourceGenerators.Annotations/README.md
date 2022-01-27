# NittyGritty Source Generator Annotations
This package contains the following annotations:

- AlsoNotifyAttribute
- CommandAttribute
- DialogAttribute
  - For the source generator to match your dialog with the correct view model with a `DialogKey` attribute, they should be within the same project. If your view models are in a separate project, you should also set the `ViewModel` property of this attribute to the type of your view model.
- DialogKeyAttribute
- InstanceAttribute
  - Classes marked with this attribute will only be picked up the source generator if it is within the same project that has the class marked with the `LocatorAttribute`.
- LocatorAttribute
- NotifyAttribute
- PageAttribute
  - For the source generator to match your page with the correct view model with a `ViewModelKey` attribute, they should be within the same project. If your view models are in a separate project, you should also set the `ViewModel` property of this attribute to the type of your view model.
- ViewModelKeyAttribute
- AccessLevel
- InstanceLifetime