# ArgsConfigReader

A simple tool to read configuration from arguments.

## Usage
### Annotation
A custom annotation, `ConfigProperty`, is provided for properties. Putting this annotation onto simple objects allows
reading that object from arguments.
It has the following properties:
* `shortName`: The short name of the config field, read with a single `-` in front.
* `longName`: The full name of the config field, read with `--` in front.
* `defaultValue`: The default value of the config field. This will be set for the field if the argument is not provided.
* `description`: The description of the config field. Used to display information about the field when printing help.
* `overwriteFlag`: Setting this flag enables a config field to be overriden with a single value, instead of setting an
explicit value in the arguments.
* `overwriteValue`: The value the field will be overwritten to. Requires the `overwriteFlag` to be set.
* `solo`: For boolean properties. If this is set to true, the value after the flag of the field will be ignored. Instead
it will be set to `true` if the flag is present at all.

### Creating a config class
The above annotation can be put onto public properties of classes (or structs/records). Some examples:  
#### 1. Short name for property:
```csharp
[ConfigProperty(shortName = "p")]
public string propertyWithShortName { get; init; }
```
This can be read with `-p someValue` to set its value to `someValue`

#### 2. Long name for property:
```csharp
[ConfigProperty(longName = "property")]
public string propertyWithLongName { get; init; }
```
This can be read with `--property someValue` to set its value to `someValue`

#### 3. Default values:
```csharp
[ConfigProperty(shortName = "p", defaultValue = "someDefault")]
public string propertyWithDefaultValue { get; init; }
```
This can be read like the short name above. If it's not provided, its value will be set to `someDefault`.

#### 4. Description:
```csharp
[ConfigProperty(shortName = "p", description = "A property with a description")]
public string propertyWithDescription { get; init; }
```
This can be read like the short name above. This information will be printed along the field when the help is printed.

#### 5. Overwrite flags:
```csharp
[ConfigProperty(shortName = "p", overwriteFlag = "o", overwriteValue = "overwritten")]
public string propertyWithOverwrite { get; init; }
```
This requires both overwrite parameters to be set. This can be read like the short name above. If the `-o` flag is
present in the arguments, the value after `-p` is ignored, and the field's value will be set to `overwritten`.

#### 6. Solo properties:
```csharp
[ConfigProperty(shortName = "p", solo = true)]
public bool propertySolo { get; init; }
```
This can be read with `-p` to set its value to `true`. Anything after `-p` will be ignored for this field. If it's not
present, it will be set to `false`.

### Printing help
A `printHelp` generic method is also provided. This reads the metadata from the annotations and constructs a help message.  
Example:
```csharp
public class TestConfig(string testStringProperty, int testIntProperty, bool testSoloBoolProperty, string testOverwriteProperty) {
    [ConfigProperty(shortName = "p", longName = "property", defaultValue = "default", description = "A property")]
    public string testStringProperty { get; } = testStringProperty;

    [ConfigProperty(shortName = "i", longName = "int", defaultValue = "12", description = "An int property")]
    public int testIntProperty { get; } = testIntProperty;

    [ConfigProperty(shortName = "b", longName = "bool", defaultValue = "false", solo = true)]
    public bool testSoloBoolProperty { get; } = testSoloBoolProperty;

    [ConfigProperty(shortName = "o", longName = "overwrite", defaultValue = "def", description = "An overwriteable property", overwriteFlag = "w", overwriteValue = "overwritten")]
    public string testOverwriteProperty { get; } = testOverwriteProperty;
}

public void help() {
    ConfigReader.printHelp<TestConfig>(Console.Out);
}
```
Calling `help()` will print:
```text
Usage: <NameOfRunnable> -[pibo]
Optional flags:
Short   Full          Default       Description
-p      --property    default       A property
-i      --int         12            An int property
-b      --bool        false         
-o      --overwrite   def           An overwriteable property
-w                    overwritten   

```