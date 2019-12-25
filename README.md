# KsWare.Presentation.Converters
Converters for KsWare Presentation Framework

- [BooleanComparingConverter](##BooleanComparingConverter)
- [DisplayTimeSpanConverter](##DisplayTimeSpanConverter)
- [HexConverter](##HexConverter)
- [SingleValueThicknessConverter](##SingleValueThicknessConverter)
- [StringConverter](##StringConverter)
- [StringJoinConverter](##StringJoinConverter)
- [TemplateConverter](##TemplateConverter)
- [TypeNameConverter](##TypeNameConverter)
- [VisibilityConverter](##VisibilityConverter)

## BooleanComparingConverter

## DisplayTimeSpanConverter

## HexConverter

## SingleValueThicknessConverter

## StringConverter

## StringJoinConverter

## TemplateConverter

## TypeNameConverter

## VisibilityConverter

```xml
<TextBlock Visibility="{Binding Converter={ksv:VisibilityConverter TrueVisibleElseCollapsed}}"/>
```
```xml
<TextBlock Visibility="{VisibilityBinding ., TrueVisibleElseCollapsed}"/>
```


| |Master|Develop|Kux|
|---|---|---|---|
|Build|[![Build status](https://ci.appveyor.com/api/projects/status/f6egmwg7elfxua7y/branch/master?svg=true)](https://ci.appveyor.com/project/KsWare/ksware-presentation-converters/branch/master)|[![Build status](https://ci.appveyor.com/api/projects/status/f6egmwg7elfxua7y/branch/develop?svg=true)](https://ci.appveyor.com/project/KsWare/ksware-presentation-converters/branch/develop)|[![Build status](https://ci.appveyor.com/api/projects/status/f6egmwg7elfxua7y/branch/develop?svg=true)](https://ci.appveyor.com/project/KsWare/ksware-presentation-converters/branch/features/kux)|
|Test|![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/ksware/ksware-presentation-converters/master)|![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/ksware/ksware-presentation-converters/develop)|![AppVeyor tests (branch)](https://img.shields.io/appveyor/tests/ksware/ksware-presentation-converters/features/kux)|
|Nuget||[![NuGet Badge](https://buildstats.info/nuget/KsWare.Presentation.Converters)](https://www.nuget.org/packages/KsWare.Presentation.Converters/)|
