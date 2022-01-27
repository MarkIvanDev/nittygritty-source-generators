# Nitty Gritty Uno Source Generators
Please read the documentation for [Nitty Gritty Source Generator Annotations](https://www.nuget.org/packages/NittyGritty.SourceGenerators.Annotations/) first before proceeding.

This package contains the following source generators:

### Locator Generator
Generate a `Locator` class that will configure your `NavigationService`, `DialogService`, view models, and other services by annotating your classes with `[Locator]`, `[Page]`, `[Dialog]`, or `[Instance]` attributes.

**Notes**
- Change the instance lifetime of your `NavigationService` by setting the `NavigationServiceLifetime` property of `LocatorAttribute`.
- Match your view models and pages by annotating your pages with `PageAttribute` and your view models with `ViewModelKeyAttribute`. The generator uses the convention `HomeViewModel` => `HomePage` or `HomeView`. You can override this by setting the `Key` properties of the attributes.
- Match your view models and dialogs by annotating your dialogs with `DialogAttribute` and your view models with `DialogKeyAttribute`. The generator uses the convention `AddUserViewModel` => `AddUserDialog` or `AddUserView`. You can override this by setting the `Key` properties of the attributes.
- Customize your `[Instance]` annotated classes by setting the following properties:
  - **Lifetime** (Change the lifetime of the instance to Singleton, Scoped, or Transient. default: Singleton)
  - **Parent** (Set the parent type of the instance that you want the `Locator` to use when referencing your instance)
  - **Name** (Set the property name of the instance inside the `Locator` class. This will only be used when `Lifetime` is set to Singleton)
- Configure additional services by adding an implementation to the generated partial method `Configure(ServiceCollection services)`.

Example:

```csharp
[Locator]
public partial class Locator { /* ... */ }

[Page]
public partial class HomePage : NGPage { /* ... */ }

[Instance, ViewModelKey]
public class HomeViewModel : ViewModelBase { /* ... */ }

[Dialog]
public partial class AddUserDialog : ContentDialog { /* ... */ }

[DialogKey]
public class AddUserViewModel : ObservableObject { /* ... */ }

[Instance(Parent = typeof(IUserService))]
public class UserService : IUserService { /* ... */ }
```

This will generate:

```csharp
public partial class Locator
{
    private readonly ServiceProvider provider;

    public Locator()
    {
        var services = new ServiceCollection();

        services.AddSingleton<INavigationService>(isp =>
        {
            var navigationService = new NavigationService();
            navigationService.Configure("Home", typeof(HomePage));
            return navigationService;
        });

        services.AddSingleton<IDialogService>(isp =>
        {
            var dialogService = new DialogService();
            dialogService.Configure("AddUser", typeof(AddUserDialog));
            return dialogService;
        });

        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<IUserService, UserService>();

        Configure(services);
        provider = services.BuildServiceProvider();
    }

    partial void Configure(ServiceCollection services);

    public HomeViewModel Home => provider.GetService<HomeViewModel>();
    public IUserService UserService => provider.GetService<IUserService>();
}
```

---

### Page Generator
Generate `ViewModel` properties inside of your `Pages` that you can use in XAML binding. Your Page's `PageAttribute` and your view model's `ViewModelKeyAttribute` should have matching `Keys`.

Example:

```csharp
[Page]
public partial class HomePage : NGPage { /* ... */ }

[ViewModelKey]
public class HomeViewModel : ViewModelBase { /* ... */ }
```

This will generate:

```csharp
public partial class HomePage
{
    public HomeViewModel ViewModel => DataContext as HomeViewModel;
}
```

---

### Dialog Generator
Generate `ViewModel` properties inside of your `ContentDialogs` that you can use in XAML binding. Your Dialog's `DialogAttribute` and your view model's `DialogKeyAttribute` should have matching `Keys`.

Example:

```csharp
[Dialog]
public partial class AddUserDialog : ContentDialog { /* ... */ }

[DialogKey]
public class AddUserViewModel : ObservableObject { /* ... */ }
```

This will generate:

```csharp
public partial class AddUserDialog
{
    public AddUserViewModel ViewModel => DataContext as AddUserViewModel;
}
```

---

##### NOTES
Don't forget to make your classes partial when using the `[Locator]`, `[Page]` and `[Dialog]` attributes. If the generated source code is not being updated even after doing a clean and rebuild, try restarting Visual Studio.