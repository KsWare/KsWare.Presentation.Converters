# KsWare.Presentation.Converters
Converters for KsWare Presentation Framework

- [BooleanComparingConverter](##BooleanComparingConverter)
- [DisplayTimeSpanConverter](##DisplayTimeSpanConverter)
- [HexConverter](##HexConverter)
- [SingleValueThicknessConverter](##SingleValueThicknessConverter)
- [StringConverter](##StringConverter)
- [StringJoinConverter](##StringJoinConverter)
- [ResourceConverter](##ResourceConverter)
  - ResourceConverterExtension](###ResourceConverterExtension)
  - EntryAssemblyResourceConverterExtension](###ExecutingAssemblyResourceConverterExtension)
- [TypeNameConverter](##TypeNameConverter)
- [VisibilityConverter](##VisibilityConverter)
  - [VisibilityBinding](###VisibilityBinding)
## BooleanComparingConverter

## DisplayTimeSpanConverter

Converter to format a TimeSpan with some custom formats. 
- e.g. Total hours : Minutes : seconds "hhh:mm:ss" => 486:59:59
- TotalDays: ddd
- TotalHours: hhh
- TotalMinutes: mmm e.g. "mmm:ss" => 123:59
- TotalSeconds: sss

```xml
<TextBlock Text="{Binding Converter={DisplayTimeSpanConverter 'mmm:ss'}}"/>
```

## HexConverter

Converts a numeric value in the hexadecimal string presentation

## SingleValueThicknessConverter

Converts a single numeric value to a thickness.

- ```Left```: new Thickness(value, 0, 0, 0)
- ```Top```: new Thickness(0, value, 0, 0)
- ```Right```: new Thickness(0, 0, value, 0)
- ```Bottom```: new Thickness(0, 0, 0, value)

Negative values
- ```-Left```: new Thickness(-value, 0, 0, 0)
- ```-Top```: new Thickness(0, -value, 0, 0)
- ```-Right```: new Thickness(0, 0, -value, 0)
- ```-Bottom```: new Thickness(0, 0, 0, -value)

Combined values
- ```* -* 80 20```: new Thickness(value, -value, 80, 20)
```xml
<TextBlock Text="{Binding Converter={SingleValueThicknessConverter Left}}"/>
```
```xml
<TextBlock Text="{Binding Converter={SingleValueThicknessConverter '* -* 80 20'}}"/>
```

## StringConverter
Converts anything to a string.

## StringJoinConverter


## ResourceConverter

Example: Icon="Example.xaml" or "Example.ico" ,...
```xml
<MenuItem Icon="{Binding Icon, Converter={x:Static ksv:ResourceConverter.Default}, ConverterParameter=/Kushed;component/Resources/" />
```

Example: Icon="Example"
```xml
<MenuItem Icon="{Binding Icon, Converter={x:Static ksv:ResourceConverter.Default}, ConverterParameter=pack://application:,,,/MyAssembly;component/Resources/{0}.xaml}" />
```
### ResourceConverterExtension

Example: Icon="Example"
```xml
<MenuItem Icon="{Binding Icon, Converter={ksv:ResourceConverter pack://application:,,,/MyAssembly;component/Resources/{0}.xaml}}" />
```

Example: Icon="Example.xaml" located in same folder of current document
```xml
<MenuItem Icon="{Binding Icon, Converter={ksv:ResourceConverter}" />
```
Example: Icon="Example.xaml" located in Data folder of current documents assembly
```xml
<MenuItem Icon="{Binding Icon, Converter={ksv:ResourceConverter /Data}" />
```
Example: Icon="Example.xaml" located in Sub folder of current documents.
```xml
<MenuItem Icon="{Binding Icon, Converter={ksv:ResourceConverter /Sub}" />
```
Example: Icon="Example.xaml" located in Data folder of specified assembly.
```xml
<MenuItem Icon="{Binding Icon, Converter={ksv:ResourceConverter MyAssembly, /Data}" />
```
Example: Icon="Example.xaml" located in Data folder of entry assembly.
```xml
<MenuItem Icon="{Binding Icon, Converter={ksv:EntryAssemblyResourceConverter EntryAssembly, /Data}" />
```

### EntryAssemblyResourceConverterExtension
Example: Icon="Example.xaml" located in Data folder of entry assembly.
```xml
<MenuItem Icon="{Binding Icon, Converter={ksv:EntryAssemblyResourceConverter /Data}" />
```


## TypeNameConverter

## VisibilityConverter
Converts a boolean value in a Visibility.

Operators: ```IsNull```, ```IsNullOr0```, ```IsTrue```, ```IsFalse```, ```IsEqual```, ```IsNotEqual```
- ```IsNullOr0``` good to check if collection is null or empty.


```xml
<TextBlock Visibility="{Binding Converter={VisibilityConverter TrueVisibleElseCollapsed}}"/>
```
```xml
<TextBlock Visibility="{VisibilityBinding ., TrueVisibleElseCollapsed}"/>
```
```xml
<TextBlock Visibility="{Binding Converter={VisibilityConverter IsTrue, Visible, Hidden}}" />
```
### VisibilityBinding

| |Master|Develop|Kux|
|---|---|---|---|
|Build|[![Build status](https://ci.appveyor.com/api/projects/status/f6egmwg7elfxua7y/branch/master?svg=true)](https://ci.appveyor.com/project/KsWare/ksware-presentation-converters/branch/master)|[![Build status](https://ci.appveyor.com/api/projects/status/f6egmwg7elfxua7y/branch/develop?svg=true)](https://ci.appveyor.com/project/KsWare/ksware-presentation-converters/branch/develop)|[![Build status](https://ci.appveyor.com/api/projects/status/f6egmwg7elfxua7y/branch/develop?svg=true)](https://ci.appveyor.com/project/KsWare/ksware-presentation-converters/branch/features/kux)|
|Test|![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/ksware/ksware-presentation-converters/master)|![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/ksware/ksware-presentation-converters/develop)|![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/ksware/ksware-presentation-converters/features/kux)|
|Nuget||[![NuGet Badge](https://buildstats.info/nuget/KsWare.Presentation.Converters)](https://www.nuget.org/packages/KsWare.Presentation.Converters/)|
