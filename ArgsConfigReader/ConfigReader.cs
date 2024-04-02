using System.Reflection;

namespace ArgsConfigReader; 

public static class ConfigReader {
    public static T? readConfigFromArgs<T>(params string[] args) {
        var properties = typeof(T).GetProperties()
            .Select(propertyInfo => readField(args, propertyInfo))
            .ToArray();
        return (T?)Activator.CreateInstance(typeof(T), properties);
    }

    private static object? readField(string[] args, PropertyInfo propertyInfo) {
        var configProperty = (ConfigPropertyAttribute?)propertyInfo.GetCustomAttribute(typeof(ConfigPropertyAttribute), false);
        if (configProperty == null) {
            return null;
        }

        var overwriteValue = readArg(args, "-" + configProperty.overwriteFlag);
        var longValue = readArg(args, "--" + configProperty.longName);
        var shortValue = readArg(args, "-" + configProperty.shortName);
        var argValue = overwriteValue.Item1
            ? configProperty.overwriteValue
            : longValue.Item1
                ? configProperty.solo
                    ? "True"
                    : longValue.Item2
                : shortValue.Item1
                    ? configProperty.solo
                        ? "True"
                        : shortValue.Item2
                    : configProperty.defaultValue;
            
        if (argValue == null) {
            return null;
        }

        if (propertyInfo.PropertyType.IsEnum) {
            var e = Activator.CreateInstance(propertyInfo.PropertyType);
            return e == null ? null : Enum.Parse(propertyInfo.PropertyType, argValue, true);
        }

        if (propertyInfo.PropertyType == typeof(bool)) {
            return bool.Parse(argValue);
        }

        return Convert.ChangeType(argValue, propertyInfo.PropertyType);
    }
    
    private static (bool, string?) readArg(string[] args, string arg) {
        var indexOfArg = args.ToList()
            .FindIndex(a => a == arg);

        var found = indexOfArg >= 0;
        var value = indexOfArg > args.Length - 2 ? null : args[indexOfArg + 1];
        return (found, value);
    }

    public static void printHelp<T>(TextWriter textWriter) {
        var executable = Path.GetFileNameWithoutExtension(typeof(T).Assembly.Location);
        var properties = typeof(T).GetProperties();
        var attributes = properties.Select(info => (ConfigPropertyAttribute?)info.GetCustomAttribute(typeof(ConfigPropertyAttribute), false))
            .NonNull()
            .ToList();
        var shortFlags = attributes.Select(attribute => attribute.shortName)
            .NonNull()
            .ToList();
        var overwriteFlagSpacing = Math.Max(attributes.Select(attribute => attribute.overwriteFlag)
            .NonNull()
            .Max(flag => flag.Length) + 1, 5) + 3;
        var longFlagSpacing = Math.Max(attributes.Select(attribute => attribute.longName)
            .NonNull()
            .Max(flag => flag.Length) + 2, 4) + 3;
        
        var defaultSpacing = Math.Max(attributes.Select(attribute => Math.Max(attribute.defaultValue?.Length ?? 0, attribute.overwriteFlag != null ? attribute.overwriteValue?.Length ?? 0 : 0))
            .NonNull()
            .Max(), 7) + 3;

        var shortFlagSpacing = Math.Max(Math.Max(shortFlags.Max(flag => flag.Length) + 1, 5) + 3, overwriteFlagSpacing);
        
        textWriter.WriteLine("Usage: {0} -[{1}]", executable, string.Join("", shortFlags));
        textWriter.WriteLine("Optional flags:");
        writeWithOffsets(textWriter, "Short", "Full", "Default", "Description", shortFlagSpacing, longFlagSpacing, defaultSpacing);
        attributes.ForEach(
            attribute => {
                writeWithOffsets(
                    textWriter,
                    attribute.shortName == null ? "" : "-" + attribute.shortName,
                    attribute.longName == null ? "" : "--" + attribute.longName,
                    attribute.defaultValue ?? "",
                    attribute.description ?? "",
                    shortFlagSpacing,
                    longFlagSpacing,
                    defaultSpacing);
                if (attribute.overwriteFlag != null) {
                    writeWithOffsets(
                        textWriter,
                        "-" + attribute.overwriteFlag,
                        "",
                        attribute.overwriteValue ?? "",
                        "",
                        shortFlagSpacing,
                        longFlagSpacing,
                        defaultSpacing
                        );
                }
            });
    }

    private static void writeWithOffsets(TextWriter textWriter, string shortFlag, string longFlag, string defaultValue, string description, int shortFlagSpacing, int longFlagSpacing, int defaultSpacing) {
        textWriter.WriteLine("{0}{4}{1}{5}{2}{6}{3}", shortFlag, longFlag, defaultValue, description, new string(' ', shortFlagSpacing - shortFlag.Length), new string(' ', longFlagSpacing - longFlag.Length), new string(' ', defaultSpacing - defaultValue.Length));
    }
}